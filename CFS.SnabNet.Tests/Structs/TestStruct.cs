using CFS.SnabNet.SourceGenerators;

namespace CFS.SnabNet.Tests.Structs
{
    [SnabStruct]
    public partial class TestStruct
    {
        [SnabField("int_field")]
        public int IntField { get; set; }

        [SnabField("real_field")]
        public float RealField { get; set; }

        [SnabField("array_field")]
        public int[]? ArrayField { get; set; }
    }
}
