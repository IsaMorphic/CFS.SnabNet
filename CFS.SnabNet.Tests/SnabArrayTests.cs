using CFS.SnabNet.Values;

namespace CFS.SnabNet.Tests
{
    public class SnabArrayTests
    {
        [Fact]
        public void ShouldResolveTypeCorrectly_List()
        {
            SnabInstance instance = new();
            byte actualTypeId = instance.GetTypeIdByValue((List<int>)[0, 1, 2]);
            Assert.Equal(SnabType.Array, actualTypeId);
        }

        [Fact]
        public void ShouldResolveTypeCorrectly_Array()
        {
            SnabInstance instance = new();
            byte actualTypeId = instance.GetTypeIdByValue((int[])[0, 1, 2]);
            Assert.Equal(SnabType.Array, actualTypeId);
        }

        [Fact]
        public void RoundtripIsCorrect_List() 
        {
            List<object?> expectedList = [1L, true, null, new SnabUndefined(), "Hello world!"];

            SnabInstance instance = new();

            IList<object?> actualList;
            using (MemoryStream ms = new())
            {
                using (SnabWriter writer = instance.CreateWriter(ms, SnabFlags.None, true))
                {
                    writer.Serialize(expectedList);
                }

                ms.Position = 0;
                using (SnabReader reader = instance.CreateReader(ms, true))
                {
                    actualList = (IList<object?>)reader.Deserialize();
                }
            }

            Assert.Equal(expectedList, actualList);
        }

        [Fact]
        public void RoundtripIsCorrect_Array()
        {
            object?[] expectedArr = [1L, true, null, new SnabUndefined(), "Hello world!"];

            SnabInstance instance = new();

            IList<object?> actualArr;
            using (MemoryStream ms = new())
            {
                using (SnabWriter writer = instance.CreateWriter(ms, SnabFlags.None, true))
                {
                    writer.Serialize(expectedArr);
                }

                ms.Position = 0;
                using (SnabReader reader = instance.CreateReader(ms, true))
                {
                    actualArr = (IList<object?>)reader.Deserialize();
                }
            }

            Assert.Equal(expectedArr, actualArr);
        }
    }
}
