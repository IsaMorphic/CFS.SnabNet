namespace CFS.SnabNet.Tests.Structs
{
    [SnabStruct]
    internal partial class TestStruct1
    {
        [SnabStruct]
        public partial class NestedStruct
        {
            [SnabField("nested_string_field", SnabType.StringW)]
            public string? NestedStringField { get; set; }
        }

        [SnabField("int_field", SnabType.Integer)]
        public int IntField { get; set; }

        [SnabField("real_field", SnabType.Real)]
        public float RealField { get; set; }

        [SnabField("struct_field", SnabType.Struct)]
        public TestStruct2? StructField { get; set; }

        [SnabField("nested_struct_field", SnabType.Struct)]
        public NestedStruct? NestedStructField { get; set; }

        [SnabField("buffer_field", SnabType.Buffer)]
        public byte[]? BufferField { get; set; }
    }
}
