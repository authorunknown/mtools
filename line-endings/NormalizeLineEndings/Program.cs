using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using href.Utils;

namespace NormalizeLineEndings
{
    class Program
    {
        public static int Main(string[] args)
        {
            bool failed = false;
            try
            {
                bool unix = false;
                List<FileInfo> files = new List<FileInfo>();
                if (args.Length == 0)
                {
                    PrintUsage();
                    return 1;
                }
                foreach (string arg in args)
                {
                    if (!string.IsNullOrEmpty(arg) && (arg.StartsWith("-") || arg.StartsWith("/")))
                    {
                        string sys = arg.Substring(1).ToLower();
                        if ("unix".StartsWith(sys))
                            unix = true;
                        else if (!"windows".StartsWith(sys))
                        {
                            PrintUsage();
                            return 1;
                        }
                    }
                    else
                        files.Add(new FileInfo(arg));
                }
                if (files.Count == 0)
                {
                    PrintUsage();
                    return 1;
                }

                char[] chars = new char[2040];
                foreach (var file in files)
                {
                    try
                    {
                        if (file.Exists)
                        {
                            string tempFile = Path.GetTempFileName();
                            using (Stream fin = file.OpenRead())
                            using (StreamReader reader = EncodingTools.OpenTextStream(fin))
                            using (Stream fout = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                            using (StreamWriter writer = new StreamWriter(fout, reader.CurrentEncoding))
                            {
                                int c;
                                while ((c = reader.Read(chars, 0, chars.Length)) > 0)
                                {
                                    char prev = (char)0;
                                    for (int i = 0; i < c; i++)
                                    {
                                        char cc = chars[i];
                                        if (unix && cc != '\r')
                                            writer.Write(cc);
                                        else if (!unix)
                                        {
                                            if (cc == '\n' && prev != '\r')
                                            {
                                                writer.Write('\r');
                                                writer.Write('\n');
                                            }
                                            else
                                                writer.Write(cc);
                                            prev = cc;
                                        }
                                    }
                                }
                                writer.Flush();
                                fout.Flush();
                                writer.Close();
                                fout.Close();
                                reader.Close();
                                fin.Close();
                            }

                            string originalName = file.FullName;
                            string originalBackup = GetUniqueTempName(file);
                            file.MoveTo(originalBackup);
                            File.Move(tempFile, originalName);
                            File.Delete(originalBackup);

                            Console.Out.WriteLine(originalName + ": normalized to " + (unix ? "unix" : "windows") + " line endings");
                        }
                        else
                        {
                            Console.Out.WriteLine(file.FullName + " does not exist");
                        }
                    }
                    catch (Exception ex)
                    {
                        PrintException(ex);
                        failed = true;
                    }
                }
            }
            catch (Exception ex)
            {
                PrintException(ex);
                return 1;
            }
            return failed ? 1 : 0;
        }

        private static string GetUniqueTempName(FileInfo file)
        {
            string dir = Path.GetDirectoryName(file.FullName);
            string name = file.Name + "~";
            int i = 0;

            string rv = Path.Combine(dir, name);
            while (File.Exists(rv))
            {
                i++;
                rv = Path.Combine(dir, name) + i.ToString();
            }
            return rv;
        }

        private static void PrintException(Exception ex)
        {
            var w = System.Console.Error;

            while (ex != null)
            {
                w.Write(ex.GetType().FullName);
                w.Write(": ");
                w.Write(ex.Message);
                w.WriteLine();
                w.WriteLine(ex.StackTrace);
                ex = ex.InnerException;
                if (ex != null)
                    w.WriteLine("   --INNER EXCEPTION");
            }
        }

        static void PrintUsage()
        {
            Console.Error.WriteLine("usage: normalize [-unix|-win] file1 [file2 ...]");
        }
    }
}
