using System.Text;

namespace CFS.SnabNet.Types
{
    internal class SnabStruct : ISnabType<IDictionary<string, object?>>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabStruct()
        {
            TypeIds = new HashSet<byte>() { 0x01 };
        }

        public IDictionary<string, object?> ReadFromInstance(SnabReader instance, byte typeId)
        {
            if (typeId != 0x01)
                throw new InvalidDataException($"Invalid typeId {typeId} for SnabStruct");

            Dictionary<string, object?> structData = new();
            using (BinaryReader reader = new(instance.BaseStream, Encoding.UTF8, true))
            {
                byte innerTypeId = reader.ReadByte();
                while (innerTypeId != 0x00)
                {
                    List<char> strBuf = new();
                    char nextChar = reader.ReadChar();
                    while (nextChar != '\x00')
                    {
                        strBuf.Add(nextChar);
                        nextChar = reader.ReadChar();
                    }
                    string key = new(strBuf.ToArray());

                    ISnabType innerType = SnabReader.GetTypeById(innerTypeId);
                    object? value = innerType.ReadFromInstance(instance, innerTypeId);

                    structData.Add(key, value);
                    innerTypeId = reader.ReadByte();
                }
            }

            return structData;
        }
    }
}
