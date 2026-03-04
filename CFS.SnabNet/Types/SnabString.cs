
using System.Text;

namespace CFS.SnabNet.Types
{
    internal class SnabString : ISnabType<string>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabString()
        {
            TypeIds = new HashSet<byte> { SnabType.String, SnabType.StringW };
        }

        public string ReadFromInstance(SnabReader instance, byte typeId)
        {
            bool isBigEndian = instance.Info.Flags.HasFlag(SnabFlags.BigEndian);

            Encoding encoding;
            switch((typeId, isBigEndian)) 
            { 
                case (SnabType.String, _): 
                    encoding = Encoding.UTF8;
                    break;
                case (SnabType.StringW, false): 
                    encoding = Encoding.Unicode;
                    break;
                case (SnabType.StringW, true): 
                    encoding = Encoding.BigEndianUnicode;
                    break;
                default:
                    throw new ArgumentException($"Invalid typeId {typeId} for SnabString", nameof(typeId));
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

        public void WriteToInstance(SnabWriter instance, byte typeId, object? obj)
        {
            bool isBigEndian = instance.Info.Flags.HasFlag(SnabFlags.BigEndian);

            Encoding encoding;
            switch ((typeId, isBigEndian))
            {
                case (SnabType.String, _):
                    encoding = Encoding.UTF8;
                    break;
                case (SnabType.StringW, false):
                    encoding = Encoding.Unicode;
                    break;
                case (SnabType.StringW, true):
                    encoding = Encoding.BigEndianUnicode;
                    break;
                default:
                    throw new ArgumentException($"Invalid typeId {typeId} for SnabString", nameof(typeId));
            }

            using (BinaryWriter writer = new(instance.BaseStream, encoding, true))
            {
                switch (obj)
                {
                    case char c:
                        writer.Write(c);
                        break;
                    case string s:
                        writer.Write(s.ToCharArray());
                        break;
                }
                writer.Write('\x00');
            }
        }
    }
}
