using ETWDeserializer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace ETW2Grid
{
    public struct EventData
    {
        public string Date { set; get; }
        public TimeSpan Time { set; get; }
        public uint Process { set; get; }
        public uint Thread { set; get; }
        public string Name => metadata.Name;
        //public string Message => metadata.Message;
        public string Parameter { set; get; }

        internal EventMetadata metadata;
    }

    public struct Info
    {
        public Info(string name, object value)
        {
            Name = name;
            Value = value;
        }
        public string Name { set; get; }
        public object Value { set; get; }
    }

    public class EtwWriter : IEtwWriter
    {
        IList<EventData> events;
        EventData current;
        JsonWriter writer;
        StringWriter stringWriter;

        public EtwWriter(IList<EventData> events)
        {
            this.events = events;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void WriteEventBegin(EventMetadata metadata, RuntimeEventMetadata runtimeMetadata)
        {
            DateTime dt = DateTime.FromFileTime(runtimeMetadata.Timestamp);
            current = new EventData
            {
                Date = dt.ToShortDateString(),
                Time = dt.TimeOfDay,
                Process = runtimeMetadata.ProcessId,
                Thread = runtimeMetadata.ThreadId,
                metadata = metadata,
            };
            stringWriter = new StringWriter();
            writer = new JsonTextWriter(stringWriter);
            writer.WriteStartObject();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteEventEnd()
        {
            writer.WriteEndObject();
            current.Parameter = stringWriter.ToString();
            events.Add(current);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteStructBegin()
        {
            writer.WriteStartObject();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteStructEnd()
        {
            writer.WriteEndObject();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WritePropertyBegin(PropertyMetadata metadata)
        {
            writer.WritePropertyName(metadata.Name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WritePropertyEnd()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArrayBegin()
        {
            writer.WriteStartArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArrayEnd()
        {
            writer.WriteEndArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteAnsiString(string value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnicodeString(string value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt8(sbyte value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt8(byte value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt16(short value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt16(ushort value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt32(int value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt32(uint value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt64(long value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt64(ulong value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFloat(float value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDouble(double value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBoolean(bool value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBinary(byte[] value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteGuid(Guid value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WritePointer(ulong value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFileTime(DateTime value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSystemTime(DateTime value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSid(string value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnicodeChar(char value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteAnsiChar(char value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteHexDump(byte[] value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWbemSid(string value)
        {
            writer.WriteValue(value);
        }
    }
}
