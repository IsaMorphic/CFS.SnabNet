using CFS.SnabNet.Tests.Structs;
using CFS.SnabNet.Values;
using System.Collections;
using System.Dynamic;

namespace CFS.SnabNet.Tests
{
    public class SnabStructTests
    {
        private void ShouldResolveTypeCorrectly<T>()
            where T : new()
        {
            SnabInstance instance = new();
            byte actualTypeId = instance.GetTypeIdByValue(new T());
            Assert.Equal(SnabType.Struct, actualTypeId);
        }

        [Fact]
        public void ShouldResolveTypeCorrectly_Dictionary() 
        {
            ShouldResolveTypeCorrectly<Dictionary<string, object?>>();
        }

        [Fact]
        public void ShouldResolveTypeCorrectly_ExpandoObject()
        {
            ShouldResolveTypeCorrectly<ExpandoObject>();
        }

        [Fact]
        public void ShouldResolveTypeCorrectly_TestStruct()
        {
            ShouldResolveTypeCorrectly<TestStruct>();
        }

        [Fact]
        public void RoundtripIsCorrect_Dictionary()
        {
            Dictionary<string, object?> expectedObj = new()
            {
                { "array_field", new long[] { 1, 2, 3 } },
                { "string_field", "Hello world!" },
                { "real_field", 3.1415 },
                { "int_field", 42L },
                { "false_field", false },
                { "true_field", true },
                { "undefined_field", new SnabUndefined() },
                { "null_field", null },
                { "buffer_field", new byte[] { 1, 2, 3 } },
            };

            SnabInstance instance = new();

            dynamic actualObj;
            using (MemoryStream ms = new())
            {
                using (SnabWriter writer = instance.CreateWriter(ms, SnabFlags.None, true))
                {
                    writer.Serialize(expectedObj);
                }

                ms.Position = 0;
                using (SnabReader reader = instance.CreateReader(ms, true))
                {
                    actualObj = reader.Deserialize();
                }
            }

            Assert.Equal((IEnumerable)expectedObj["array_field"]!, (IEnumerable)actualObj.array_field);
            Assert.Equal((string)expectedObj["string_field"]!, (string)actualObj.string_field);
            Assert.Equal((double)expectedObj["real_field"]!, actualObj.real_field);
            Assert.Equal((long)expectedObj["int_field"]!, actualObj.int_field);
            Assert.Equal((bool)expectedObj["false_field"]!, actualObj.false_field);
            Assert.Equal((bool)expectedObj["true_field"]!, actualObj.true_field);
            Assert.Equal((SnabUndefined)expectedObj["undefined_field"]!, actualObj.undefined_field);
            Assert.Equal(expectedObj["null_field"], (object?)actualObj.null_field);
            Assert.Equal((byte[])expectedObj["buffer_field"]!, (byte[])actualObj.buffer_field);
        }

        [Fact]
        public void RoundtripIsCorrect_ExpandoObject()
        {
            dynamic expectedObj = new ExpandoObject();
            expectedObj.array_field = new long[] { 1, 2, 3 };
            expectedObj.string_field = "Hello world!";
            expectedObj.real_field = 3.1415;
            expectedObj.int_field = 42;
            expectedObj.false_field = false;
            expectedObj.true_field = true;
            expectedObj.undefined_field = new SnabUndefined();
            expectedObj.null_field = null;
            expectedObj.buffer_field = new byte[] { 1, 2, 3 };

            SnabInstance instance = new();

            dynamic actualObj;
            using (MemoryStream ms = new())
            {
                using (SnabWriter writer = instance.CreateWriter(ms, SnabFlags.None, true))
                {
                    writer.Serialize(expectedObj);
                }

                ms.Position = 0;
                using (SnabReader reader = instance.CreateReader(ms, true))
                {
                    actualObj = reader.Deserialize();
                }
            }

            Assert.Equal((IEnumerable)expectedObj.array_field, (IEnumerable)actualObj.array_field);
            Assert.Equal((string)expectedObj.string_field, (string)actualObj.string_field);
            Assert.Equal(expectedObj.real_field, actualObj.real_field);
            Assert.Equal(expectedObj.int_field, actualObj.int_field);
            Assert.Equal(expectedObj.false_field, actualObj.false_field);
            Assert.Equal(expectedObj.true_field, actualObj.true_field);
            Assert.Equal(expectedObj.undefined_field, actualObj.undefined_field);
            Assert.Equal((object?)expectedObj.null_field, (object?)actualObj.null_field);
            Assert.Equal((byte[])expectedObj.buffer_field, (byte[])actualObj.buffer_field);
        }

        [Fact]
        public void RoundtripIsCorrect_TestStruct()
        {
            TestStruct expectedObj = new TestStruct()
            {
                IntField = 42,
                RealField = 3.1415f,
                ArrayField = [ 1, 2, 3 ],
            };

            SnabInstance instance = new();

            dynamic actualObj;
            using (MemoryStream ms = new())
            {
                using (SnabWriter writer = instance.CreateWriter(ms, SnabFlags.None, true))
                {
                    writer.Serialize(expectedObj);
                }

                ms.Position = 0;
                using (SnabReader reader = instance.CreateReader(ms, true))
                {
                    actualObj = reader.Deserialize();
                }
            }

            Assert.Equal((long)expectedObj.IntField, actualObj.int_field);
            Assert.Equal((double)expectedObj.RealField, actualObj.real_field);
            Assert.Equal(expectedObj.ArrayField!.Select(n => (long)n), (IEnumerable)actualObj.array_field);
        }
    }
}
