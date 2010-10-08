using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Configuration;
using System.IO;
using System.Threading;

namespace service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0 && args[0].ToLower().StartsWith("-debug"))
            {
                TestLog();
                TestServiceRunner();
            }
            else if (args != null && args.Length > 0)
            {
                Config config = new Config();
                config.Arguments = new[] { "get", ".", "-recursive" };
                config.BufferSize = 1024;
                config.ImagePath = @"C:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE\TF.exe";
                config.LogFile = @"D:\foo.txt";
                config.MaxLogSize = 2048;
                config.MaxNumberOfLogBackups = 1;
                config.WorkingDirectory = @"D:\_tfs";
                try
                {
                    config.WriteTo(args[0]);
                    Console.Out.WriteLine("Wrote a sample config file to " + args[0] + ".  The full path to this file should be placed in the config file's <appSettings> section with the key 'ServiceDefinition'.");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
                    Console.Error.WriteLine();
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("usage: service-runner [path-to-write-config-file]");
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new Service1() 
                };
                ServiceBase.Run(ServicesToRun);
            }
        }

        private static void TestLog()
        {
            if (File.Exists(@"D:\foo.txt"))
                File.Delete(@"D:\foo.txt");
            Log log = new Log(@"D:\foo.txt", 0, true);
            log.MaxSize = 2048;
            log.MaxNumberBackups = 0;
            for (int i = 0; i < 1000; i++)
                log.Write("test " + i.ToString() + System.Environment.NewLine);
            log.Flush();
        }

        private static void TestServiceRunner()
        {
            string serviceConfigFile = ConfigurationManager.AppSettings["ServiceDefinition"];
            ServiceRunner runner = new ServiceRunner(new FakeServiceControl(), serviceConfigFile);
            runner.Start();
            Thread.Sleep(10000);
            runner.Stop();
        }

        private class FakeServiceControl : IServiceControl
        {
            #region IServiceControl Members

            public void Stop() { }

            #endregion
        }

    }

}