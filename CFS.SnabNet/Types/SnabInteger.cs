namespace CFS.SnabNet.Types
{
    internal class SnabInteger : ISnabType<long>
    {
        public HashSet<byte> TypeIds { get; }

        public SnabInteger()
        {
            TypeIds = new HashSet<byte> { SnabType.Integer };
        }

        public long ReadFromInstance(SnabReader instance, byte typeId)
        {
            if (typeId != SnabType.Integer)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabInteger", nameof(typeId));

            Span<byte> bytesSpan = stackalloc byte[sizeof(long)];
            instance.BaseStream.ReadExactly(bytesSpan);

            if (BitConverter.IsLittleEndian ==
                instance.Info.Flags.HasFlag(SnabFlags.BigEndian))
            {
                bytesSpan.Reverse();
            }

            return BitConverter.ToInt64(bytesSpan);
        }

        public void WriteToInstance(SnabWriter instance, byte typeId, object? obj)
        {
            if (typeId != SnabType.Integer)
                throw new ArgumentException($"Invalid typeId {typeId} for SnabInteger", nameof(typeId));

            long value;
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
                    value = (long)ul;
                    break;
                case long sl:
                    value = sl;
                    break;
                case nuint un:
                    value = (long)un;
                    break;
                case nint sn:
                    value = sn;
                    break;
                default:
                    throw new ArgumentException($"Object of type '{obj?.GetType().FullName ?? "null"}' cannot be written as SnabInteger", nameof(obj));
            }

            Span<byte> bytesSpan = stackalloc byte[sizeof(long)];
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
