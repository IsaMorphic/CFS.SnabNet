using System.IO.Compression;

namespace CFS.SnabNet
{
    public class SnabReader : IDisposable
    {
        private readonly SnabInstance _instance;

        private readonly bool _leaveOpen;

        private bool _disposedValue;

        internal SnabHeader Info { get; }

        public Stream BaseStream { get; }

        internal SnabReader(SnabInstance instance, Stream stream, bool leaveOpen)
        {
            _instance = instance;
            _leaveOpen = leaveOpen;

            Info = SnabHeader.ReadFromStream(stream);
            if (Info.Flags.HasFlag(SnabFlags.Compressed))
            {
                BaseStream = new ZLibStream(stream, CompressionMode.Decompress, leaveOpen);
            }
            else
            {
                BaseStream = stream;
            }
        }

        internal byte GetTypeIdByValue(object? value) => _instance.GetTypeIdByValue(value);

        internal ISnabType GetTypeById(byte typeId) => _instance.GetTypeById(typeId);

        public object Deserialize()
        {
            int typeId = BaseStream.ReadByte();
            switch (typeId)
            {
                case SnabType.Struct:
                case SnabType.Array:
                    ISnabType type = GetTypeById((byte)typeId);
                    return type.ReadFromInstance(this, (byte)typeId)!;
                case > SnabType.None:
                    throw new InvalidDataException($"Invalid typeId {typeId}; SNAB data root must be either struct or array.");
                default:
                    throw new EndOfStreamException("Unexpected end of stream while reading SNAB data.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing && (!_leaveOpen || BaseStream is ZLibStream))
                {
                    BaseStream.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
