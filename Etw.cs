namespace ETW
{
    using ETWDeserializer;
    using System;
    using System.Runtime.InteropServices;
    using GUID = System.Guid;
    using LARGE_INTEGER = System.Int64;
    using LONG = System.Int32;
    using LONGLONG = System.Int64;
    using LPWSTR = System.String;
    using UCHAR = System.Byte;
    using ULONG = System.UInt32;
    using ULONGLONG = System.UInt64;
    using USHORT = System.UInt16;

    [StructLayout(LayoutKind.Sequential)]
    internal struct TRACE_LOGFILE_HEADER
    {
        public ULONG BufferSize;         // Logger buffer size in Kbytes
        public ULONG Version;            // Logger version
        public ULONG ProviderVersion;    // defaults to NT version
        public ULONG NumberOfProcessors; // Number of Processors
        public LARGE_INTEGER EndTime;            // Time when logger stops
        public ULONG TimerResolution;    // assumes timer is constant!!!
        public ULONG MaximumFileSize;    // Maximum in Mbytes
        public ULONG LogFileMode;        // specify logfile mode
        public ULONG BuffersWritten;     // used to file start of Circular File

        public ULONG StartBuffers;       // Count of buffers written at start.
        public ULONG PointerSize;        // Size of pointer type in bits
        public ULONG EventsLost;         // Events losts during log session
        public ULONG CpuSpeedInMHz;      // Cpu Speed in MHz

        public IntPtr LoggerName;
        public IntPtr LogFileName;
        public TIME_ZONE_INFORMATION TimeZone;

        public LARGE_INTEGER BootTime;
        public LARGE_INTEGER PerfFreq;           // Reserved
        public LARGE_INTEGER StartTime;          // Reserved
        public ULONG ReservedFlags;      // ClockType
        public ULONG BuffersLost;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EVENT_TRACE_HEADER // overlays WNODE_HEADER
    {
        public USHORT Size; // Size of entire record

        public USHORT FieldTypeFlags; // Indicates valid fields

        public UCHAR Type; // event type
        public UCHAR Level; // trace instrumentation level
        public USHORT Version; // version of trace record

        public ULONG ThreadId; // Thread Id
        public ULONG ProcessId; // Process Id
        public LARGE_INTEGER TimeStamp; // time when event happens
        public GUID Guid; // Guid that identifies event

        public ULONG KernelTime; // Kernel Mode CPU ticks
        public ULONG UserTime; // User mode CPU ticks
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EVENT_TRACE
    {
        public EVENT_TRACE_HEADER Header; // Event trace header
        public ULONG InstanceId; // Instance Id of this event
        public ULONG ParentInstanceId; // Parent Instance Id.
        public GUID ParentGuid; // Parent Guid;
        public IntPtr MofData; // Pointer to Variable Data
        public ULONG MofLength; // Variable Datablock Length

        public UCHAR ProcessorNumber;
        public UCHAR Alignment;
        public USHORT LoggerId;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct EVENT_TRACE_LOGFILEW
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public LPWSTR LogFileName; // Logfile Name

        [MarshalAs(UnmanagedType.LPWStr)]
        public LPWSTR LoggerName; // LoggerName

        public LONGLONG CurrentTime; // timestamp of last event
        public ULONG BuffersRead; // buffers read to date

        public ULONG LogFileMode; // Mode of the logfile

        public EVENT_TRACE CurrentEvent; // Current Event from this stream.
        public TRACE_LOGFILE_HEADER LogfileHeader; // logfile header structure
        public PEVENT_TRACE_BUFFER_CALLBACKW BufferCallback; // callback before each buffer is read
        //
        // following variables are filled for BufferCallback.
        //
        public LONG BufferSize;
        public LONG Filled;
        public LONG EventsLost;
        //
        // following needs to be propaged to each buffer
        //

        // Callback with EVENT_RECORD on Vista and above
        public PEVENT_RECORD_CALLBACK EventRecordCallback;

        public ULONG IsKernelTrace; // TRUE for kernel logfile

        public IntPtr Context; // reserved for internal use
    }

    [StructLayout(LayoutKind.Sequential, Size = 0xAC, CharSet = CharSet.Unicode)]
    internal struct TIME_ZONE_INFORMATION
    {
        public ULONG Bias;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string StandardName;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 8)]
        public USHORT[] StandardDate;

        public ULONG StandardBias;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DaylightName;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 8)]
        public USHORT[] DaylightDate;

        public ULONG DaylightBias;
    }

    internal delegate bool PEVENT_TRACE_BUFFER_CALLBACKW([In] IntPtr logfile);

    internal unsafe delegate void PEVENT_RECORD_CALLBACK([In] EVENT_RECORD* eventRecord);


    [Flags]
    enum LogFileMode : uint
    {
        FILE_MODE_SEQUENTIAL = 0x00000001,
        FILE_MODE_CIRCULAR = 0x00000002,
        FILE_MODE_APPEND = 0x00000004,
        FILE_MODE_NEWFILE = 0x00000008,  // Auto-switch log file
        USE_MS_FLUSH_TIMER = 0x00000010,
        FILE_MODE_PREALLOCATE = 0x00000020,  // Pre-allocate mode
        NONSTOPPABLE_MODE = 0x00000040,
        SECURE_MODE = 0x00000080,
        REAL_TIME_MODE = 0x00000100,
        DELAY_OPEN_FILE_MODE = 0x00000200, //deprecated
        BUFFERING_MODE = 0x00000400,
        PRIVATE_LOGGER_MODE = 0x00000800,
        ADD_HEADER_MODE = 0x00001000,
        USE_KBYTES_FOR_SIZE = 0x00002000,
        USE_GLOBAL_SEQUENCE = 0x00004000,  // Use global sequence no.
        USE_LOCAL_SEQUENCE = 0x00008000,  // Use local sequence no.
        RELOG_MODE = 0x00010000,  // Relogger
        PRIVATE_IN_PROC = 0x00020000,
        BUFFER_INTERFACE_MODE = 0x00040000,
        KD_FILTER_MODE = 0x00080000,
        REAL_TIME_RELOG_MODE = 0x00100000,
        LOST_EVENTS_DEBUG_MODE = 0x00200000,
        STOP_ON_HYBRID_SHUTDOWN = 0x00400000,
        PERSIST_ON_HYBRID_SHUTDOWN = 0x00800000,
        USE_PAGED_MEMORY = 0x01000000,
        SYSTEM_LOGGER_MODE = 0x02000000,
        COMPRESSED_MODE = 0x04000000,
        INDEPENDENT_SESSION_MODE = 0x08000000,  // Independent logger session
        NO_PER_PROCESSOR_BUFFERING = 0x10000000,
        BLOCKING_MODE = 0x20000000,
        UnUsed = 0x40000000,
        ADDTO_TRIAGE_DUMP = 0x80000000,
    }

    enum Clock : uint
    {
        QueryPerformanceCounter = 1,
        SystemTime = 2,
        CpuCycleCounter = 3
    }

    internal static class Native
    {
        [DllImport("advapi32.dll", EntryPoint = "OpenTraceW", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern UInt64 OpenTrace([In] [Out] ref EVENT_TRACE_LOGFILEW Logfile);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal extern static int ProcessTrace([In] ULONGLONG[] HandleArray, [In] ULONG HandleCount, [In] IntPtr StartTime, [In] IntPtr EndTime);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal extern static int CloseTrace([In] ULONGLONG TraceHandle);

        internal const ULONG PROCESS_TRACE_MODE_EVENT_RECORD = 0x10000000;
        internal const ULONG PROCESS_TRACE_MODE_REAL_TIME = 0x00000100;
        internal const ULONG PROCESS_TRACE_MODE_RAW_TIMESTAMP = 0x00001000;
    }
}