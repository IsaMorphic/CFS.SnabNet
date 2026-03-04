using CFS.SnabNet.Types;
using System.Collections;
using System.Text;

namespace CFS.SnabNet
{
    public class SnabInstance
    {
        public const byte MAJOR_VERSION = 1;
        public const byte MINOR_VERSION = 0;

        private const string _LANG_ID = "CS\x00\x00";
        public static readonly uint LANG_ID =
            BitConverter.ToUInt32(
                Encoding.ASCII.GetBytes(_LANG_ID)
                );

        private readonly SortedDictionary<byte, ISnabType> _typeMap = new();

        public SnabInstance()
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

        internal ISnabType GetTypeById(byte typeId)
        {
            if (_typeMap.TryGetValue(typeId, out ISnabType? type))
            {
                return type;
            }
            else
            {
                throw new ArgumentException($"Unknown typeId {typeId}", nameof(typeId));
            }
        }

        internal byte GetTypeIdByValue(object? value)
        {
            switch (value)
            {
                // Struct types
                case ISnabStruct:
                case IDictionary<string, object?>:
                case IReadOnlyDictionary<string, object?>:
                    return SnabType.Struct;

                // String types
                case char:
                case string:
                    return SnabType.String;

                // Real types
                case Half:
                case float:
                case double:
                    return SnabType.Real;

                // Integer types
                case byte:
                case sbyte:
                case ushort:
                case short:
                case uint:
                case int:
                case ulong:
                case long:
                case nint:
                case nuint:
                    return SnabType.Integer;

                // Boolean types
                case false:
                    return SnabType.False;
                case true:
                    return SnabType.True;

                // Null types
                case Values.SnabUndefined:
                    return SnabType.Undefined;
                case null:
                    return SnabType.Null;

                // Buffer types
                case byte[]:
                    return SnabType.Buffer;

                // Array types
                case IEnumerable:
                    return SnabType.Array;

                default:
                    return _typeMap
                        .SkipWhile(x => x.Key <= SnabType.LastReserved)
                        .Select(x => (byte?)x.Value.GetTypeIdForValue(value))
                        .FirstOrDefault(id => id > SnabType.None) ?? 
                        throw new ArgumentException($"Unsupported value type {value.GetType().FullName}", nameof(value));
            }
        }

        public void RegisterType<T>()
            where T : ISnabType, new()
        {
            T type = new();

            IEnumerable<byte> conflictIds = _typeMap.Keys.Where(type.TypeIds.Contains);
            if (conflictIds.Any())
            {
                throw new ArgumentException($"Instance already contains a mapping for typeIds: {string.Join(", ", conflictIds)}", nameof(T));
            }
            else
            {
                foreach (byte typeId in type.TypeIds)
                {
                    _typeMap.Add(typeId, type);
                }
            }
        }

        public SnabReader CreateReader(Stream stream, bool leaveOpen = false)
        {
            return new SnabReader(this, stream, leaveOpen);
        }

        public SnabWriter CreateWriter(Stream stream, SnabFlags flags, bool leaveOpen = false)
        {
            return new SnabWriter(this, stream, flags, leaveOpen);
        }
    }
}
