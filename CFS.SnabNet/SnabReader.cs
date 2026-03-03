using CFS.SnabNet.Types;
using System.IO.Compression;

namespace CFS.SnabNet
{
    public class SnabReader : IDisposable
    {
        private static readonly Dictionary<byte, ISnabType> _sTypeMap = new();

        static SnabReader()
        {
            RegisterType<SnabStruct>();
            RegisterType<SnabArray>();
            RegisterType<SnabString>();
            RegisterType<SnabReal>();
            RegisterType<SnabInteger>();
            RegisterType<SnabBoolean>();
            RegisterType<SnabUndefined>();
            RegisterType<SnabNull>();
            RegisterType<SnabBuffer>();
        }

        internal static ISnabType GetTypeById(byte typeId)
        {
            if (_sTypeMap.TryGetValue(typeId, out ISnabType? type))
            {
                return type;
            }
            else
            {
                throw new ArgumentException($"Unknown typeId {typeId}");
            }
        }

        public static void RegisterType<T>() 
            where T : ISnabType, new()
        {
            T type = new();

            IEnumerable<byte> conflictIds = _sTypeMap.Keys.Where(type.TypeIds.Contains);
            if (conflictIds.Any())
            {
                throw new ArgumentException($"SnabReader already contains a mapping for typeIds: {string.Join(", ", conflictIds)}");
            }
            else
            {
                foreach (byte typeId in type.TypeIds)
                {
                    _sTypeMap.Add(typeId, type);
                }
            }
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
                BaseStream = new ZLibStream(stream, CompressionMode.Decompress, leaveOpen);
            }
            else
            {
                BaseStream = stream;
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
