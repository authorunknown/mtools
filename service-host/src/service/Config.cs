using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Configuration;

namespace service
{
    public class Config
    {
        public string ImagePath { get; set; }
        public string WorkingDirectory { get; set; }
        public string[] Arguments { get; set; }
        public int MaxNumberOfLogBackups { get; set; }
        public int BufferSize { get; set; }
        public string LogFile { get; set; }
        public long MaxLogSize { get; set; }
        public bool ReliableLogging { get; set; }

        public void WriteTo(string file)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.IndentChars = "  ";
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(file, settings))
            {
                writer.WriteStartElement("service");
                writer.WriteElementString("imagePath", this.ImagePath);
                writer.WriteElementString("workingDirectory", this.WorkingDirectory);
                writer.WriteStartElement("arguments");
                if (Arguments != null)
                {
                    foreach (string arg in this.Arguments)
                    {
                        writer.WriteElementString("arg", arg);
                    }
                }
                writer.WriteEndElement();
                writer.WriteElementString("maxNumberOfLogBackups", this.MaxNumberOfLogBackups.ToString());
                writer.WriteElementString("bufferSize", this.BufferSize.ToString());
                writer.WriteElementString("logFile", this.LogFile);
                writer.WriteElementString("maxLogSize", this.MaxLogSize.ToString());
                writer.WriteElementString("reliableLogging", this.ReliableLogging ? "true" : "false");
                writer.WriteEndElement();

                writer.Flush();
                writer.Close();
            }
        }

        public static Config ReadFrom(string file)
        {
            Config rv = new Config();
            XDocument xd = XDocument.Load(XmlReader.Create(file));
            foreach (var el in xd.Root.Elements())
            {
                switch (el.Name.LocalName.ToLower())
                {
                    case "imagepath":
                        rv.ImagePath = el.Value;
                        break;
                    case "workingdirectory":
                        rv.WorkingDirectory = el.Value;
                        break;
                    case "arguments":
                        rv.Arguments = el.Elements()
                            .Select(arg => arg.Value)
                            .ToArray();
                        break;
                    case "maxnumberoflogbackups":
                        rv.MaxNumberOfLogBackups = int.Parse(el.Value);
                        break;
                    case "buffersize":
                        rv.BufferSize = int.Parse(el.Value);
                        break;
                    case "logfile":
                        rv.LogFile = el.Value;
                        break;
                    case "maxlogsize":
                        rv.MaxLogSize = long.Parse(el.Value);
                        break;
                    case "reliablelogging":
                        rv.ReliableLogging = bool.Parse(el.Value);
                        break;
                    default:
                        throw new ConfigurationErrorsException("Unknown config property: " + el.Name.LocalName);
                }
            }
            return rv;
        }
    }
}
