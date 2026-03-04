using System.Dynamic;
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

            IDictionary<string, object?> structData = new ExpandoObject();
            using (BinaryReader reader = new(instance.BaseStream, Encoding.UTF8, true))
            {
                byte innerTypeId = reader.ReadByte();
                while (innerTypeId != SnabType.None)
                {
                    if (!instance.Info.Flags.HasFlag(SnabFlags.User) && 
                        !instance.Info.Flags.HasFlag(SnabFlags.Extended) && 
                        innerTypeId > SnabType.LastReserved)
                        throw new ArgumentException($"Cannot deserialize user-defined typeId {innerTypeId}; instance does not allow it.", nameof(instance));

                    List<char> strBuf = new();
                    char nextChar = reader.ReadChar();
                    while (nextChar != '\x00')
                    {
                        strBuf.Add(nextChar);
                        nextChar = reader.ReadChar();
                    }
                    string key = new(strBuf.ToArray());

                    ISnabType innerType = instance.GetTypeById(innerTypeId);
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

            object? dict = (obj as ISnabStruct)?.Create() ?? obj;
            switch (dict) 
            {
                case IDictionary<string, object?>:
                case IReadOnlyDictionary<string, object?>:
                    IEnumerable<KeyValuePair<string, object?>> items =
                        (IEnumerable<KeyValuePair<string, object?>>)dict;
                    using (BinaryWriter writer = new(instance.BaseStream, Encoding.UTF8, true))
                    {
                        foreach ((string key, object? value) in items)
                        {
                            byte innerTypeId = value is SnabField field ? 
                                field.TypeId : SnabType.None;
                            innerTypeId = innerTypeId == SnabType.None ?
                                instance.GetTypeIdByValue(
                                    (value as SnabField)?.Value ?? value
                                    ) : innerTypeId;
                            if (!instance.Info.Flags.HasFlag(SnabFlags.User) &&
                                !instance.Info.Flags.HasFlag(SnabFlags.Extended) &&
                                innerTypeId > SnabType.LastReserved)
                                throw new ArgumentException($"Cannot serialize user-defined typeId {innerTypeId}; instance does not allow it.", nameof(instance));

                            ISnabType innerType = instance.GetTypeById(innerTypeId);
                            writer.Write(innerTypeId);

                            writer.Write(key.ToCharArray());
                            writer.Write('\x00');

                            innerType.WriteToInstance(instance, innerTypeId, 
                                (value as SnabField)?.Value ?? value);
                        }
                        writer.Write(SnabType.None);
                    }
                    break;
                default:
                    throw new ArgumentException($"Object of type '{obj?.GetType().FullName ?? "null"}' cannot be written as SnabStruct", nameof(obj));
            }
        }
    }
}
