using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Configuration;

namespace service
{
    public partial class Service1 : ServiceBase, IServiceControl
    {
        ServiceRunner _runner;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string serviceConfigFile = ConfigurationManager.AppSettings["ServiceDefinition"];
            _runner = new ServiceRunner(this, serviceConfigFile);
            _runner.Start();
        }

        protected override void OnStop()
        {
            _runner.Stop();
        }
    }
}
