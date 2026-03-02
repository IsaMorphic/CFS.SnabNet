
namespace CFS.SnabNet.Types
{
    internal class SnabReal : ISnabType<double>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabReal()
        {
            TypeIds = new HashSet<byte> { 0x05 };
        }

        public double ReadFromInstance(SnabReader instance, byte typeId)
        {
            if (typeId != 0x05)
                throw new InvalidDataException($"Invalid typeId {typeId} for SnabReal");

            Span<byte> bytesSpan = stackalloc byte[sizeof(double)];
            instance.BaseStream.ReadExactly(bytesSpan);

            if (BitConverter.IsLittleEndian ==
                instance.Header.Flags.HasFlag(SnabFlags.BigEndian))
            {
                bytesSpan.Reverse();
            }

            return BitConverter.ToDouble(bytesSpan);
        }
    }
}
