
namespace CFS.SnabNet.Types
{
    internal class SnabReal : ISnabType<double>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabReal()
        {
            TypeIds = new HashSet<byte> { SnabType.Real };
        }

        public double ReadFromInstance(SnabReader instance, byte typeId)
        {
            if (typeId != SnabType.Real)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabReal", nameof(typeId));

            Span<byte> bytesSpan = stackalloc byte[sizeof(double)];
            instance.BaseStream.ReadExactly(bytesSpan);

            if (BitConverter.IsLittleEndian ==
                instance.Info.Flags.HasFlag(SnabFlags.BigEndian))
            {
                bytesSpan.Reverse();
            }

            return BitConverter.ToDouble(bytesSpan);
        }

        public void WriteToInstance(SnabWriter instance, byte typeId, object? obj)
        {
            if (typeId != SnabType.Real)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabReal", nameof(typeId));

            double value;
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
                    value = (double)h;
                    break;
                case float f:
                    value = f;
                    break;
                case double d:
                    value = d;
                    break;
                default:
                    throw new ArgumentException($"Object of type '{obj?.GetType().FullName ?? "null"}' cannot be written as SnabReal", nameof(obj));
            }

            Span<byte> bytesSpan = stackalloc byte[sizeof(double)];
            BitConverter.TryWriteBytes(bytesSpan, value);
            if (BitConverter.IsLittleEndian ==
                instance.Info.Flags.HasFlag(SnabFlags.BigEndian))
            {
                bytesSpan.Reverse();
            }
            instance.BaseStream.Write(bytesSpan);
        }
    }
}
