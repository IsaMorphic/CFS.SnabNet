
namespace CFS.SnabNet.Tests
{
    public class SnabBufferTests
    {
        [Theory]
        [InlineData([new byte[] { }, false])]
        [InlineData([new byte[] { 0x01 }, false])]
        [InlineData([new byte[] { 0x01, 0x02 }, false])]
        [InlineData([new byte[] { 0x01, 0x02, 0x03 }, false])]
        [InlineData([new byte[] { }, true])]
        [InlineData([new byte[] { 0x01 }, true])]
        [InlineData([new byte[] { 0x01, 0x02 }, true])]
        [InlineData([new byte[] { 0x01, 0x02, 0x03 }, true])]
        public void ShouldReadCorrectly(byte[] expectedData, bool isBigEndian) 
        {
            SnabInstance instance = new();
            SnabHeader header = new()
            {
                Flags = isBigEndian ? 
                SnabFlags.BigEndian : SnabFlags.None,
            };

            byte[] lengthBytes = BitConverter.GetBytes((uint)expectedData.Length);
            if (BitConverter.IsLittleEndian == isBigEndian) 
            {
                Array.Reverse(lengthBytes);
            }

            byte[] actualData;
            using (MemoryStream ms = new([.. lengthBytes, .. expectedData]))
            using (SnabReader reader = new(instance, header, ms, false)) 
            {
                ISnabType bufferType = reader.GetTypeById(SnabType.Buffer);
                actualData = (byte[])bufferType.ReadFromInstance(reader, SnabType.Buffer)!;
            }

            Assert.Equal(expectedData, actualData);
        }

        [Theory]
        [InlineData([new byte[] { }, false])]
        [InlineData([new byte[] { 0x01 }, false])]
        [InlineData([new byte[] { 0x01, 0x02 }, false])]
        [InlineData([new byte[] { 0x01, 0x02, 0x03 }, false])]
        [InlineData([new byte[] { }, true])]
        [InlineData([new byte[] { 0x01 }, true])]
        [InlineData([new byte[] { 0x01, 0x02 }, true])]
        [InlineData([new byte[] { 0x01, 0x02, 0x03 }, true])]
        public void ShouldWriteCorrectly(byte[] bufferData, bool isBigEndian) 
        {
            SnabInstance instance = new();

            byte[] lengthBytes = BitConverter.GetBytes((uint)bufferData.Length);
            if (BitConverter.IsLittleEndian == isBigEndian)
            {
                Array.Reverse(lengthBytes);
            }

            byte[] actualData, expectedData = [..lengthBytes, ..bufferData];
            using (MemoryStream ms = new())
            using (SnabWriter writer = new(instance, ms, Stream.Null, isBigEndian ?
                SnabFlags.BigEndian : SnabFlags.None, false))
            {
                ISnabType bufferType = writer.GetTypeById(SnabType.Buffer);
                bufferType.WriteToInstance(writer, SnabType.Buffer, bufferData);
                actualData = ms.ToArray();
            }

            Assert.Equal(expectedData, actualData);
        }
    }
}
