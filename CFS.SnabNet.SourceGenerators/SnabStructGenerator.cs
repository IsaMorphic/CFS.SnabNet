using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Linq;
using System.Text;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CFS.SnabNet.SourceGenerators
{
    [Generator]
    public class SnabStructGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            IncrementalValuesProvider<TypeDeclarationSyntax> typeDefs = initContext.SyntaxProvider
                .ForAttributeWithMetadataName(
                "CFS.SnabNet.SourceGenerators.SnabStructAttribute",
                (t, ct) => t is ClassDeclarationSyntax || t is StructDeclarationSyntax,
                (ctx, ct) => ctx.TargetNode as TypeDeclarationSyntax
                );

            // generate a class that contains their values as const strings
            initContext.RegisterSourceOutput(typeDefs, (spc, typeDef) =>
            {
                string className = typeDef.Identifier.ValueText;
                spc.AddSource(className, GenerateSourceOutput(typeDef));
            });
        }

        public SourceText GenerateSourceOutput(TypeDeclarationSyntax oldTypeDef)
        {
            MethodDeclarationSyntax methodDef = MethodDeclaration(
                ParseTypeName("IReadOnlyDictionary<string, object?>"),
                "Create").AddModifiers(ParseToken("public"))
                .AddBodyStatements(
                    ParseStatement("Dictionary<string, object?> structData = new();")
                );
            foreach (PropertyDeclarationSyntax propDef in oldTypeDef.ChildNodes()
                .Where(x => x is PropertyDeclarationSyntax)
                .Select(x => (PropertyDeclarationSyntax)x)
                .Where(x => x.AttributeLists.Any(l =>
                    l.Attributes.Any(a => a.Name.ToString() == "SnabField")
                    )))
            {
                AttributeSyntax attribute = propDef.AttributeLists
                    .SelectMany(l => l.Attributes)
                    .Single(a => a.Name.ToString() == "SnabField");
                string propName = attribute.ArgumentList.Arguments[0]
                    .Expression.ToString().Trim('"');
                string typeIdStr = attribute.ArgumentList.Arguments.Count > 1 ?
                    attribute.ArgumentList.Arguments[1].Expression.ToString() : "0x00";

                methodDef = methodDef.AddBodyStatements(
                    ParseStatement(
                        $"structData.Add(\"{propName}\", " +
                        $"new SnabField({typeIdStr}, {propDef.Identifier.ValueText}));"
                        )
                );
            }
            methodDef = methodDef.AddBodyStatements(ParseStatement("return structData;"));

            BaseTypeDeclarationSyntax newTypeDef;
            switch (oldTypeDef)
            {
                case ClassDeclarationSyntax _:
                    newTypeDef = ClassDeclaration(oldTypeDef.Identifier)
                        .AddModifiers(ParseToken("public"), ParseToken("partial"))
                        .AddBaseListTypes(SimpleBaseType(ParseTypeName("ISnabStruct")))
                        .AddMembers(methodDef);
                    break;
                case StructDeclarationSyntax _:
                    newTypeDef = StructDeclaration(oldTypeDef.Identifier)
                        .AddModifiers(ParseToken("public"), ParseToken("partial"))
                        .AddBaseListTypes(SimpleBaseType(ParseTypeName("ISnabStruct")))
                        .AddMembers(methodDef);
                    break;
                default:
                    throw new ArgumentException("Only classes and structs are supported", nameof(oldTypeDef));
            }

            return CompilationUnit()
                .AddUsings(UsingDirective(ParseName("CFS.SnabNet")))
                .AddMembers(
                    NamespaceDeclaration(
                    ((NamespaceDeclarationSyntax)oldTypeDef.Parent).Name
                    ).AddMembers(newTypeDef))
                .NormalizeWhitespace()
                .GetText(Encoding.UTF8);
        }
    }
}
