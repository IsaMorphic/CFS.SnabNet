namespace CFS.SnabNet
{
    public interface ISnabType
    {
        HashSet<byte> TypeIds { get; }

        object? ReadFromInstance(SnabReader instance, byte typeId);

        void WriteToInstance(SnabWriter instance, byte typeId, object? obj);
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
