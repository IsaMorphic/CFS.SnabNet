using CFS.SnabNet.Wrappers;
using System.IO.Hashing;

namespace CFS.SnabNet.Tests
{
    public class Crc32StreamTests
    {
        [Theory]
        [InlineData([new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }])]
        [InlineData([new byte[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }])]
        [InlineData([new byte[] { 1, 10, 2, 12, 3, 13, 4, 14, 5, 15 }])]
        public void ShouldCalculateHashCorrectly(byte[] streamData) 
        {
            uint actualHash, expectedHash = Crc32.HashToUInt32(streamData);
            using (MemoryStream ms = new(streamData))
            using (Crc32Stream stream = new(ms))
            {
                stream.CopyTo(Stream.Null);
                actualHash = stream.Crc32Value;
            }

            Assert.Equal(expectedHash, actualHash);
        }
    }
}
