using System;

namespace CFS.SnabNet.SourceGenerators
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SnabFieldAttribute : Attribute
    {
        public string Name { get; }

        public new byte TypeId { get; }

        public SnabFieldAttribute() { }

        public SnabFieldAttribute(string name) 
        {
            Name = name;
            TypeId = 0;
        }

        public SnabFieldAttribute(string name, byte typeId)
        {
            Name = name;
            TypeId = typeId;
        }
    }
}
