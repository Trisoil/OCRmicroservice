using System;
using System.ServiceProcess;
using log4net;
using System.Threading;

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

        /// <summary>
        /// Stop service
        /// </summary>
        protected override void OnStop()
        {
            log.Info("Stop APP OCR microservice");
            oThread.Abort();
        }

        /// <summary>
        /// Create the manager which is called by the main Thread
        /// </summary>
        private void Execution()
        {
            manager = new Manager(log);
            manager.Start();
        }
    }
}
