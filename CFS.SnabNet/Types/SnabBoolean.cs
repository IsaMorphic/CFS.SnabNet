namespace CFS.SnabNet.Types
{
    internal class SnabBoolean : ISnabType<bool>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabBoolean()
        {
            TypeIds = new HashSet<byte> { SnabType.False, SnabType.True };
        }

        public bool ReadFromInstance(SnabReader reader, byte typeId)
        {
            return typeId switch
            {
                SnabType.False => false,
                SnabType.True => true,
                _ => throw new ArgumentException($"Invalid typeId {typeId} for SnabBoolean", nameof(typeId))
            };
        }

        public void WriteToInstance(SnabWriter instance, byte typeId, object? obj)
        {
            switch ((typeId, obj)) 
            {
                case (SnabType.False, false):
                case (SnabType.True, true):
                    break;
                default:
                    throw new ArgumentException($"Invalid typeId {typeId} for SnabBoolean", nameof(typeId));
            }
        }
    }
}
