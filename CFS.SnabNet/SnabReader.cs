using System.IO.Compression;

namespace CFS.SnabNet
{
    public class SnabReader : IDisposable
    {
        private readonly bool _leaveOpen;

        private bool disposedValue;

        internal SnabHeader Info { get; }

        public Stream BaseStream { get; }

        public SnabReader(Stream stream, bool leaveOpen = false)
        {
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

        public object Deserialize()
        {
            int typeId = BaseStream.ReadByte();
            switch (typeId)
            {
                case 0x01:
                case 0x02:
                    ISnabType type = SnabLibrary.GetTypeById((byte)typeId);
                    return type.ReadFromInstance(this, (byte)typeId)!;
                case > 0:
                    throw new InvalidDataException($"Invalid typeId {typeId}; SNAB data root must be either struct or array.");
                default:
                    throw new EndOfStreamException("Unexpected end of stream while reading SNAB data.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing && (!_leaveOpen || BaseStream is ZLibStream))
                {
                    BaseStream.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
