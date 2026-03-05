using CFS.SnabNet.Tests.Types;
using CFS.SnabNet.Types;

namespace CFS.SnabNet.Tests
{
    public class SnabInstanceTests
    {
        [Theory]
        [InlineData([SnabType.Struct, nameof(SnabStruct)])]
        [InlineData([SnabType.Array, nameof(SnabArray)])]
        [InlineData([SnabType.String, nameof(SnabString)])]
        [InlineData([SnabType.StringW, nameof(SnabString)])]
        [InlineData([SnabType.Real, nameof(SnabReal)])]
        [InlineData([SnabType.Integer, nameof(SnabInteger)])]
        [InlineData([SnabType.False, nameof(SnabBoolean)])]
        [InlineData([SnabType.True, nameof(SnabBoolean)])]
        [InlineData([SnabType.Undefined, nameof(SnabUndefined)])]
        [InlineData([SnabType.Null, nameof(SnabNull)])]
        [InlineData([SnabType.Buffer, nameof(SnabBuffer)])]
        public void DefaultTypeIsMappedCorrectly(byte typeId, string typeName)
        {
            SnabInstance instance = new();
            ISnabType type = instance.GetTypeById(typeId);
            Assert.Equal(typeName, type.GetType().Name);
        }

        [Theory]
        [InlineData([SnabType.Array, new int[] { 1, 2, 3 }])]
        [InlineData([SnabType.String, 'E'])]
        [InlineData([SnabType.String, "hello world"])]
        [InlineData([SnabType.Real, 3.1415f])]
        [InlineData([SnabType.Real, 3.1415])]
        [InlineData([SnabType.Integer, 42])]
        [InlineData([SnabType.Integer, 42U])]
        [InlineData([SnabType.Integer, 42L])]
        [InlineData([SnabType.Integer, 42UL])]
        [InlineData([SnabType.False, false])]
        [InlineData([SnabType.True, true])]
        [InlineData([SnabType.Null, null])]
        [InlineData([SnabType.Buffer, new byte[] { 1, 2, 3 }])]
        public void DefaultTypeIsResolvedCorrectly(byte typeId, object? obj)
        {
            SnabInstance instance = new();
            byte resolvedTypeId = instance.GetTypeIdByValue(obj);
            Assert.Equal(typeId, resolvedTypeId);
        }

        [Fact]
        public void ShouldNotRegisterConflictedDefaultType()
        {
            SnabInstance instance = new();
            Assert.Throws<ArgumentException>(
                () => instance.RegisterType<ConflictedDefaultType>(isDefaultType: true)
                );
        }

        [Fact]
        public void ShouldRegisterValidCustomType()
        {
            SnabInstance instance = new();
            instance.RegisterType<ValidCustomType>();

            ISnabType type = instance.GetTypeById(ValidCustomType.TypeId);
            Assert.IsType<ValidCustomType>(type);
        }

        [Fact]
        public void ShouldNotRegisterInvalidCustomType()
        {
            SnabInstance instance = new();
            Assert.Throws<ArgumentException>(
                instance.RegisterType<InvalidCustomType>
                );
        }

        [Fact]
        public void ShouldNotRegisterConflictedCustomType()
        {
            SnabInstance instance = new();
            instance.RegisterType<ValidCustomType>();

            Assert.Throws<ArgumentException>(
                instance.RegisterType<ValidCustomType>
                );
        }
    }
}
