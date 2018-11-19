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

        //[TestMethod()]
        //public void PerformTargetedOcrTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void TestMainTest()
        //{
        //    ILog log;
        //    log4net.Config.XmlConfigurator.Configure();
        //    log = LogManager.GetLogger(typeof(Program));
        //    log.Info("Start APP OCR microservice");
        //    Manager manager = new Manager(log);
        //    manager.Start();
        //    Assert.Fail();
        //}

        [TestMethod()]
        public void SendObjectTest()
        {
            ILog log;
            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger(typeof(Program));
            log.Info("Start APP OCR microservice");
            Manager manager = new Manager(log);
          //  Task SendAnswer = manager.SendObject(FakeEnvelopeBroken());
         //   Task SendAnswer2 = manager.SendObject(FakeEnvelope());
            manager.Start();
            //SendAnswer.Wait();

        }

        //public Com.Paycasso.Divacs.Protocol.Envelope GenerateEnvelope()
        //{
        //    return new Com.Paycasso.Divacs.Protocol.Envelope();
        //}

        //[TestMethod()]
        //public void ConsumingTest()
        //{
        //    ILog log;
        //    log4net.Config.XmlConfigurator.Configure();
        //    log = LogManager.GetLogger(typeof(Program));
        //    log.Info("Start APP OCR microservice");
        //    Manager manager = new Manager(log);
            
           

        //}

        //[TestMethod()]
        //public void Consuming( Offset offset)
        //{
        //  //  log.Info("Waiting new Kafka messages");
        //    ConsumerConfig Consumerconf = new Confluent.Kafka.ConsumerConfig
        //    {
        //        GroupId = Constants.GroupID,
        //        BootstrapServers = "localhost:9092",
        //        // Note: The AutoOffsetReset property determines the start offset in the event
        //        // there are not yet any committed offsets for the consumer group for the
        //        // topic/partitions of interest. By default, offsets are committed
        //        // automatically, so in this example, consumption will only start from the
        //        // eariest message in the topic 'my-topic' the first time you run the program.
        //        AutoOffsetReset = AutoOffsetResetType.Earliest
        //    };
        //    using (var consumer = new Consumer<Ignore, byte[]>(Consumerconf))
        //    {
              
        //        bool consuming = true;

        //        // The client will automatically recover from non-fatal errors. You typically
        //        // don't need to take any action unless an error is marked as fatal.
        //        consumer.OnError += (_, e) => consuming = !e.IsFatal;

        //        while (consuming && true)
        //        {
        //            try
        //            {
        //                var cr = consumer.Consume();

        //                // do job
        //               // DoJob(cr.Value);
        //            }
        //            catch (ConsumeException e)
        //            {
        //              //  log.Error($"Error occured receiving kafka mex: {e.Error.Reason}");
        //            }
        //        }
        //        // Ensure the consumer leaves the group cleanly and final offsets are committed.
        //        consumer.Close();
        //    }
        //}

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
    }
}
