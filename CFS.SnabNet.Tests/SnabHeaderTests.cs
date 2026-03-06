namespace CFS.SnabNet.Tests
{
    public class SnabHeaderTests
    {
        [Fact]
        public void ShouldReadHeaderFromStream()
        {
            // Arrange
            byte[] headerBytes = new byte[]
            {
                0x01, 0x00, // MajorVersion, MinorVersion
                0x03, 0x00, // Flags (SnabFlags.BigEndian | SnabFlags.Extended)
                0x43, 0x53, 0x00, 0x00, // LangId ("CS\x00\x00")
                0x78, 0x56, 0x34, 0x12, // Checksum (0x12345678)
                0xEF, 0xCD, 0xAB, 0x90 // Length (0x90ABCDEF)
            };
            using MemoryStream stream = new(headerBytes);
            // Act
            SnabHeader header = SnabHeader.ReadFromStream(stream);
            // Assert
            Assert.Equal(1, header.MajorVersion);
            Assert.Equal(0, header.MinorVersion);
            Assert.Equal(SnabFlags.BigEndian | SnabFlags.Extended, header.Flags);
            Assert.Equal(0x00005343u, header.LangId);
            Assert.Equal(0x12345678u, header.Checksum);
            Assert.Equal(0x90ABCDEFu, header.Length);
        }

        [Fact]
        public void ShouldWriteHeaderToStream()
        {
            // Arrange
            SnabHeader header = new()
            {
                MajorVersion = 1,
                MinorVersion = 0,
                Flags = SnabFlags.BigEndian | SnabFlags.Extended,
                LangId = 0x00005343u, // "CS\x00\x00"
                Checksum = 0x12345678u,
                Length = 0x90ABCDEFu
            };
            using MemoryStream stream = new();
            // Act
            header.WriteToStream(stream);
            byte[] writtenBytes = stream.ToArray();
            // Assert
            byte[] expectedBytes = new byte[]
            {
                0x01, 0x00, // MajorVersion, MinorVersion
                0x03, 0x00, // Flags (SnabFlags.BigEndian | SnabFlags.Extended)
                0x43, 0x53, 0x00, 0x00, // LangId ("CS\x00\x00")
                0x78, 0x56, 0x34, 0x12, // Checksum (0x12345678)
                0xEF, 0xCD, 0xAB, 0x90 // Length (0x90ABCDEF)
            };
            Assert.Equal(expectedBytes, writtenBytes);
        }
    }
}
