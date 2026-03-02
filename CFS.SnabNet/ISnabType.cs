namespace CFS.SnabNet
{
    public interface ISnabType
    {
        HashSet<byte> TypeIds { get; }

        object? ReadFromInstance(SnabReader instance, byte typeId);
    }

    public interface ISnabType<T> : ISnabType
    {
        object? ISnabType.ReadFromInstance(SnabReader instance, byte typeId)
        {
            return ReadFromInstance(instance, typeId);
        }

        new T ReadFromInstance(SnabReader instance, byte typeId);
    }
}
