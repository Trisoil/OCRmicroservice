using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Threading;
using System.Threading.Tasks;

namespace OCRmicroservice
{
    public partial class Service1 : ServiceBase
    {
        private ILog log;
        private Manager manager;
        private Thread oThread;
        public Service1()
        {
            InitializeComponent();           
        }

        /// <summary>
        /// Start OCR service creating a new manager
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger(typeof(Program));
            log.Info("Start APP OCR microservice");
            oThread = new Thread(Execution);
            oThread.Name = "OCRmicroserviceThread";
            oThread.Priority = ThreadPriority.Highest;
            oThread.Start();
        }

        protected override void OnStop()
        {
            log.Info("Stop APP OCR microservice");
            oThread.Abort();
        }

        private void Execution()
        {
            manager = new Manager(log);
        }
    }
}
