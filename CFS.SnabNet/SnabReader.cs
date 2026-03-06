using CFS.SnabNet.Wrappers;
using System.IO.Compression;

namespace CFS.SnabNet
{
    public class SnabReader : IDisposable
    {
        private readonly SnabInstance _instance;

        private readonly Crc32Stream _stream;

        private readonly bool _leaveOpen;

        private bool _disposedValue;

        internal SnabHeader Info { get; }

        public Stream BaseStream { get; }

        internal SnabReader(SnabInstance instance, SnabHeader? header, Stream stream, bool leaveOpen)
        {
            _instance = instance;
            _leaveOpen = leaveOpen;

            Info = header ?? SnabHeader.ReadFromStream(stream);

            _stream = new Crc32Stream(stream);
            if (Info.Flags.HasFlag(SnabFlags.Compressed))
            {
                BaseStream = new ZLibStream(_stream, CompressionMode.Decompress, leaveOpen);
            }
            else
            {
                BaseStream = _stream;
            }
        }

        internal byte GetTypeIdByValue(object? value) => _instance.GetTypeIdByValue(value);

        internal ISnabType GetTypeById(byte typeId) => _instance.GetTypeById(typeId);

        public object Deserialize()
        {
            object value;

            int typeId = BaseStream.ReadByte();
            switch (typeId)
            {
                case SnabType.Struct:
                case SnabType.Array:
                    ISnabType type = GetTypeById((byte)typeId);
                    value = type.ReadFromInstance(this, (byte)typeId)!;
                    break;
                case > SnabType.None:
                    throw new InvalidDataException($"Invalid typeId {typeId}; SNAB data root must be either struct or array.");
                default:
                    throw new EndOfStreamException("Unexpected end of stream while reading SNAB data.");
            }

            if(_stream.Position - SnabHeader.HEADER_SIZE != Info.Length)
            {
                throw new InvalidDataException($"SNAB data integrity check failed: expected length {Info.Length} bytes, but read {_stream.Position} bytes.");
            }

            if (_stream.Crc32Value != Info.Checksum)
            {
                throw new InvalidDataException("SNAB data integrity check failed: CRC32 checksum does not match the expected value.");
            }

            return value;
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
