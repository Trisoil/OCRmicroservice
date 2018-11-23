using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using OCRmicroservice;
using System.IO;
using Google.Protobuf;

namespace OCRmicroservice.Tests
{
    [TestClass()]
    public class UnitTest1
    {

        [TestMethod()]
        public void TestMainTest()
        {
            ILog log;
            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger(typeof(Program));
            log.Info("Start APP OCR microservice");
            Manager manager = new Manager(log);
            manager.Start();
            Assert.Fail();
        }

        [TestMethod()]
        public void SendObjectTest()
        {
            ILog log;
            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger(typeof(Program));
            log.Info("Start APP OCR microservice");
            Manager manager = new Manager(log);
            Task SendAnswer = manager.SendObject(FakeEnvelopeAAMVA());
         //   Task SendAnswer2 = manager.SendObject(FakeEnvelope());
            manager.Start();
            //SendAnswer.Wait();

        }


        public static Com.Paycasso.Divacs.Protocol.Envelope FakeEnvelope()

        {
            Com.Paycasso.Divacs.Protocol.Envelope fake = new Com.Paycasso.Divacs.Protocol.Envelope();
            fake.OcrDocument = new OcrDocument();
            fake.OcrDocument.Country = Country.Ltu;
            fake.OcrDocument.Language = "Lithuanian";
            OcrRoi barcode = new OcrRoi();
            barcode.Kind = RoiKind.Barcode;
            barcode.Name = "ROI_BARCODE";
            barcode.X = 209;
            barcode.Y = 133;
            barcode.W = 414;
            barcode.H = 83;


            OcrRoi mrz = new OcrRoi();
            mrz.Kind = RoiKind.Mrz;
            mrz.Name = "ROI_MRZ";
            mrz.X = 27;
            mrz.Y = 322;
            mrz.W = 723;
            mrz.H = 139;



            OcrRoi number = new OcrRoi();
            number.Kind = RoiKind.Text;
            number.Name = "ROI_LTU_ISSUE_DATE_AND_AUTHORITY";
            number.X = 230;
            number.Y = 58;
            number.W = 484;
            number.H = 80;



            fake.OcrDocument.Rois.Add(barcode);
            fake.OcrDocument.Rois.Add(mrz);
            fake.OcrDocument.Rois.Add(number);


            Image image = Image.FromFile("c:\\Users\\PaoloCiliberto\\source\\repos\\OCRmicroservice\\OCRmicroservice_Test\\TestImages\\LTU_BO_02002_back.jpg");
            var ms = new MemoryStream();
            image.Save(ms, image.RawFormat);
            //ms.ToArray();


           // fake.OcrDocument.Image = ByteString.CopyFrom(ms.ToArray());


            fake.OcrDocument.Image  = ByteString.CopyFrom(ms.ToArray());

            return fake;
        }

        public static Com.Paycasso.Divacs.Protocol.Envelope FakeEnvelopeBroken()

        {
            Com.Paycasso.Divacs.Protocol.Envelope fake = new Com.Paycasso.Divacs.Protocol.Envelope();
            fake.OcrDocument = new OcrDocument();
            fake.OcrDocument.Country = Country.Ltu;
            fake.OcrDocument.Language = "Lithuanian";
            OcrRoi barcode = new OcrRoi();
            barcode.Kind = RoiKind.Barcode;
            barcode.Name = "ROI_BARCODE";
            barcode.X = 209;
            barcode.Y = 133;
            barcode.W = 414;
            barcode.H = 83;


            OcrRoi mrz = new OcrRoi();
            mrz.Kind = RoiKind.Mrz;
            mrz.Name = "ROI_MRZ";
            mrz.X = 27;
            mrz.Y = 322;
            mrz.W = 723;
            mrz.H = 139;



            OcrRoi number = new OcrRoi();
            number.Kind = RoiKind.Text;
            number.Name = "ROI_LTU_ISSUE_DATE_AND_AUTHORITY";
            number.X = 230;
            number.Y = 58;
            number.W = 484;
            number.H = 8000;



            fake.OcrDocument.Rois.Add(barcode);
            fake.OcrDocument.Rois.Add(mrz);
            fake.OcrDocument.Rois.Add(number);


            Image image = Image.FromFile("c:\\Users\\PaoloCiliberto\\source\\repos\\OCRmicroservice\\OCRmicroservice_Test\\TestImages\\LTU_BO_02002_back.jpg");
            var ms = new MemoryStream();
            image.Save(ms, image.RawFormat);
            //ms.ToArray();


            // fake.OcrDocument.Image = ByteString.CopyFrom(ms.ToArray());


            fake.OcrDocument.Image = ByteString.CopyFrom(ms.ToArray());

            return fake;
        }


        public static Com.Paycasso.Divacs.Protocol.Envelope FakeEnvelopeAAMVA()

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
            var ms = new MemoryStream();
            image.Save(ms, image.RawFormat);
            //ms.ToArray();

            // fake.OcrDocument.Image = ByteString.CopyFrom(ms.ToArray());


            fake.OcrDocument.Image = ByteString.CopyFrom(ms.ToArray());

            return fake;
        }

    }
}
