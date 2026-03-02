using CFS.SnabNet.Types;
using System.IO.Compression;

namespace CFS.SnabNet
{
    public class SnabReader : IDisposable
    {
        private static readonly ISnabType[] _sTypes = [
            new SnabStruct(),
            new SnabArray(),
            new SnabString(),
            new SnabReal(),
            new SnabInteger(),
            new SnabBoolean(),
            new SnabUndefined(),
            new SnabNull(),
            new SnabBuffer(),
        ];

        private static readonly IReadOnlyDictionary<byte, ISnabType> _sTypeMap;

        static SnabReader()
        {
            Dictionary<byte, ISnabType> typeMap = new();
            foreach (ISnabType type in _sTypes)
            {
                foreach (byte typeId in type.TypeIds)
                {
                    typeMap.Add(typeId, type);
                }
            }
            _sTypeMap = typeMap;
        }

        private readonly bool _leaveOpen;

        private bool disposedValue;

        internal SnabHeader Header { get; }

        public Stream BaseStream { get; }

        public SnabReader(Stream stream, bool leaveOpen = false)
        {
            _leaveOpen = leaveOpen;

            Header = SnabHeader.ReadFromStream(stream);

            if (Header.Flags.HasFlag(SnabFlags.Compressed))
            {
                BaseStream = new ZLibStream(stream, CompressionMode.Decompress, false);
            }
            else 
            {
                BaseStream = stream;
            }
        }

        internal static ISnabType GetTypeById(byte typeId)
        {
            if (_sTypeMap.TryGetValue(typeId, out ISnabType? type))
            {
                return type;
            }
            else
            {
                throw new InvalidDataException($"Unknown typeId {typeId}");
            }
        }

        public object? Deserialize()
        {
            int typeId = BaseStream.ReadByte();
            switch (typeId) 
            {
                case 0x01:
                case 0x02:
                    ISnabType type = GetTypeById((byte)typeId);
                    return type.ReadFromInstance(this, (byte)typeId);
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
                if (disposing && !_leaveOpen)
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
