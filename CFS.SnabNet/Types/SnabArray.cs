using System.Text;

namespace CFS.SnabNet.Types
{
    internal class SnabArray : ISnabType<IReadOnlyList<object?>>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabArray()
        {
            TypeIds = new HashSet<byte> { SnabType.Array };
        }

        public IReadOnlyList<object?> ReadFromInstance(SnabReader instance, byte typeId)
        {
            if(typeId != SnabType.Array)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabArray", nameof(typeId));

            List<object?> arrayData = new();
            using (BinaryReader reader = new(instance.BaseStream, Encoding.UTF8, true))
            {
                byte innerTypeId = reader.ReadByte();
                while (innerTypeId != 0x00)
                {
                    ISnabType innerType = SnabLibrary.GetTypeById(innerTypeId);
                    object? element = innerType.ReadFromInstance(instance, innerTypeId);

                    arrayData.Add(element);
                    innerTypeId = reader.ReadByte();
                }
            }

            return arrayData;
        }

        public void WriteToInstance(SnabWriter instance, byte typeId, object? obj)
        {
            if (typeId != SnabType.Array)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabArray", nameof(typeId));

            switch (obj) 
            {
                case IReadOnlyList<object?> list:
                    foreach (var element in list)
                    {
                        byte elemTypeId = SnabLibrary.GetTypeIdByValue(element);
                        instance.BaseStream.WriteByte(elemTypeId);

                        ISnabType elemType = SnabLibrary.GetTypeById(elemTypeId);
                        elemType.WriteToInstance(instance, elemTypeId, element);
                    }
                    instance.BaseStream.WriteByte(0x00);
                    break;
            }
        }
    }
}
