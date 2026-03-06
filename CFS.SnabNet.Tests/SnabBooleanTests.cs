
namespace CFS.SnabNet.Tests
{
    public class SnabBooleanTests
    {
        [Theory]
        [InlineData(false, SnabType.False)]
        [InlineData(true, SnabType.True)]
        public void ShouldReadCorrectly(bool expectedValue, byte typeId)
        {
            SnabInstance instance = new();
            SnabHeader header = new();

            bool actualValue;
            using (SnabReader reader = new(instance, header, Stream.Null, false))
            {
                ISnabType booleanType = reader.GetTypeById(typeId);
                actualValue = (bool)booleanType.ReadFromInstance(reader, typeId)!;
            }

            Assert.Equal(expectedValue, actualValue);
        }

        [Theory]
        [InlineData(false, SnabType.False)]
        [InlineData(true, SnabType.True)]
        public void ShouldNotThrowOnValidWrite(bool value, byte typeId)
        {
            SnabInstance instance = new();

            Exception? caughtEx;
            try
            {
                using (SnabWriter writer = new(instance, null, Stream.Null, SnabFlags.None, false))
                {
                    ISnabType booleanType = writer.GetTypeById(typeId);
                    booleanType.WriteToInstance(writer, typeId, value);
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
