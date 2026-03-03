
namespace CFS.SnabNet.Types
{
    internal class SnabUndefined : ISnabType<Values.SnabUndefined>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabUndefined()
        {
            TypeIds = new HashSet<byte> { SnabType.Undefined };
        }

        public Values.SnabUndefined ReadFromInstance(SnabReader reader, byte typeId)
        {
            if (typeId != SnabType.Undefined)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabUndefined", nameof(typeId));
            return new Values.SnabUndefined();
        }

        public void WriteToInstance(SnabWriter instance, byte typeId, object? obj)
        {
            if (typeId != SnabType.Undefined)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabUndefined", nameof(typeId));

            if (obj is not Values.SnabUndefined)
                throw new ArgumentException("Value must be of type SnabUndefined", nameof(obj));
        }
    }
}
