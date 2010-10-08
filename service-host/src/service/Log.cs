using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace service
{
    /// <summary>
    /// Thread safe log file writer that won't fill up a disk.
    /// </summary>
    public class Log
    {
        const int MaxRetries = 5;

        string _fileName;
        service.Buffer _buffer;
        Encoding _encoding;
        bool _reliable;

        private object syncBlock = new object();

        public Log(string fileName, int bufferSize, bool reliable)
        {
            this._fileName = fileName;
            this.MaxNumberBackups = 1;
            this.MaxSize = 10 * 1024 * 1024;
            if (bufferSize > 0)
                this._buffer = new Buffer(bufferSize);
            this._encoding = Encoding.UTF8;
            this._reliable = reliable;
        }

        /// <summary>
        /// Max size in bytes
        /// </summary>
        public long MaxSize { get; set; }

        /// <summary>
        /// Max number of log files that can exist
        /// </summary>
        public int MaxNumberBackups { get; set; }

        public void Flush()
        {
            try
            {
                bool haveLock = false;
                if (_buffer != null && _buffer.Count > 0)
                {
                    try
                    {
                        Monitor.Enter(syncBlock, ref haveLock);
                        CheckSizeAndRollOver();
                        using (Stream fout = OpenLog())
                        {
                            _buffer.Flush(fout);
                            fout.Flush();
                            fout.Close();
                        }
                    }
                    finally
                    {
                        if (haveLock)
                            Monitor.Exit(syncBlock);
                    }
                }
            }
            catch
            {
                if (_reliable)
                    throw;
            }
        }

        public void Write(string val)
        {
            if (string.IsNullOrEmpty(val))
                return;
            try
            {
                Write(_encoding.GetBytes(val));
            }
            catch
            {
                if (_reliable)
                    throw;
            }
        }

        public void WriteLine(string val)
        {
            try
            {
                string nl = Environment.NewLine;
                int size = 0;
                if (!string.IsNullOrEmpty(val))
                    size = _encoding.GetByteCount(val);
                size += _encoding.GetByteCount(nl);

                byte[] chunk = new byte[size];
                int i = 0;
                if (!string.IsNullOrEmpty(val))
                    i = _encoding.GetBytes(val, 0, val.Length, chunk, 0);
                _encoding.GetBytes(nl, 0, nl.Length, chunk, i);

                Write(chunk);
            }
            catch
            {
                if (_reliable)
                    throw;
            }
        }

        private void Write(byte[] chunk)
        {
            bool haveLock = false;
            try
            {
                Monitor.Enter(syncBlock, ref haveLock);
                CheckSizeAndRollOver();
                if (this._buffer == null)
                {
                    using (Stream fout = OpenLog())
                    {
                        fout.Write(chunk, 0, chunk.Length);
                        fout.Flush();
                        fout.Close();
                    }
                }
                else
                {
                    if (_buffer.CanHold(chunk.Length))
                    {
                        _buffer.Add(chunk);
                    }
                    else
                    {
                        using (Stream fout = OpenLog())
                        {
                            _buffer.Flush(fout);
                            fout.Write(chunk, 0, chunk.Length);
                            fout.Flush();
                            fout.Close();
                        }
                    }
                }
            }
            finally
            {
                if (haveLock)
                    Monitor.Exit(syncBlock);
            }
        }

        private void CheckSizeAndRollOver()
        {
            var info = new FileInfo(_fileName);
            if (info.Exists && info.Length >= this.MaxSize)
                Rollover();
        }

        private Stream OpenLog()
        {
            bool writeBOM = !File.Exists(_fileName);
            Stream fout = null;
            int i = 0;
            while (true)
            {
                try
                {
                    fout = new FileStream(_fileName, FileMode.Append, FileAccess.Write, FileShare.Read);
                    break;
                }
                catch (Exception)
                {
                    i++;
                    if (i == MaxRetries)
                        throw;
                    Thread.Sleep(250);
                }
            }
            if (writeBOM)
            {
                var byteOrderMarks = _encoding.GetPreamble(); //unicode thing
                if (byteOrderMarks != null && byteOrderMarks.Length > 0)
                    fout.Write(byteOrderMarks, 0, byteOrderMarks.Length);
            }
            return fout;
        }

        private void Rollover()
        {
            if (_buffer != null && _buffer.Count > 0)
            {
                using (Stream fout = OpenLog())
                {
                    _buffer.Flush(fout);
                }
            }

            if (MaxNumberBackups < 1)
            {
                File.Delete(_fileName);
                return;
            }

            string nm = _fileName + "." + MaxNumberBackups.ToString();
            if (File.Exists(nm))
                Delete(nm, MaxRetries);
            for (int i = MaxNumberBackups - 1; i > 0; i--)
            {
                nm = _fileName + "." + i.ToString();
                if (File.Exists(nm))
                    Move(nm, _fileName + "." + (i + 1).ToString(), MaxRetries);
            }
            Move(_fileName, _fileName + ".1", MaxRetries);
        }

        private void Delete(string file, int maxRetries)
        {
            int i = 0;
            while (true)
            {
                try
                {
                    File.Delete(file);
                    return;
                }
                catch (Exception)
                {
                    if (i < maxRetries)
                    {
                        i++;
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        throw;
                    }

                }
            }
        }

        private void Move(string source, string dest, int maxRetries)
        {
            int i = 0;
            while (true)
            {
                try
                {
                    File.Move(source, dest);
                    return;
                }
                catch (Exception)
                {
                    if (i < maxRetries)
                    {
                        i++;
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}
