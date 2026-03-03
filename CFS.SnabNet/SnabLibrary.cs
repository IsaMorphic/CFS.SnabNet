using CFS.SnabNet.Types;
using System.Text;

namespace CFS.SnabNet
{
    public static class SnabLibrary
    {
        public const byte MAJOR_VERSION = 1;
        public const byte MINOR_VERSION = 0;

        private const string _LANG_ID = "CS\x00\x00";
        public static readonly uint LANG_ID =
            BitConverter.ToUInt32(
                Encoding.ASCII.GetBytes(_LANG_ID)
                );

        private static readonly Dictionary<byte, ISnabType> _sTypeMap = new();

        static SnabLibrary()
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
                throw new ArgumentException($"Unknown typeId {typeId}", nameof(typeId));
            }
        }

        internal static byte GetTypeIdByValue(object? value)
        {
            switch (value)
            {
                // Struct types
                case ISnabStruct:
                case IDictionary<string, object?>:
                    return 0x01;

                // Array types
                case IReadOnlyList<object?>:
                    return 0x02;

                // String types
                case char:
                case string:
                    return 0x03;

                // Real types
                case Half:
                case float:
                case double:
                    return 0x05;

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
                    return 0x06;

                // Boolean types
                case false:
                    return 0x07;
                case true:
                    return 0x08;

                // Null types
                case Values.SnabUndefined:
                    return 0x09;
                case null:
                    return 0x0A;

                // Buffer types
                case byte[]:
                    return 0x0B;

                default:
                    throw new ArgumentException($"Unsupported value type {value.GetType().FullName}", nameof(value));
            }
        }

        public static void RegisterType<T>()
            where T : ISnabType, new()
        {
            T type = new();

            IEnumerable<byte> conflictIds = _sTypeMap.Keys.Where(type.TypeIds.Contains);
            if (conflictIds.Any())
            {
                throw new ArgumentException($"SnabReader already contains a mapping for typeIds: {string.Join(", ", conflictIds)}", nameof(T));
            }
            else
            {
                foreach (byte typeId in type.TypeIds)
                {
                    _sTypeMap.Add(typeId, type);
                }
            }
        }
    }
}
