using CFS.SnabNet.Values;
using System.Text;

namespace CFS.SnabNet.Types
{
    internal class SnabStruct : ISnabType<IDictionary<string, object?>>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabStruct()
        {
            TypeIds = new HashSet<byte>() { SnabType.Struct };
        }

        public IDictionary<string, object?> ReadFromInstance(SnabReader instance, byte typeId)
        {
            if (typeId != SnabType.Struct)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabStruct", nameof(typeId));

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

                    ISnabType innerType = SnabLibrary.GetTypeById(innerTypeId);
                    object? value = innerType.ReadFromInstance(instance, innerTypeId);

                    structData.Add(key, value);
                    innerTypeId = reader.ReadByte();
                }
            }

            return structData;
        }

        public void WriteToInstance(SnabWriter instance, byte typeId, object? obj)
        {
            if (typeId != SnabType.Struct)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabStruct", nameof(typeId));

            switch (obj) 
            {
                case ISnabStruct @struct:
                    IReadOnlyDictionary<string, SnabField> structData = @struct.Create();
                    using (BinaryWriter writer = new(instance.BaseStream, Encoding.UTF8, true))
                    {
                        foreach ((string key, (byte typeIdOrZero, object? value)) in structData)
                        {
                            byte innerTypeId = typeIdOrZero == 0x00 ?
                                SnabLibrary.GetTypeIdByValue(value) :
                                typeIdOrZero;
                            writer.Write(innerTypeId);

                            writer.Write(key.ToCharArray());
                            writer.Write('\x00');

                            ISnabType innerType = SnabLibrary.GetTypeById(innerTypeId);
                            innerType.WriteToInstance(instance, innerTypeId, value);
                        }
                        writer.Write((byte)0x00);
                    }
                    break;
                case IReadOnlyDictionary<string, object?> dict:
                    using (BinaryWriter writer = new(instance.BaseStream, Encoding.UTF8, true))
                    {
                        foreach ((string key, object? value) in dict)
                        {
                            byte innerTypeId = SnabLibrary.GetTypeIdByValue(value);
                            writer.Write(innerTypeId);

                            writer.Write(key.ToCharArray());
                            writer.Write('\x00');

                            ISnabType innerType = SnabLibrary.GetTypeById(innerTypeId);
                            innerType.WriteToInstance(instance, innerTypeId, value);
                        }
                        writer.Write((byte)0x00);
                    }
                    break;
                default:
                    throw new ArgumentException($"Object of type '{obj?.GetType().FullName ?? "null"}' cannot be written as SnabStruct", nameof(obj));
            }
        }
    }
}
