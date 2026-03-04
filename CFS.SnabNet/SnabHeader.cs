namespace CFS.SnabNet
{
    public class SnabHeader
    {
        public const int HEADER_SIZE = 16;

        public byte MajorVersion { get; internal set; }

        public byte MinorVersion { get; internal set; }

        public SnabFlags Flags { get; internal set; }

        public uint LangId { get; internal set; }

        public uint Checksum { get; internal set; }

        public uint Length { get; internal set; }

        internal static SnabHeader ReadFromStream(Stream stream)
        {
            Span<byte> bytesSpan = stackalloc byte[sizeof(uint)];
            stream.ReadExactly(bytesSpan[..2]);

            byte majorVersion = bytesSpan[0];
            byte minorVersion = bytesSpan[1];

            SnabFlags flags;
            stream.ReadExactly(bytesSpan[..2]);
            if (!BitConverter.IsLittleEndian)
            {
                bytesSpan[..2].Reverse();
            }
            flags = (SnabFlags)BitConverter.ToUInt16(bytesSpan[..2]);

            uint langIndicator;
            stream.ReadExactly(bytesSpan);
            if (!BitConverter.IsLittleEndian)
            {
                bytesSpan.Reverse();
            }
            langIndicator = BitConverter.ToUInt32(bytesSpan);

            uint checksum;
            stream.ReadExactly(bytesSpan);
            if (!BitConverter.IsLittleEndian)
            {
                bytesSpan.Reverse();
            }
            checksum = BitConverter.ToUInt32(bytesSpan);

            uint length;
            stream.ReadExactly(bytesSpan);
            if (!BitConverter.IsLittleEndian)
            {
                bytesSpan.Reverse();
            }
            length = BitConverter.ToUInt32(bytesSpan);

            return new()
            {
                MajorVersion = majorVersion,
                MinorVersion = minorVersion,
                Flags = flags,
                LangId = langIndicator,
                Checksum = checksum,
                Length = length,
            };
        }

        internal void WriteToStream(Stream stream)
        {
            Span<byte> bytesSpan = stackalloc byte[sizeof(uint)];

            stream.WriteByte(MajorVersion);
            stream.WriteByte(MinorVersion);

            BitConverter.TryWriteBytes(bytesSpan, (ushort)Flags);
            if (!BitConverter.IsLittleEndian)
            {
                bytesSpan[..2].Reverse();
            }
            stream.Write(bytesSpan[..2]);

            BitConverter.TryWriteBytes(bytesSpan, LangId);
            if (!BitConverter.IsLittleEndian) 
            {
                bytesSpan.Reverse();
            }
            stream.Write(bytesSpan);

            BitConverter.TryWriteBytes(bytesSpan, Checksum);
            if (!BitConverter.IsLittleEndian)
            {
                bytesSpan.Reverse();
            }
            stream.Write(bytesSpan);

            BitConverter.TryWriteBytes(bytesSpan, Length);
            if (!BitConverter.IsLittleEndian)
            {
                bytesSpan.Reverse();
            }
            stream.Write(bytesSpan);
        }
    }
}
