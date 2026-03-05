using System.IO.Compression;
using System.IO.Hashing;

namespace CFS.SnabNet
{
    public class SnabWriter : IDisposable
    {
        private readonly SnabInstance _instance;

        private readonly Stream _stream;
        private readonly bool _leaveOpen;

        private readonly MemoryStream _buffer;
        private bool _isCompleted;

        private bool _disposedValue;

        internal SnabHeader Info { get; }

        public Stream BaseStream { get; }

        internal SnabWriter(SnabInstance instance, MemoryStream? buffer, Stream stream, SnabFlags flags, bool leaveOpen)
        {
            _instance = instance;

            _stream = stream;
            _leaveOpen = leaveOpen;

            Info = new SnabHeader()
            {
                MajorVersion = SnabInstance.MAJOR_VERSION,
                MinorVersion = SnabInstance.MINOR_VERSION,
                LangId = SnabInstance.LANG_ID,
                Flags = flags,
            };

            _buffer = buffer ?? new MemoryStream();
            if (flags.HasFlag(SnabFlags.Compressed))
            {
                BaseStream = new ZLibStream(_buffer, CompressionMode.Compress, false);
            }
            else
            {
                BaseStream = _buffer;
            }
        }

        internal byte GetTypeIdByValue(object? value) => _instance.GetTypeIdByValue(value);

        internal ISnabType GetTypeById(byte typeId) => _instance.GetTypeById(typeId);

        public void Serialize(object obj)
        {
            if (_isCompleted)
            {
                throw new InvalidOperationException("Cannot serialize after completion.");
            }

            byte typeId = GetTypeIdByValue(obj);
            switch (typeId)
            {
                case SnabType.Struct:
                case SnabType.Array:
                    BaseStream.WriteByte(typeId);
                    ISnabType type = GetTypeById(typeId);
                    type.WriteToInstance(this, typeId, obj);
                    break;
                default:
                    throw new ArgumentException("Object must be either IReadOnlyDictionary<string, object?>, IEnumerable, or ISnabStruct.", nameof(obj));
            }

            if (BaseStream != _buffer)
            {
                BaseStream.Dispose();
            }

            byte[] bufferArr;
            using (_buffer)
            {
                bufferArr = _buffer.ToArray();
            }

            Info.Checksum = Crc32.HashToUInt32(bufferArr);
            Info.Length = (uint)bufferArr.Length;
            Info.WriteToStream(_stream);

            _stream.Write(bufferArr, 0, bufferArr.Length);
            _isCompleted = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing && !_leaveOpen)
                {
                    _stream.Dispose();
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
