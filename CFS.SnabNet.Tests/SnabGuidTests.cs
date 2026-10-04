
namespace CFS.SnabNet.Tests
{
    public class SnabGuidTests
    {
        [Theory]
        [InlineData([false])]
        [InlineData([true])]
        public void ShouldReadCorrectly(bool isBigEndian)
        {
            SnabInstance instance = new SnabInstance(includeExtTypes: true);
            SnabHeader header = new SnabHeader()
            {
                Flags = isBigEndian ?
                SnabFlags.BigEndian : SnabFlags.None,
            };

            Guid n = Guid.NewGuid();
            byte[] guidBytes = new byte[16];
            n.TryWriteBytes(guidBytes, isBigEndian, out int _);

            Guid m;
            using (MemoryStream ms = new(guidBytes))
            using (SnabReader reader = new(instance, header, ms, false))
            {
                ISnabType integerType = reader.GetTypeById(SnabType.Guid);
                m = (Guid)integerType.ReadFromInstance(reader, SnabType.Guid)!;
            }

            Assert.Equal(n, m);
        }

        [Theory]
        [InlineData([false])]
        [InlineData([true])]
        public void ShouldWriteCorrectly(bool isBigEndian)
        {
            SnabInstance instance = new SnabInstance(includeExtTypes: true);
            Guid n = Guid.NewGuid();

            byte[] expectedBytes = new byte[16];
            n.TryWriteBytes(expectedBytes, isBigEndian, out int _);

            byte[] actualBytes;
            using (MemoryStream ms = new())
            using (SnabWriter writer = new(instance, ms, Stream.Null, isBigEndian ?
                SnabFlags.BigEndian : SnabFlags.None, false))
            {
                ISnabType guidType = writer.GetTypeById(SnabType.Guid);
                guidType.WriteToInstance(writer, SnabType.Guid, n);
                actualBytes = ms.ToArray();
            }

            Assert.Equal(expectedBytes, actualBytes);
        }
    }
}