namespace CFS.SnabNet
{
    [Flags]
    public enum SnabFlags : ushort
    {
        None = 0x0000,
        BigEndian = 0x0001,
        Extended = 0x0002,
        User = 0x0004,
        Compressed = 0x0008,
    }
}