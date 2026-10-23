
using QuadrupleLib;
using System.Runtime.InteropServices;

namespace CFS.SnabNet.Types.Extension
{
    public class SnabBigReal : ISnabType<Float128>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabBigReal()
        {
            TypeIds = [SnabType.BigReal];
        }

        public Float128 ReadFromInstance(SnabReader instance, byte typeId)
        {
            if (!TypeIds.Contains(typeId))
            {
                throw new ArgumentException($"Invalid typeId {typeId} for SnabBigReal", nameof(typeId));
            }

            Span<byte> bytesSpan = stackalloc byte[16];
            instance.BaseStream.ReadExactly(bytesSpan);

            if (BitConverter.IsLittleEndian == instance.Flags.HasFlag(SnabFlags.BigEndian))
            {
                bytesSpan.Reverse();
            }

            return MemoryMarshal.AsRef<Float128>(bytesSpan);
        }

        public void WriteToInstance(SnabWriter instance, byte typeId, object? obj)
        {
            if (!TypeIds.Contains(typeId))
                throw new ArgumentException($"Invalid typeId {typeId} for SnabBigReal", nameof(typeId));

            Float128 value;
            switch (obj)
            {
                case byte ub:
                    value = ub;
                    break;
                case sbyte sb:
                    value = sb;
                    break;
                case ushort us:
                    value = us;
                    break;
                case short ss:
                    value = ss;
                    break;
                case uint ui:
                    value = ui;
                    break;
                case int si:
                    value = si;
                    break;
                case ulong ul:
                    value = ul;
                    break;
                case long sl:
                    value = sl;
                    break;
                case nuint un:
                    value = un;
                    break;
                case nint sn:
                    value = sn;
                    break;
                case Half h:
                    value = (Float128)h;
                    break;
                case float f:
                    value = f;
                    break;
                case double d:
                    value = d;
                    break;
                case Float128 r:
                    value = r;
                    break;
                default:
                    throw new ArgumentException($"Object of type '{obj?.GetType().FullName ?? "null"}' cannot be written as SnabBigReal", nameof(obj));
            }

            Span<byte> bytesSpan = MemoryMarshal.AsBytes((Span<Float128>)[value]);
            if (BitConverter.IsLittleEndian ==
                instance.Flags.HasFlag(SnabFlags.BigEndian))
            {
                bytesSpan.Reverse();
            }

            instance.BaseStream.Write(bytesSpan);
        }

        byte GetTypeIdForValue(object? value) 
        {
            return value is Float128 ? SnabType.BigReal : SnabType.None;
        }
    }
}
