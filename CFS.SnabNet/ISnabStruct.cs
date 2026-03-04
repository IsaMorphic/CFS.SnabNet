namespace CFS.SnabNet
{
    public interface ISnabStruct
    {
        IReadOnlyDictionary<string, object?> Create();
    }
}
