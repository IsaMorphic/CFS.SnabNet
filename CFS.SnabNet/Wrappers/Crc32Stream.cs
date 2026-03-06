using System.IO.Hashing;

namespace CFS.SnabNet.Wrappers
{
    internal class Crc32Stream : Stream
    {
        private readonly Stream _innerStream;
        private readonly bool _leaveOpen;

        private readonly Crc32 _crc32;

        private bool _disposedValue;

        public uint Crc32Value => _crc32.GetCurrentHashAsUInt32();

        public Crc32Stream(Stream stream, bool leaveOpen = false)
        {
            _innerStream = stream;
            _leaveOpen = leaveOpen;

            _crc32 = new Crc32();
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => _innerStream.Length;

        public override long Position 
        { 
            get => _innerStream.Position; 
            set => throw new NotSupportedException(); 
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int readCount = _innerStream.Read(buffer, offset, count);
            _crc32.Append(buffer.AsSpan(offset, readCount));
            return readCount;
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing && !_leaveOpen)
                {
                    _innerStream.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
