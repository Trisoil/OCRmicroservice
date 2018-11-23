using Google.Protobuf;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OCRmicroservice
{
   public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
         
        }

        public static void TestMain()
        {
            ILog log;
            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger(typeof(Program));
            log.Info("Start APP OCR microservice");
            Manager manager = new Manager(log);
            manager.Start();
            Task SendAnswer = manager.SendObject(FakeEnvelope2());
        }

        public static Com.Paycasso.Divacs.Protocol.Envelope FakeEnvelope2()

        {
            Com.Paycasso.Divacs.Protocol.Envelope fake = new Com.Paycasso.Divacs.Protocol.Envelope();
            fake.OcrDocument = new OcrDocument();
            fake.OcrDocument.Country = Country.Ltu;
            fake.OcrDocument.Language = "English";
            OcrRoi barcode = new OcrRoi();
            barcode.Kind = RoiKind.Barcode;
            barcode.Name = "ROI_BARCODE";
            barcode.X = 1;
            barcode.Y = 1;
            barcode.W = 1122;
            barcode.H = 238;



            fake.OcrDocument.Rois.Add(barcode);


            Image image = Image.FromFile("C:\\Users\\PaoloCiliberto\\Pictures\\kentucky\\AAMVA.jpg");
            var ms = new System.IO.MemoryStream();
            image.Save(ms, image.RawFormat);
            //ms.ToArray();


            // fake.OcrDocument.Image = ByteString.CopyFrom(ms.ToArray());


            fake.OcrDocument.Image = ByteString.CopyFrom(ms.ToArray());

            return fake;
        }
    }
}
