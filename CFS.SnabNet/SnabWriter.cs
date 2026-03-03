using System.IO.Compression;
using System.IO.Hashing;

namespace CFS.SnabNet
{
    public class SnabWriter : IDisposable
    {
        private readonly Stream _stream;
        private readonly bool _leaveOpen;

        private readonly MemoryStream _buffer;
        private bool _isCompleted;

        private bool _disposedValue;

        public SnabHeader Info { get; }

        public Stream BaseStream { get; }

        public SnabWriter(Stream stream, SnabFlags flags, bool leaveOpen = false)
        {
            _leaveOpen = leaveOpen;
            _stream = stream;

            Info = new SnabHeader()
            {
                MajorVersion = SnabLibrary.MAJOR_VERSION,
                MinorVersion = SnabLibrary.MINOR_VERSION,
                LangId = SnabLibrary.LANG_ID,
                Flags = flags,
            };

            _buffer = new MemoryStream();
            if (flags.HasFlag(SnabFlags.Compressed))
            {
                BaseStream = new ZLibStream(_buffer, CompressionMode.Compress, false);
            }
            else 
            {
                BaseStream = _buffer;
            }
        }

        public void Serialize(object? obj)
        {
            if (_isCompleted) 
            {
                throw new InvalidOperationException("Cannot serialize after completion.");
            }

            byte typeId = SnabLibrary.GetTypeIdByValue(obj);
            switch (typeId)
            {
                case 0x01:
                case 0x02:
                    BaseStream.WriteByte(typeId);
                    ISnabType type = SnabLibrary.GetTypeById(typeId);
                    type.WriteToInstance(this, typeId, obj);
                    break;
                default:
                    throw new ArgumentException("Value must be either ISnabStruct or IReadOnlyList.", nameof(obj));
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
                if (disposing)
                {
                    if (!_leaveOpen)
                    {
                        _stream.Dispose();
                    }
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
