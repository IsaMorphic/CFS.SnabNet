
namespace CFS.SnabNet.Tests
{
    public class SnabIntegerTests
    {
        [Theory]
        [InlineData([1, false])]
        [InlineData([2, false])]
        [InlineData([3, false])]
        [InlineData([1, true])]
        [InlineData([2, true])]
        [InlineData([3, true])]
        public void ShouldReadCorrectly(long n, bool isBigEndian) 
        {
            SnabInstance instance = new SnabInstance();
            SnabHeader header = new SnabHeader()
            {
                Flags = isBigEndian ? 
                SnabFlags.BigEndian : SnabFlags.None,
            };

            byte[] integerBytes = BitConverter.GetBytes(n);
            if (BitConverter.IsLittleEndian == isBigEndian) 
            {
                Array.Reverse(integerBytes);
            }

            long m;
            using (MemoryStream ms = new(integerBytes))
            using (SnabReader reader = new(instance, header, ms, false))
            {
                ISnabType integerType = reader.GetTypeById(SnabType.Integer);
                m = (long)integerType.ReadFromInstance(reader, SnabType.Integer)!;
            }

            Assert.Equal(n, m);
        }

        [Theory]
        [InlineData([1, false])]
        [InlineData([2, false])]
        [InlineData([3, false])]
        [InlineData([1, true])]
        [InlineData([2, true])]
        [InlineData([3, true])]
        public void ShouldWriteCorrectly(long n, bool isBigEndian)
        {
            SnabInstance instance = new();
            byte[] expectedBytes = BitConverter.GetBytes(n);
            if (BitConverter.IsLittleEndian == isBigEndian) 
            {
                Array.Reverse(expectedBytes);
            }

            byte[] actualBytes;
            using (MemoryStream ms = new())
            using (SnabWriter writer = new(instance, ms, Stream.Null, isBigEndian ? 
                SnabFlags.BigEndian : SnabFlags.None, false))
            {
                ISnabType integerType = writer.GetTypeById(SnabType.Integer);
                integerType.WriteToInstance(writer, SnabType.Integer, n);
                actualBytes = ms.ToArray();
            }

            Assert.Equal(expectedBytes, actualBytes);
        }
    }
}