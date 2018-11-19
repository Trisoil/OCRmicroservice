using Com.Paycasso.Divacs.Protocol;
using Confluent.Kafka;
using log4net;
using System;
using System.Threading.Tasks;
using Google.Protobuf;
using System.IO;
using System.Drawing;
using Google.Protobuf.WellKnownTypes;

namespace OCRmicroservice
{
    /// <summary>
    /// This is the Main class of the project. Here There are the Kafka consumer that get the  messages (protobuf) and
    /// OCR the images within Leadtools, then it send the response with the new information to Kafka
    /// </summary>
   public class Manager : IDisposable
    {
        #region "Properties"      
        private LeadToolsOCRManager OCR;
        private Response machineLearning;
        private OcrResponse ocrResponse;
        private Envelope request;
        private Envelope answer;
        private static ILog log;
        Boolean Activation;
        private string pathDirectoryApp;
        #endregion

        #region "Inititializations"
        public Manager(ILog Log)
        {
            log = Log;
            pathDirectoryApp = System.AppDomain.CurrentDomain.BaseDirectory;
            OCR = new LeadToolsOCRManager(log);
            Activation = true;
        }

        /// <summary>
        /// Start Consuming and OCR
        /// </summary>
        public void Start()
        {
            Consuming();
        }

        #endregion

        #region"Kafka Methods"
        /// <summary>
        /// Configure kafka consumer
        /// </summary>
        public void Consuming()
        {
            log.Info("Waiting new Kafka messages");
            ConsumerConfig Consumerconf = new Confluent.Kafka.ConsumerConfig
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

                while (consuming && Activation)
                {
                    try
                    {
                        var cr = c.Consume();

                        // do job
                        DoJob(cr.Value);
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
        /// Send a Message to the Kafka broker with type Envelope as jsonstring serialised by Protobuf
        /// </summary>
        /// <param name="envelope"></param>
        public async Task SendObject(Envelope envelope)
        {
            try
            {
                log.Info("Start Sending answer");
                var config = new ProducerConfig { BootstrapServers = Constants.KafkaBootstrapServers };
                using (var p = new Producer<Null, byte[]>(config))
                {
                    //for (int i = 0; i < 2; ++i)
                    //{
                        try
                        {
                         
                            var dr = await p.ProduceAsync(Constants.ProducerTopic, new Message<Null, byte[]> { Value = envelope.ToByteArray() });

                            //lock (log)
                            //{
                                log.Info("transactionID" + envelope.TransactionId+ " succesfully delivered");
                            //}                         
                        }
                        catch (KafkaException e)
                        {
                            lock (log)
                            {
                                log.Error($"Delivery failed to Kafka: {e.Error.Reason}");
                            }                           
                        }
                    //}
                }
            }
            catch(Exception ex)
            {
                log.Error("Errors sending the message to kafka. ", ex);
            }         
        }

        #endregion

        #region"Protobuf Methods"
        /// <summary>
        /// Converting Kafka mex to Protobuf Mex
        /// </summary>
        /// <param name="ProtobufMex"></param>
        public Boolean AnalyzeProtobufMex(ref byte[] ProtobufMex)
        {
          //  Response MachineLearning = new Response();
            try
            {
                log.Info("start to converting the kafka message to protobuf message");
                request = Envelope.Parser.ParseFrom(ProtobufMex);
                answer = request;
                log.Info("End succesfully conversion protobuf message");
                log.Info("TransactionID: " + request.TransactionId);
                log.Info("Country: " + request.OcrDocument.Country);
                log.Info("Language: " + request.OcrDocument.Language);
                log.Info("ROIS received:");
                foreach (var field in request.OcrDocument.Rois)
                {
                    log.Info("ROIS: Name " + field.Name + "; ROI Kind" + field.Kind+ "; X:" + field.X.ToString() + "; Y:" + field.Y.ToString() + "; Width:" + field.W.ToString() + "; Height:" + field.H.ToString());
                }
                
            }
            catch (Exception ex)
            {
                log.Error("Error converting Protobuf mex", ex);
                return false;
            }

            return true;
        }

        public void ConvertToAnswer()
        {
            try
            {
                //to do
                answer = new Envelope();
            }
            catch (Exception ex)
            {
                log.Error("errors",ex);
            }
        }

        #endregion

        #region "Methods"
        /// <summary>
        /// the jobs of the software
        /// </summary>
        /// <param name="ConsumedInput"></param>
        public void DoJob(byte[] ConsumedInput)
        {
            try
            {
                // convert the kafka message within protobuf     
                AnalyzeProtobufMex(ref ConsumedInput);

                //Start OCR
                ocrResponse = StartOCR();

                //Print result and save into Pyaload
                if (ocrResponse !=null)
                {
                    foreach (var field in ocrResponse.RoiValues)
                    {
                        log.Info("**OCR Result for: " + field.Key+ " => "+ field.Value);
                    }
                    answer.Payload.Add(Any.Pack(ocrResponse));
                }                

                //SendAnswer in background and continuos new jobs
                Task SendAnswer = SendObject(answer);
            }

            catch (Exception ex)
            {
                log.Error("errors during the work", ex);
            }
        }

        /// <summary>
        /// start ocr process
        /// </summary>
        /// <param name="NewTransaction"></param>
        public OcrResponse StartOCR()
        {
            OcrResponse _OCRResponse = new OcrResponse();
            try
            {
                //methods that call leadtool class and read every fields
                return _OCRResponse = OCR.PerformTargetedOcr( ref request);
            }
            catch (Exception ex)
            {
                log.Error("errors", ex);
            }
            return _OCRResponse;
        }

        /// <summary>
        /// Save the images to the TestImages folder if the software is in testing mode
        /// </summary>
        /// <param name="EnvelopeMessage"></param>
        //public void SaveImages(ref Com.Paycasso.Divacs.Protocol.Message EnvelopeMessage)
        //{
        //    try
        //    {
        //        log.Info("saving image received in the disk");
        //        var imageInByte = Convert.FromBase64String(EnvelopeMessage.ClassifyDocument.Parts);
        //        using (var mStream = new MemoryStream(imageInByte))
        //        {
        //            Image OriginalImage = Image.FromStream((Stream)mStream);
        //            DirectoryInfo TestImages = Directory.CreateDirectory(pathDirectoryApp + "TestImages");

        //            if (File.Exists(pathDirectoryApp+"TestImages\\" + EnvelopeMessage.TransactionId.ToString() + ".png"))
        //            {
        //                if (File.Exists(pathDirectoryApp + "TestImages\\" + EnvelopeMessage.TransactionId.ToString() + "_2.png"))
        //                {
        //                    OriginalImage.Save(pathDirectoryApp + "TestImages\\" + EnvelopeMessage.TransactionId.ToString() + "_3.png");
        //                }
        //                else
        //                    OriginalImage.Save(pathDirectoryApp + "TestImages\\" + EnvelopeMessage.TransactionId.ToString() + "_2.png");
        //            }
        //            else
        //                OriginalImage.Save(pathDirectoryApp + "TestImages\\" + EnvelopeMessage.TransactionId.ToString() + ".png");
        //        }
        //        log.Info("saved images succesfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("error saving the images received,", ex);
        //    }
        //}

        /// <summary>
        /// Shut down Manager OCR
        /// </summary>
        public void Dispose()
        {
            Activation = false;
            OCR.Dispose();
        }

        #endregion
    }
}
