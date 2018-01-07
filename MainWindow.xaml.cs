using ETW;
using ETWDeserializer;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ETW2Grid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            BindingOperations.EnableCollectionSynchronization(infos, infos);
            BindingOperations.EnableCollectionSynchronization(events, events);
            logView = CollectionViewSource.GetDefaultView(events);
            InitializeComponent();
            infoGrid.ItemsSource = infos;
            logGrid.ItemsSource = logView;

            var args = Environment.GetCommandLineArgs();
            if (args.Length < 2)
            {
                infos.Add(new Info("Command Line", "LogFileName [number of buffers]"));
                return;
            }
            LogFileName = args[1];
            buffersViewMax = 10;
            if (args.Length > 2)
            {
                int.TryParse(args[2], out buffersViewMax);
            }

            Task.Run((Action)Start);
        }

        Collection<EventData> events = new ObservableCollection<EventData>();
        Collection<Info> infos = new ObservableCollection<Info>();
        ICollectionView logView;
        Deserializer<EtwWriter> deserializer;

        string LogFileName;
        int buffersViewMax;

        private void Group_Checked(object sender, RoutedEventArgs e)
        {
            logView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(EventData.Name)));
        }

        private void Group_UnChecked(object sender, RoutedEventArgs e)
        {
            logView.GroupDescriptions.Clear();
        }

        private void Filter_Checked(object sender, RoutedEventArgs e)
        {
            if (logGrid.CurrentCell.Item is EventData current)
            {
                switch (logGrid.CurrentCell.Column.Header)
                {
                    case nameof(EventData.Process):
                        logView.Filter = x => x is EventData data && data.Process == current.Process;
                        break;
                    case nameof(EventData.Thread):
                        logView.Filter = x => x is EventData data && data.Thread == current.Thread;
                        break;
                }
            }
        }

        private void Filter_UnChecked(object sender, RoutedEventArgs e)
        {
            logView.Filter = null;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Task.Run((Action)Start);
        }

        void Start()
        {
            infos.Clear();
            infos.Add(new Info("LogFileName", LogFileName));
            infos.Add(new Info("ViewLastBuffers", buffersViewMax));
            events.Clear();
            deserializer = new Deserializer<EtwWriter>(new EtwWriter(events));
            var logfile = new EVENT_TRACE_LOGFILEW
            {
                LogFileName = LogFileName,
                BufferCallback = BufferCallback,
                LogFileMode = Native.PROCESS_TRACE_MODE_EVENT_RECORD,
            };
            unsafe { logfile.EventRecordCallback = Deserialize; }

            var handle = Native.OpenTrace(ref logfile);
            int error = Marshal.GetLastWin32Error();
            if (error != 0)
            {
                infos.Add(new Info("Error", new Win32Exception(error).Message));
                return;
            }

            int buffersInFile = (int)logfile.LogfileHeader.BuffersWritten;
            bool liveTrace = !File.GetAttributes(LogFileName).HasFlag(FileAttributes.Archive);
            if (liveTrace || ((LogFileMode)logfile.LogfileHeader.LogFileMode).HasFlag(LogFileMode.FILE_MODE_CIRCULAR))
            {
                buffersInFile = (int)((new FileInfo(LogFileName).Length) / logfile.LogfileHeader.BufferSize);
                infos.Add(new Info("BuffersInFile", buffersInFile));
            }
            infos.Add(new Info("BootTime", DateTime.FromFileTime(logfile.LogfileHeader.BootTime)));
            infos.Add(new Info("StartTime", DateTime.FromFileTime(logfile.LogfileHeader.StartTime)));
            if (liveTrace)
            {
                infos.Add(new Info("Live Trace", true));
            }
            else
            {
                infos.Add(new Info("EndTime", DateTime.FromFileTime(logfile.LogfileHeader.EndTime)));
            }
            if (logfile.LogfileHeader.EventsLost != 0 || logfile.LogfileHeader.BuffersLost != 0)
            {
                infos.Add(new Info("EventsLost", logfile.LogfileHeader.EventsLost));
                infos.Add(new Info("BuffersLost", logfile.LogfileHeader.BuffersLost));
            }
            infos.Add(new Info("BuffersWritten", logfile.LogfileHeader.BuffersWritten));
            infos.Add(new Info("BufferSize", logfile.LogfileHeader.BufferSize));
            infos.Add(new Info("LogFileMode", (LogFileMode)logfile.LogfileHeader.LogFileMode));
            infos.Add(new Info("Clock", (Clock)logfile.LogfileHeader.ReservedFlags));
            infos.Add(new Info("Version", logfile.LogfileHeader.Version & 0xff));
            infos.Add(new Info("Build", logfile.LogfileHeader.ProviderVersion));

            bufferRead = 0;
            bufferViewStart = buffersInFile - buffersViewMax;
            int res = Native.ProcessTrace(new[] { handle }, 1, IntPtr.Zero, IntPtr.Zero);
            Native.CloseTrace(handle);
        }

        int bufferViewStart;
        [AllowReversePInvokeCalls]
        public unsafe void Deserialize(EVENT_RECORD* eventRecord)
        {
            if (eventRecord->Id == 65534 || bufferRead > bufferViewStart)
            {
                deserializer.Deserialize(eventRecord);
            }
        }

        int bufferRead;
        [AllowReversePInvokeCalls]
        public bool BufferCallback(IntPtr p)
        {
            bufferRead++;
            return true;
        }
    }
}
