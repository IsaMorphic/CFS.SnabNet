
namespace CFS.SnabNet.Tests
{
    public class SnabNullTests
    {
        [Fact]
        public void ShouldReadCorrectly() 
        {
            SnabInstance instance = new();
            SnabHeader header = new();

            object? actualValue;
            using (SnabReader reader = new(instance, header, Stream.Null, false)) 
            {
                ISnabType nullType = reader.GetTypeById(SnabType.Null);
                actualValue = nullType.ReadFromInstance(reader, SnabType.Null);
            }

            Assert.Null(actualValue);
        }

        [Fact]
        public void ShouldNotThrowOnValidWrite() 
        {
            SnabInstance instance = new();

            Exception? caughtEx;
            try
            {
                using (SnabWriter writer = new(instance, null, Stream.Null, SnabFlags.None, false))
                {
                    ISnabType nullType = writer.GetTypeById(SnabType.Null);
                    nullType.WriteToInstance(writer, SnabType.Null, null);
                }

                caughtEx = null;
            }
            catch (Exception ex) 
            {
                caughtEx = ex;
            }

            Assert.Null(caughtEx);
        }
    }
}
