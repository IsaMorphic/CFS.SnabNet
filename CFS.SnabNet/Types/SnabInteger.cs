namespace CFS.SnabNet.Types
{
    internal class SnabInteger : ISnabType<long>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabInteger()
        {
            TypeIds = new HashSet<byte> { 0x06 };
        }

        public long ReadFromInstance(SnabReader instance, byte typeId)
        {
            if (typeId != 0x06)
                throw new InvalidDataException($"Invalid typeId {typeId} for SnabInteger");

            Span<byte> bytesSpan = stackalloc byte[sizeof(long)];
            instance.BaseStream.ReadExactly(bytesSpan);

            if (BitConverter.IsLittleEndian == 
                instance.Header.Flags.HasFlag(SnabFlags.BigEndian)) 
            {
                bytesSpan.Reverse();
            }

            return BitConverter.ToInt64(bytesSpan);
        }
    }
}
