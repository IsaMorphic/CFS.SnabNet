namespace CFS.SnabNet.Types.Extensions
{
    public class SnabGuid : ISnabType<Guid>
    {   
        public HashSet<byte> TypeIds { get; } = [SnabType.Guid];

        public Guid ReadFromInstance(SnabReader instance, byte typeId)
        {
            Span<byte> bytes = stackalloc byte[16];
            instance.BaseStream.ReadExactly(bytes);
            return new Guid(bytes, instance.Flags.HasFlag(SnabFlags.BigEndian));
        }

        public void WriteToInstance(SnabWriter instance, byte typeId, object? obj)
        {
            Span<byte> bytes = stackalloc byte[16];
            ((Guid)obj!).TryWriteBytes(bytes, instance.Flags.HasFlag(SnabFlags.BigEndian), out int _);
            instance.BaseStream.Write(bytes);
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
