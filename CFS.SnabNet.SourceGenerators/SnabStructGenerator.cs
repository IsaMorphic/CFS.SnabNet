using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
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
                "CFS.SnabNet.SnabStructAttribute",
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
            MethodDeclarationSyntax dehydrateMethodDef = MethodDeclaration(
                ParseTypeName("IDictionary<string, object?>"), "Dehydrate"
                ).WithExplicitInterfaceSpecifier(
                ExplicitInterfaceSpecifier(ParseName("ISnabStruct"))
                ).AddBodyStatements(
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
                    .SingleOrDefault(a => a.Name.ToString() == "SnabField");
                string propName = attribute?.ArgumentList?.Arguments.Count > 0 ?
                    attribute.ArgumentList?.Arguments[0]
                    .Expression.ToString().Trim('"') : propDef.Identifier.ToString();
                string typeIdStr = attribute?.ArgumentList?.Arguments.Count > 1 ?
                    attribute.ArgumentList?.Arguments[1].Expression.ToString() : "SnabType.None";

                dehydrateMethodDef = dehydrateMethodDef.AddBodyStatements(
                    ParseStatement(
                        $"structData.Add(\"{propName}\", " +
                        $"new SnabField({typeIdStr}, {propDef.Identifier.ValueText}));"
                        )
                );
            }
            dehydrateMethodDef = dehydrateMethodDef.AddBodyStatements(ParseStatement("return structData;"));

            MethodDeclarationSyntax hydrateMethodDef = MethodDeclaration(
                ParseTypeName($"{oldTypeDef.Identifier}?"), "Hydrate"
                ).AddModifiers(ParseToken("public"), ParseToken("static")
                ).AddParameterListParameters(Parameter(Identifier("structData"))
                    .WithType(ParseTypeName("IDictionary<string, object?>?"))
                ).AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(ParseName("NotNullIfNotNull"))
                        .AddArgumentListArguments(
                            AttributeArgument(ParseExpression("nameof(structData)"))
                            ))
                    ).WithTarget(AttributeTargetSpecifier(ParseToken("return")))
                ).AddBodyStatements(
                    ParseStatement("if (structData is null) return null;"),
                    ParseStatement($"{oldTypeDef.Identifier} inst = new();")
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
                    .SingleOrDefault(a => a.Name.ToString() == "SnabField");
                string propName = attribute?.ArgumentList?.Arguments.Count > 0 ?
                    attribute.ArgumentList.Arguments[0]
                    .Expression.ToString().Trim('"') : propDef.Identifier.ToString();
                string typeIdStr = attribute?.ArgumentList?.Arguments.Count > 1 ?
                    attribute.ArgumentList.Arguments[1].Expression.ToString() : "SnabType.None";

                string fallbackExpr;
                switch (typeIdStr)
                {
                    case "SnabType.Struct":
                        fallbackExpr = $"{propDef.Type.ToString().TrimEnd('?')}.Hydrate((IDictionary<string, object?>?){propName})";
                        break;
                    case "SnabType.Array":
                        string elemType = propDef.Type.ToString().TrimEnd('[', ']', '?');
                        fallbackExpr = $"({propDef.Type})({propName} is null ? " +
                            $"null : [..((IList<object?>){propName})" +
                            $".Select(x => (x as IConvertible)?.ToType(typeof({elemType}), null))" +
                            $".Cast<{elemType}>()])";
                        break;
                    default:
                        fallbackExpr = "default";
                        break;
                }

                string propType = propDef.Type.ToString().TrimEnd('?');
                hydrateMethodDef = hydrateMethodDef.AddBodyStatements(
                    IfStatement(ParseExpression($"structData.TryGetValue(\"{propName}\", out object? {propName})"),
                    ParseStatement($"inst.{propDef.Identifier} = " +
                    $"({propType}?)({propName} as IConvertible)?.ToType(" +
                    $"typeof({propType}), null) ?? {fallbackExpr};"
                    )));
            }
            hydrateMethodDef = hydrateMethodDef.AddBodyStatements(ParseStatement("return inst;"));

            BaseTypeDeclarationSyntax newTypeDef;
            switch (oldTypeDef)
            {
                case ClassDeclarationSyntax _:
                    newTypeDef = ClassDeclaration(oldTypeDef.Identifier)
                        .AddModifiers(ParseToken("partial"))
                        .AddBaseListTypes(SimpleBaseType(
                            ParseTypeName($"ISnabStruct<{oldTypeDef.Identifier}>")
                            ))
                        .AddMembers(dehydrateMethodDef, hydrateMethodDef);
                    break;
                case StructDeclarationSyntax _:
                    newTypeDef = StructDeclaration(oldTypeDef.Identifier)
                        .AddModifiers(ParseToken("partial"))
                        .AddBaseListTypes(SimpleBaseType(
                            ParseTypeName($"ISnabStruct<{oldTypeDef.Identifier}>")
                            ))
                        .AddMembers(dehydrateMethodDef, hydrateMethodDef);
                    break;
                default:
                    throw new ArgumentException("Only classes and structs are supported", nameof(oldTypeDef));
            }

            HashSet<string> requiredUsings = new HashSet<string>()
            {
                "System.Diagnostics.CodeAnalysis",
                "CFS.SnabNet",
            };
            return CompilationUnit()
                .AddUsings(requiredUsings.Select(x => UsingDirective(ParseName(x))).ToArray())
                .AddUsings(oldTypeDef.SyntaxTree
                .GetRoot().ChildNodes()
                .Where(x => x is UsingDirectiveSyntax ud &&
                    !requiredUsings.Contains(ud.Name?.ToString()))
                .Cast<UsingDirectiveSyntax>()
                .ToArray())
                .AddMembers(
                    NamespaceDeclaration(
                    ((NamespaceDeclarationSyntax)oldTypeDef.Parent).Name
                    ).AddMembers(newTypeDef))
                .NormalizeWhitespace()
                .GetText(Encoding.UTF8);
        }
    }
}
