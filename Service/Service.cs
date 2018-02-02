using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.InteropServices;

namespace Service
{
    public partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
            StartLogging();
        }

        private void StartLogging()
        {
            if (eventLog == null) { 
                eventLog = new EventLog();
            }
            if (!System.Diagnostics.EventLog.SourceExists("ServiceLog"))
            {
                System.Diagnostics.EventLog.CreateEventSource("CustomService", "ServiceLog");
            }
            eventLog.Source = "CustomService";
            eventLog.Log = "ServiceLog";
        }

        protected override void OnStart(string[] args)
        {
            StartLogging();
            // Update the service state to Start Pending
            ServiceStatus serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.SERVICE_START_PENDING,
                dwWaitHint = 100000
            };
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventLog.WriteEntry("Custom Service Started", EventLogEntryType.Information);

            // Update the service state to Running
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            // Update the service state to stop pending  
            ServiceStatus serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.SERVICE_STOP_PENDING,
                dwWaitHint = 100000
            };
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventLog.WriteEntry("Service Stopped", EventLogEntryType.Information);

            // Update the service state to Stopped
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };
    }
}
