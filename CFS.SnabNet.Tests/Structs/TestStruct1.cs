namespace CFS.SnabNet.Tests.Structs
{
    [SnabStruct]
    internal partial class TestStruct1
    {
        [SnabField("int_field", SnabType.Integer)]
        public int IntField { get; set; }

        [SnabField("real_field", SnabType.Real)]
        public float RealField { get; set; }

        [SnabField("struct_field", SnabType.Struct)]
        public TestStruct2? StructField { get; set; }
    }
}
