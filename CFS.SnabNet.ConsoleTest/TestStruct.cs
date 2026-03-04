using CFS.SnabNet.SourceGenerators;
using CFS.SnabNet.Values;

namespace CFS.SnabNet.ConsoleTest
{
    [SnabStruct]
    public partial class TestStruct
    {
        [SnabField("int_field")]
        public int IntField { get; }

        [SnabField("array_field")]
        public List<int> ArrayField { get; }

        [SnabField("undefined_field")]
        public object UndefinedField { get; }

        public TestStruct(int intField, List<int> arrayField)
        {
            IntField = intField;
            ArrayField = arrayField;
            UndefinedField = new SnabUndefined();
        }
    }
}
