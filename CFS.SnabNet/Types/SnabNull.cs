
namespace CFS.SnabNet.Types
{
    internal class SnabNull : ISnabType
    {
        public HashSet<byte> TypeIds { get; }

        public SnabNull()
        {
            TypeIds = new HashSet<byte> { 0x0A };
        }

        public object? ReadFromInstance(SnabReader reader, byte typeId)
        {
            if (typeId != 0x0A)
                throw new InvalidDataException($"Invalid typeId {typeId} for SnabNull");
            return null;
        }
    }
}
