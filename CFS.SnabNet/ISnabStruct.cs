namespace CFS.SnabNet
{
    public interface ISnabStruct
    {
        IReadOnlyDictionary<string, SnabField> Create();
    }
}
