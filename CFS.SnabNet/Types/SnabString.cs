
using System.Text;

namespace CFS.SnabNet.Types
{
    internal class SnabString : ISnabType<string>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabString()
        {
            TypeIds = new HashSet<byte> { 0x03, 0x04 };
        }

        public string ReadFromInstance(SnabReader instance, byte typeId)
        {
            bool isBigEndian = instance.Header.Flags.HasFlag(SnabFlags.BigEndian);

            Encoding encoding;
            switch((typeId, isBigEndian)) 
            { 
                case (0x03, _): 
                    encoding = Encoding.UTF8;
                    break;
                case (0x04, false): 
                    encoding = Encoding.Unicode;
                    break;
                case (0x04, true): 
                    encoding = Encoding.BigEndianUnicode;
                    break;
                default:
                    throw new InvalidDataException($"Invalid typeId {typeId} for SnabString");
            }

            using (BinaryReader reader = new(instance.BaseStream, encoding, true))
            {
                List<char> strBuf = new();

                char nextChar = reader.ReadChar();
                while (nextChar != '\x00')
                {
                    strBuf.Add(nextChar);
                    nextChar = reader.ReadChar();
                }

                return new(strBuf.ToArray());
            }
        }
    }
}
