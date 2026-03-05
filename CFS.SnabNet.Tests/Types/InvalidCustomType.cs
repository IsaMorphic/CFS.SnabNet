namespace CFS.SnabNet.Tests.Types
{
    internal class InvalidCustomType : ISnabType
    {
        public const byte TypeId = SnabType.LastReserved;

        public HashSet<byte> TypeIds { get; }

        public InvalidCustomType()
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
