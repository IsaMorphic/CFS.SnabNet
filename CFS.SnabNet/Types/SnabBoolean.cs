namespace CFS.SnabNet.Types
{
    internal class SnabBoolean : ISnabType<bool>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabBoolean()
        {
            TypeIds = new HashSet<byte> { 0x07, 0x08 };
        }

        public bool ReadFromInstance(SnabReader reader, byte typeId)
        {
            return typeId switch
            {
                0x07 => false,
                0x08 => true,
                _ => throw new InvalidDataException($"Invalid typeId {typeId} for SnabBoolean")
            };
        }
    }
}
