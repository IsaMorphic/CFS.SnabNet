
namespace CFS.SnabNet.Types
{
    internal class SnabUndefined : ISnabType<Values.SnabUndefined>
    {
        public HashSet<byte> TypeIds { get; }
        public SnabUndefined()
        {
            TypeIds = new HashSet<byte> { 0x09 };
        }
        public Values.SnabUndefined ReadFromInstance(SnabReader reader, byte typeId)
        {
            if (typeId != 0x09)
                throw new InvalidDataException($"Invalid typeId {typeId} for SnabUndefined");
            return new Values.SnabUndefined();
        }
    }
}
