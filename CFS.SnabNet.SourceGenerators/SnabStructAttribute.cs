using System;

namespace CFS.SnabNet.SourceGenerators
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class SnabStructAttribute : Attribute
    {
        public SnabStructAttribute() { }
    }
}
