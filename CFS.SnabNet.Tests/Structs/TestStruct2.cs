namespace CFS.SnabNet.Tests.Structs
{
    [SnabStruct]
    internal partial class TestStruct2
    {
        [SnabField("string_field", SnabType.StringW)]
        public string? StringField { get; set; }

        [SnabField("array_field", SnabType.Array)]
        public int[]? ArrayField { get; set; }
    }
}
