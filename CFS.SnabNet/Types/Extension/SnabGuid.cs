namespace CFS.SnabNet.Types.Extension
{
    public class SnabGuid : ISnabType<Guid>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabGuid()
        {
            TypeIds = new HashSet<byte> { SnabType.Guid };
        }

        public Guid ReadFromInstance(SnabReader instance, byte typeId)
        {
            if (TypeIds.Contains(typeId))
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
            if (obj is Guid guid && TypeIds.Contains(typeId))
            {
                Span<byte> bytes = stackalloc byte[16];
                guid.TryWriteBytes(bytes, instance.Flags.HasFlag(SnabFlags.BigEndian), out int _);
                instance.BaseStream.Write(bytes);
            }
            else
            {
                throw new ArgumentException($"Invalid typeId {typeId} for SnabGuid", nameof(typeId));
            }
        }

        public byte GetTypeIdForValue(object? value)
        {
            if (value is Guid)
            {
                return SnabType.Guid;
            }
            else
            {
                return SnabType.None;
            }
        }
    }
}
