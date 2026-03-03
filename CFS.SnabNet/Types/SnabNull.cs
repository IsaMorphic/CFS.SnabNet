
namespace CFS.SnabNet.Types
{
    internal class SnabNull : ISnabType
    {
        public HashSet<byte> TypeIds { get; }

        public SnabNull()
        {
            TypeIds = new HashSet<byte> { SnabType.Null };
        }

        public object? ReadFromInstance(SnabReader reader, byte typeId)
        {
            if (typeId != SnabType.Null)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabNull", nameof(typeId));
            return null;
        }

        public void WriteToInstance(SnabWriter instance, byte typeId, object? obj)
        {
            if (typeId != SnabType.Null)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabNull", nameof(typeId));

            if (obj is not null)
                throw new ArgumentException($"Expected null for SnabNull, got {obj.GetType()}", nameof(obj));
        }
    }
}
