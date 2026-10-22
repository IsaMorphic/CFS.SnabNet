
using QuadrupleLib;
using System.Runtime.InteropServices;

namespace CFS.SnabNet.Tests
{
    public class SnabBigRealTests
    {
        [Theory]
        [InlineData([1, false])]
        [InlineData([2.5, false])]
        [InlineData([3.1415, false])]
        [InlineData([1, true])]
        [InlineData([2.5, true])]
        [InlineData([3.1415, true])]
        public void DoesReadCorrectly(Float128 x, bool isBigEndian) 
        {
            SnabInstance instance = new SnabInstance(includeExtTypes: true);
            SnabHeader header = new SnabHeader()
            {
                Flags = isBigEndian ? 
                SnabFlags.BigEndian : SnabFlags.None,
            };

            byte[] realBytes = [.. MemoryMarshal.AsBytes([x])];
            if (BitConverter.IsLittleEndian == isBigEndian) 
            {
                Array.Reverse(realBytes);
            }

            Float128 y;
            using (MemoryStream ms = new(realBytes))
            using (SnabReader reader = new(instance, header, ms, false))
            {
                ISnabType realType = reader.GetTypeById(SnabType.BigReal);
                y = (Float128)realType.ReadFromInstance(reader, SnabType.BigReal)!;
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
        public void DoesWriteCorrectly(Float128 x, bool isBigEndian)
        {
            SnabInstance instance = new(includeExtTypes: true);
            byte[] expectedBytes = [.. MemoryMarshal.AsBytes([x])];
            if (BitConverter.IsLittleEndian == isBigEndian) 
            {
                Array.Reverse(expectedBytes);
            }

            byte[] actualBytes;
            using (MemoryStream ms = new())
            using (SnabWriter writer = new(instance, ms, Stream.Null, isBigEndian ? 
                SnabFlags.BigEndian : SnabFlags.None, false))
            {
                ISnabType realType = writer.GetTypeById(SnabType.BigReal);
                realType.WriteToInstance(writer, SnabType.BigReal, x);
                actualBytes = ms.ToArray();
            }

            Assert.Equal(expectedBytes, actualBytes);
        }
    }
}