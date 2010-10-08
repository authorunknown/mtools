using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ServiceProcess;

namespace service
{
    public class ExternalProcess
    {
        Process _process;
        bool _running;
        Log _log;

        public string ImagePath { get; set; }
        public string WorkingDirectory { get; set; }
        public string[] Arguments { get; set; }
        public int MaxNumberOfLogBackups { get; set; }
        public int BufferSize { get; set; }
        public string LogFile { get; set; }
        public long MaxLogSize { get; set; }
        public bool ReliableLogging { get; set; }

        public void Start()
        {
            if (_running)
                throw new InvalidOperationException("Process is already running");

            if (!string.IsNullOrEmpty(LogFile))
            {
                _log = new Log(LogFile, BufferSize, ReliableLogging);
                _log.MaxNumberBackups = MaxNumberOfLogBackups;
                _log.MaxSize = MaxLogSize;
            }

            _process = new Process();
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.FileName = ImagePath;
            _process.StartInfo.Arguments = Arguments
                .Select(a => a.Replace("\"", "\\\""))
                .Select(a => a.Contains(" ") ? "\"" + a + "\"" : a)
                .Aggregate(new StringBuilder(), (sb, a) => sb.Append(a).Append(' '))
                .ToString()
                .Trim();
            _process.StartInfo.WorkingDirectory = WorkingDirectory;

            _process.ErrorDataReceived += new DataReceivedEventHandler(_process_OutputDataReceived);
            _process.OutputDataReceived += new DataReceivedEventHandler(_process_OutputDataReceived);
            _process.Exited += new EventHandler(_process_Exited);
            _process.Start();
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
            _running = true;
        }

        void _process_Exited(object sender, EventArgs e)
        {
            if (_log != null)
                _log.Flush();
            _running = false;
        }

        public void Stop()
        {
            if (!_running)
                return;

            try
            {
                _process.CancelErrorRead();
                _process.ErrorDataReceived -= new DataReceivedEventHandler(_process_OutputDataReceived);
                _process.CancelOutputRead();
                _process.OutputDataReceived -= new DataReceivedEventHandler(_process_OutputDataReceived);
                _process.Exited -= new EventHandler(_process_Exited);
                _process.Kill();
                _running = false;
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine();
                    sb.Append("ERROR: Exception trying to kill ");
                    sb.Append(ImagePath);
                    sb.AppendLine();
                    sb.AppendLine(ex.ToString());

                    _log.Write(sb.ToString());
                }
            }
            finally
            {
                if (_log != null)
                    _log.Flush();
            }
        }

        public void WaitForExit()
        {
            if (!_running)
                return;

            _process.WaitForExit();
        }

        void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_log != null)
                _log.WriteLine(e.Data);
        }

        void _process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_log != null)
                _log.WriteLine(e.Data);
        }
    }
}
