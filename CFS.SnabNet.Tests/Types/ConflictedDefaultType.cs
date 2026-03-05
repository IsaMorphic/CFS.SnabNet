
namespace CFS.SnabNet.Tests.Types
{
    internal class ConflictedDefaultType : ISnabType
    {
        public HashSet<byte> TypeIds { get; }

        public ConflictedDefaultType() 
        {
            TypeIds = new HashSet<byte>() { SnabType.Real, SnabType.Integer };
        }

        public object? ReadFromInstance(SnabReader instance, byte typeId)
        {
            throw new NotImplementedException();
        }

        public void WriteToInstance(SnabWriter instance, byte typeId, object? obj)
        {
            throw new NotImplementedException();
        }
    }
}
