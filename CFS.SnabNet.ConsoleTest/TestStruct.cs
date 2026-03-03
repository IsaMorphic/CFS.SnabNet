using CFS.SnabNet.SourceGenerators;

namespace CFS.SnabNet.ConsoleTest
{
    [SnabStruct]
    public partial class TestStruct
    {
        [SnabField("int_field", SnabType.Real)]
        public int IntField { get; set; }
    }
}
