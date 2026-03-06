namespace CFS.SnabNet.Tests.Types
{
    internal class ValidCustomType : ISnabType
    {
        public const byte TypeId = 0x80;

        public HashSet<byte> TypeIds { get; }

        public ValidCustomType()
        {
            TypeIds = new HashSet<byte> { TypeId };
        }

        public object? ReadFromInstance(SnabReader instance, byte typeId)
        {
            throw new NotImplementedException();
        }

        public void WriteToInstance(SnabWriter instance, byte typeId, object? obj)
        {
            throw new NotImplementedException();
        }
    }
}
