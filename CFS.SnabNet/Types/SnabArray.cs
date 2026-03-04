using System.Collections;
using System.Text;

namespace CFS.SnabNet.Types
{
    internal class SnabArray : ISnabType<IList<object?>>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabArray()
        {
            TypeIds = new HashSet<byte> { SnabType.Array };
        }

        public IList<object?> ReadFromInstance(SnabReader instance, byte typeId)
        {
            if(typeId != SnabType.Array)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabArray", nameof(typeId));

            List<object?> arrayData = new();
            using (BinaryReader reader = new(instance.BaseStream, Encoding.UTF8, true))
            {
                byte elemTypeId = reader.ReadByte();
                while (elemTypeId != SnabType.None)
                {
                    if (!instance.Info.Flags.HasFlag(SnabFlags.User) &&
                        !instance.Info.Flags.HasFlag(SnabFlags.Extended) &&
                        elemTypeId > SnabType.LastReserved)
                        throw new ArgumentException($"Cannot deserialize user-defined typeId {elemTypeId}; instance does not allow it.", nameof(instance));

                    ISnabType elemType = instance.GetTypeById(elemTypeId);
                    object? element = elemType.ReadFromInstance(instance, elemTypeId);

                    arrayData.Add(element);
                    elemTypeId = reader.ReadByte();
                }
            }

            return arrayData;
        }

        public void WriteToInstance(SnabWriter instance, byte typeId, object? obj)
        {
            if (typeId != SnabType.Array)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabArray", nameof(typeId));

            foreach (var element in (obj as IEnumerable) ?? 
                throw new ArgumentException($"Object of type '{obj?.GetType().FullName ?? "null"}' cannot be serialized as SnabArray.", nameof(obj)))
            {
                byte elemTypeId = instance.GetTypeIdByValue(element);
                if (!instance.Info.Flags.HasFlag(SnabFlags.User) &&
                    !instance.Info.Flags.HasFlag(SnabFlags.Extended) &&
                    elemTypeId > SnabType.LastReserved)
                    throw new ArgumentException($"Cannot serialize user-defined typeId {elemTypeId}; instance does not allow it.", nameof(instance));

                ISnabType elemType = instance.GetTypeById(elemTypeId);
                instance.BaseStream.WriteByte(elemTypeId);
                elemType.WriteToInstance(instance, elemTypeId, element);
            }
            instance.BaseStream.WriteByte(SnabType.None);
        }
    }
}
