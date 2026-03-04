
namespace CFS.SnabNet.Types
{
    internal class SnabBuffer : ISnabType<byte[]>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabBuffer()
        {
            TypeIds = new HashSet<byte> { SnabType.Buffer };
        }

        public byte[] ReadFromInstance(SnabReader instance, byte typeId)
        {
            if (typeId != SnabType.Buffer)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabBuffer", nameof(typeId));

            Span<byte> bytesSpan = stackalloc byte[sizeof(uint)];
            instance.BaseStream.ReadExactly(bytesSpan);
            if (BitConverter.IsLittleEndian ==
                instance.Info.Flags.HasFlag(SnabFlags.BigEndian))
            {
                bytesSpan.Reverse();
            }
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

        public void WriteToInstance(SnabWriter instance, byte typeId, object? obj)
        {
            if (typeId != SnabType.Buffer)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabBuffer", nameof(typeId));

            switch (obj)
            {
                case byte[] buffer:
                    Span<byte> bytesSpan = stackalloc byte[sizeof(int)];
                    BitConverter.TryWriteBytes(bytesSpan, buffer.Length);
                    if (BitConverter.IsLittleEndian ==
                        instance.Info.Flags.HasFlag(SnabFlags.BigEndian))
                    {
                        bytesSpan.Reverse();
                    }
                    instance.BaseStream.Write(bytesSpan);
                    instance.BaseStream.Write(buffer, 0, buffer.Length);
                    break;
                default:
                    throw new ArgumentException($"Object of type '{obj?.GetType().FullName ?? "null"}' cannot be written as SnabBuffer", nameof(obj));
            }
        }
    }
}
