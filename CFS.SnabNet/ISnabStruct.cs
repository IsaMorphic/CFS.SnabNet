namespace CFS.SnabNet
{
    public interface ISnabStruct
    {
        IDictionary<string, object?> Dehydrate();
    }

    public interface ISnabStruct<T> : ISnabStruct
        where T : ISnabStruct<T>, new()
    {
        static abstract T Hydrate(IDictionary<string, object?> structData);
    }
}
