
namespace CFS.SnabNet.Tests
{
    public class SnabRealTests
    {
        [Theory]
        [InlineData([1, false])]
        [InlineData([2.5, false])]
        [InlineData([3.1415, false])]
        [InlineData([1, true])]
        [InlineData([2.5, true])]
        [InlineData([3.1415, true])]
        public void DoesReadCorrectly(double x, bool isBigEndian) 
        {
            SnabInstance instance = new SnabInstance();
            SnabHeader header = new SnabHeader()
            {
                Flags = isBigEndian ? 
                SnabFlags.BigEndian : SnabFlags.None,
            };

            byte[] realBytes = BitConverter.GetBytes(x);
            if (BitConverter.IsLittleEndian == isBigEndian) 
            {
                realBytes.AsSpan().Reverse();
            }

            double y;
            using (MemoryStream ms = new(realBytes))
            using (SnabReader reader = new(instance, header, ms, false))
            {
                ISnabType realType = reader.GetTypeById(SnabType.Real);
                y = (double)realType.ReadFromInstance(reader, SnabType.Real)!;
            }

            Assert.Equal(x, y);
        }

        [Theory]
        [InlineData([1, false])]
        [InlineData([2.5, false])]
        [InlineData([3.1415, false])]
        [InlineData([1, true])]
        [InlineData([2.5, true])]
        [InlineData([3.1415, true])]
        public void DoesWriteCorrectly(double x, bool isBigEndian)
        {
            SnabInstance instance = new();
            byte[] expectedBytes = BitConverter.GetBytes(x);
            if (BitConverter.IsLittleEndian == isBigEndian) 
            {
                expectedBytes.AsSpan().Reverse();
            }

            byte[] actualBytes;
            using (MemoryStream ms = new())
            using (SnabWriter writer = new(instance, ms, Stream.Null, isBigEndian ? 
                SnabFlags.BigEndian : SnabFlags.None, false))
            {
                ISnabType realType = writer.GetTypeById(SnabType.Real);
                realType.WriteToInstance(writer, SnabType.Real, x);
                actualBytes = ms.ToArray();
            }

            Assert.Equal(expectedBytes, actualBytes);
        }
    }
}