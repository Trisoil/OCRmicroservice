using Com.Paycasso.Divacs.Protocol;
using Confluent.Kafka;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Collections;

using System.Threading;
using System.IO;
using System.Drawing;

namespace OCRmicroservice
{
    class Manager
    {
        private ConsumerConfig Consumerconf;
        private ProducerConfig Producerconf;
        private Action<DeliveryReportResult<Null, string>> handler;
        private LeadToolsOCRManager OCR;

        static private ILog log;
        public Manager()
        {
            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger(typeof(Program));
            log.Info("Start OCR");
            OCR = new LeadToolsOCRManager(log);
            //configuration kfka consumer

            Producer();

            // warning Consumer will stay in loop for ever
            Consuming();
        }

        /// <summary>
        /// Configure kafka consumer
        /// </summary>
        public void Consuming()
        {
            log.Info("Start waiting new messages");
            Consumerconf = new Confluent.Kafka.ConsumerConfig
            {
                GroupId = Constants.GroupID,
                BootstrapServers = Constants.KafkaBootstrapServers,
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // eariest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetResetType.Earliest
            };
            using (var c = new Consumer<Ignore, byte[]>(Consumerconf))
            {
                //topic
                c.Subscribe(Constants.ConsumerTopic);

                bool consuming = true;
                // The client will automatically recover from non-fatal errors. You typically
                // don't need to take any action unless an error is marked as fatal.
                c.OnError += (_, e) => consuming = !e.IsFatal;

                while (consuming)
                {
                    try
                    {
                        var cr = c.Consume();

                        if (cr.Value!= null)
                        {
                            AnalyzeProtobufMex(cr.Value);
                        }
                        
                    }
                    catch (ConsumeException e)
                    {
                        log.Error($"Error occured receiving kafka mex: {e.Error.Reason}");
                    }
                }

                // Ensure the consumer leaves the group cleanly and final offsets are committed.
                c.Close();
            }
        }


        /// <summary>
        /// Converting Kafka mex to Protobuf Mex
        /// </summary>
        /// <param name="ProtobufMex"></param>
        public void AnalyzeProtobufMex(byte[] ProtobufMex)
        {
            try
            {
              //  byte[] array = pro;
                log.Info("New Transaction is started to be consumed");
                Com.Paycasso.Divacs.Protocol.Envelope NewTransaction = Com.Paycasso.Divacs.Protocol.Envelope.Parser.ParseFrom(ProtobufMex);
                Com.Paycasso.Divacs.Protocol.Message EnvelopeMessage = Com.Paycasso.Divacs.Protocol.Message.Parser.ParseFrom(NewTransaction.Payload.Value);

                if (Constants.TestingMode) // this save the original image just if OCR is set to testing mode
                {
                    SaveImages(ref EnvelopeMessage);
                }

                //to do create response from Envelope
                Response MachineLearning = new Response();

                // new transaction
                StartOCR(MachineLearning);
            }
            catch(Exception ex)
            {
                log.Error("Error converting Protobuf mex", ex);
            }
        }

        /// <summary>
        /// Configure Kafka Producer
        /// </summary>
        public void Producer()
        {
            Producerconf = new ProducerConfig { BootstrapServers = Constants.KafkaBootstrapServers };

            handler = r =>
                Console.WriteLine(!r.Error.IsError
                    ? $"Delivered message to {r.TopicPartitionOffset}"
                    : $"Delivery Error: {r.Error.Reason}");
          
        }

        /// <summary>
        /// Send a Message to the Kafka broker with type Envelope as jsonstring serialised by Protobuf
        /// </summary>
        /// <param name="envelope"></param>
        public void SendObject(Envelope envelope)
        {
            using (var p = new Producer<Null, string>(Producerconf))
            {
                for (int i = 0; i < 2; ++i)
                {
                    p.BeginProduce(Constants.ProducerTopic, new Message<Null, string> { Value = envelope.ToString() }, handler);
                }
                // wait for up to 10 seconds for any inflight messages to be delivered.
                p.Flush(TimeSpan.FromSeconds(10));
            }
        }


        /// <summary>
        /// start ocr process
        /// </summary>
        /// <param name="NewTransaction"></param>
        public void StartOCR(Response machineLearning)
        {
            OCRResponse _OCRResponse = new OCRResponse();
            try
            {
                //methods that call leadtool class and read every fields
                _OCRResponse = OCR.PerformTargetedOcr(machineLearning);
            }
            catch (Exception ex)
            {
                log.Error("errors", ex);
            }
           // return _OCRResponse;
        }


        /// <summary>
        /// Save the images to the TestImages folder if the software is in testing mode
        /// </summary>
        /// <param name="EnvelopeMessage"></param>
        public void SaveImages(ref Com.Paycasso.Divacs.Protocol.Message EnvelopeMessage)
        {
            try
            {
                var imageInByte = Convert.FromBase64String(EnvelopeMessage.ClassifyDocument.Parts);
                using (var mStream = new MemoryStream(imageInByte))
                {
                    Image OriginalImage = Image.FromStream((Stream)mStream);
                    DirectoryInfo TestImages = Directory.CreateDirectory(Environment.CurrentDirectory + "\\TestImages");

                    if (File.Exists("TestImages\\" + EnvelopeMessage.TransactionId.ToString() + ".png"))
                    {
                        if (File.Exists("TestImages\\" + EnvelopeMessage.TransactionId.ToString() + "_2.png"))
                        {
                            OriginalImage.Save("TestImages\\" + EnvelopeMessage.TransactionId.ToString() + "_3.png");
                        }
                        else
                            OriginalImage.Save("TestImages\\" + EnvelopeMessage.TransactionId.ToString() + "_2.png");
                    }
                    else
                        OriginalImage.Save("TestImages\\" + EnvelopeMessage.TransactionId.ToString() + ".png");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

    }
}
