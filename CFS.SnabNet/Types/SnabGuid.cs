namespace CFS.SnabNet.Types
{
    public class SnabGuid : ISnabType<Guid>
    {
        private const byte EXT_TYPE_ID = 0x80;

        public HashSet<byte> TypeIds { get; }

        public SnabGuid()
        {
            TypeIds = new HashSet<byte> { SnabType.Guid, EXT_TYPE_ID };
        }

        public Guid ReadFromInstance(SnabReader instance, byte typeId)
        {
            if ((typeId == EXT_TYPE_ID && instance.Version is (1, 0)) ||
                (typeId == SnabType.Guid && instance.Version is (>= 1, >= 1)))
            {
                Span<byte> bytes = stackalloc byte[16];
                instance.BaseStream.ReadExactly(bytes);
                return new Guid(bytes, instance.Flags.HasFlag(SnabFlags.BigEndian));
            }
            else
            {
                throw new ArgumentException($"Invalid typeId {typeId} for SnabGuid", nameof(typeId));
            }
        }

        public void WriteToInstance(SnabWriter instance, byte typeId, object? obj)
        {
            if (obj is Guid guid &&
                ((typeId == EXT_TYPE_ID && instance.Version is (1, 0)) ||
                (typeId == SnabType.Guid && instance.Version is (>= 1, >= 1))))
            {
                Span<byte> bytes = stackalloc byte[16];
                guid.TryWriteBytes(bytes, instance.Flags.HasFlag(SnabFlags.BigEndian), out int _);
                instance.BaseStream.Write(bytes);
            }
            else if (obj is not Guid)
            {
                throw new ArgumentException($"Invalid value type '{obj?.GetType().Name ?? "null"}' for SnabGuid", nameof(obj));
            }
            else
            {
                throw new ArgumentException($"Invalid typeId {typeId} for SnabGuid", nameof(typeId));
            }
        }
    }
}
