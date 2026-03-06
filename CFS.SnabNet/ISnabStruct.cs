using System.Diagnostics.CodeAnalysis;

namespace CFS.SnabNet
{
    public interface ISnabStruct
    {
        IDictionary<string, object?> Dehydrate();
    }

    public interface ISnabStruct<T> : ISnabStruct
        where T : ISnabStruct<T>, new()
    {
        [return: NotNullIfNotNull(nameof(structData))]
        static abstract T? Hydrate(IDictionary<string, object?>? structData);
    }
}
