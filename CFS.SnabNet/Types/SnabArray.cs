using System.Text;

namespace CFS.SnabNet.Types
{
    internal class SnabArray : ISnabType<IList<object?>>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabArray()
        {
            TypeIds = new HashSet<byte> { 0x02 };
        }

        public IList<object?> ReadFromInstance(SnabReader instance, byte typeId)
        {
            if(typeId != 0x02)
                throw new InvalidDataException($"Invalid typeId {typeId} for SnabStruct");

            List<object?> arrayData = new();
            using (BinaryReader reader = new(instance.BaseStream, Encoding.UTF8, true))
            {
                byte innerTypeId = reader.ReadByte();
                while (innerTypeId != 0x00)
                {
                    ISnabType innerType = SnabReader.GetTypeById(innerTypeId);
                    object? element = innerType.ReadFromInstance(instance, innerTypeId);

                    arrayData.Add(element);
                    innerTypeId = reader.ReadByte();
                }
            }

            return arrayData;
        }
    }
}
