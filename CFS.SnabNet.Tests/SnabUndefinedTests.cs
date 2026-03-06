
using CFS.SnabNet.Values;

namespace CFS.SnabNet.Tests
{
    public class SnabUndefinedTests
    {
        [Fact]
        public void ShouldReadCorrectly() 
        {
            SnabInstance instance = new();
            SnabHeader header = new();

            object? actualValue;
            using (SnabReader reader = new(instance, header, Stream.Null, false)) 
            {
                ISnabType undefinedType = reader.GetTypeById(SnabType.Undefined);
                actualValue = undefinedType.ReadFromInstance(reader, SnabType.Undefined);
            }

            Assert.IsType<SnabUndefined>(actualValue);
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
                    ISnabType undefinedType = writer.GetTypeById(SnabType.Undefined);
                    undefinedType.WriteToInstance(writer, SnabType.Undefined, new SnabUndefined());
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
