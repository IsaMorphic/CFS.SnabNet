using System.Text;

namespace CFS.SnabNet.Tests
{
    public class SnabStringTests
    {
        [Theory]
        [InlineData("Hello world!")]
        [InlineData("Testing 123")]
        [InlineData("")]
        public void ShouldReadCorrectlyUTF8(string expectedStr) 
        {
            SnabInstance instance = new();
            SnabHeader header = new SnabHeader();

            byte[] strBytes = Encoding.UTF8.GetBytes([..expectedStr.ToCharArray(), '\x00']);

            string actualStr;
            using (MemoryStream ms = new(strBytes))
            using (SnabReader reader = new(instance, header, ms, false))
            {
                ISnabType stringType = reader.GetTypeById(SnabType.String);
                actualStr = (string)stringType.ReadFromInstance(reader, SnabType.String)!;
            }

            Assert.Equal(expectedStr, actualStr);
        }

        [Theory]
        [InlineData("Hello world!", false)]
        [InlineData("Testing 123", false)]
        [InlineData("", false)]
        [InlineData("Hello world!", true)]
        [InlineData("Testing 123", true)]
        [InlineData("", true)]
        public void ShouldReadCorrectlyUnicode(string expectedStr, bool isBigEndian)
        {
            SnabInstance instance = new();
            SnabHeader header = new SnabHeader()
            {
                Flags = isBigEndian ? 
                SnabFlags.BigEndian : SnabFlags.None,
            };

            Encoding encoding = isBigEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;
            byte[] strBytes = encoding.GetBytes([.. expectedStr.ToCharArray(), '\x00']);

            string actualStr;
            using (MemoryStream ms = new(strBytes))
            using (SnabReader reader = new(instance, header, ms, false))
            {
                ISnabType stringType = reader.GetTypeById(SnabType.StringW);
                actualStr = (string)stringType.ReadFromInstance(reader, SnabType.StringW)!;
            }

            Assert.Equal(expectedStr, actualStr);
        }

        [Theory]
        [InlineData("Hello world!")]
        [InlineData("Testing 123")]
        [InlineData("")]
        public void ShouldWriteCorrectlyUTF8(string str) 
        {
            SnabInstance instance = new();

            byte[] actualBytes, expectedBytes =
                Encoding.UTF8.GetBytes([..str.ToCharArray(), '\x00']);
            using (MemoryStream ms = new())
            using (SnabWriter writer = new(instance, ms, Stream.Null, SnabFlags.None, false))
            {
                ISnabType stringType = writer.GetTypeById(SnabType.String);
                stringType.WriteToInstance(writer, SnabType.String, str);
                actualBytes = ms.ToArray();
            }

            Assert.Equal(expectedBytes, actualBytes);
        }

        [Theory]
        [InlineData("Hello world!", false)]
        [InlineData("Testing 123", false)]
        [InlineData("", false)]
        [InlineData("Hello world!", true)]
        [InlineData("Testing 123", true)]
        [InlineData("", true)]
        public void ShouldWriteCorrectlyUnicode(string str, bool isBigEndian)
        {
            SnabInstance instance = new();
            Encoding encoding = isBigEndian ? 
                Encoding.BigEndianUnicode : Encoding.Unicode;

            byte[] actualBytes, expectedBytes =
                encoding.GetBytes([.. str.ToCharArray(), '\x00']);
            using (MemoryStream ms = new())
            using (SnabWriter writer = new(instance, ms, Stream.Null, isBigEndian ? 
                SnabFlags.BigEndian : SnabFlags.None, false))
            {
                ISnabType stringType = writer.GetTypeById(SnabType.StringW);
                stringType.WriteToInstance(writer, SnabType.StringW, str);
                actualBytes = ms.ToArray();
            }

            Assert.Equal(expectedBytes, actualBytes);
        }
    }
}
