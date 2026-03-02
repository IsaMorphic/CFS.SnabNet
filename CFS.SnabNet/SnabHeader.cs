namespace CFS.SnabNet
{
    public class SnabHeader
    {
        public byte MajorVersion { get; private init; }

        public byte MinorVersion { get; private init; }

        public SnabFlags Flags { get; private init; }

        public uint LangId { get; private init; }

        public uint Checksum { get; private init; }

        public uint Length { get; private init; }

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
                bytesSpan[..1].Reverse();
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
    }
}
