
namespace CFS.SnabNet.Types
{
    internal class SnabBuffer : ISnabType<byte[]>
    {
        public HashSet<byte> TypeIds { get; }
        public SnabBuffer()
        {
            TypeIds = new HashSet<byte> { 0x0B };
        }
        public byte[] ReadFromInstance(SnabReader instance, byte typeId)
        {
            if (typeId != 0x0B)
                throw new InvalidDataException($"Invalid typeId {typeId} for SnabBuffer");

            Span<byte> bytesSpan = stackalloc byte[sizeof(uint)];
            if(BitConverter.IsLittleEndian ^ 
                instance.Header.Flags.HasFlag(SnabFlags.BigEndian)) 
            {
                bytesSpan.Reverse();
            }

            instance.BaseStream.ReadExactly(bytesSpan);
            uint length = BitConverter.ToUInt32(bytesSpan);

            byte[] buffer = new byte[length];
            for (int i = 0; i < length; i++) 
            {
                int nextByte = instance.BaseStream.ReadByte();
                if (nextByte < 0) throw new EndOfStreamException();

                buffer[i] = (byte)nextByte;
            }

            return buffer;
        }
    }
}
