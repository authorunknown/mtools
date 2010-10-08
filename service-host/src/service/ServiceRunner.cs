using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Threading;

namespace service
{
    public class ServiceRunner
    {
        IServiceControl _service;
        Thread _waitThread;
        ExternalProcess _process;

        public ServiceRunner(IServiceControl service, string configFile)
        {
            this._service = service;
            this._waitThread = new Thread(() => Wait());
            this._process = new ExternalProcess();
            Config config = Config.ReadFrom(configFile);

            this._process.Arguments = config.Arguments;
            this._process.BufferSize = config.BufferSize;
            this._process.ImagePath = config.ImagePath;
            this._process.LogFile = config.LogFile;
            this._process.MaxLogSize = config.MaxLogSize;
            this._process.MaxNumberOfLogBackups = config.MaxNumberOfLogBackups;
            this._process.WorkingDirectory = config.WorkingDirectory;
            this._process.ReliableLogging = config.ReliableLogging;
        }

        private void Wait()
        {
            _process.WaitForExit();
        }

        public void Start()
        {
            _process.Start();
            _waitThread.Start();
        }

        public void Stop()
        {
            _process.Stop();
            _waitThread.Abort();
        }
    }
}
