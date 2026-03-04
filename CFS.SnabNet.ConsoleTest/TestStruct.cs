using CFS.SnabNet.SourceGenerators;

namespace CFS.SnabNet.ConsoleTest
{
    [SnabStruct]
    public partial class TestStruct
    {
        [SnabField("int_field")]
        public int IntField { get; }

        [SnabField("array_field")]
        public int[] ArrayField { get; }

        public TestStruct(int intField, int[] arrayField)
        {
            IntField = intField;
            ArrayField = arrayField;
        }
    }
}
