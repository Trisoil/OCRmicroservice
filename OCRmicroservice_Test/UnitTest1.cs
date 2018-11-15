using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCRmicroservice;

namespace OCRmicroservice.Tests
{
    [TestClass()]
    public class UnitTest1
    {

        [TestMethod()]
        public void PerformTargetedOcrTest()
        {
            Assert.Fail();
        }

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
            Task SendAnswer = manager.SendObject(new Com.Paycasso.Divacs.Protocol.Envelope());
            SendAnswer.Wait();

        }

        public Com.Paycasso.Divacs.Protocol.Envelope GenerateEnvelope()
        {
            return new Com.Paycasso.Divacs.Protocol.Envelope();
        }

        [TestMethod()]
        public void ConsumingTest()
        {
            ILog log;
            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger(typeof(Program));
            log.Info("Start APP OCR microservice");
            Manager manager = new Manager(log);
            
           

        }

        [TestMethod()]
        public void Consuming( Offset offset)
        {
          //  log.Info("Waiting new Kafka messages");
            ConsumerConfig Consumerconf = new Confluent.Kafka.ConsumerConfig
            {
                GroupId = Constants.GroupID,
                BootstrapServers = "localhost:9092",
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // eariest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetResetType.Earliest
            };
            using (var consumer = new Consumer<Ignore, byte[]>(Consumerconf))
            {
                consumer.Assign(new List { new TopicPartitionOffset(topics.First(), 0, 0) });

                consumer.OnPartitionsAssigned += (obj, partitions) => {
                    Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}], member id: {consumer.MemberId}");
                    var fromBeginning = partitions.Select(p => new TopicPartitionOffset(p.Topic, p.Partition, Offset.Beginning)).ToList();
                    Console.WriteLine($"Updated assignment: [{string.Join(", ", fromBeginning)}]");
                    consumer.Assign(fromBeginning);
                };

                consumer.Subscribe(Constants.ConsumerTopic);
                List<Partition> partitions = consumer.Assignment();
                //Partition partition = consumer.Commit(default(CancellationToken));
                TopicPartitionOffset topic;
                //topic

                Partition partition = consumer.OnPartitionEOF();
                bool consuming = true;

                // The client will automatically recover from non-fatal errors. You typically
                // don't need to take any action unless an error is marked as fatal.
                consumer.OnError += (_, e) => consuming = !e.IsFatal;

                while (consuming && true)
                {
                    try
                    {
                        var cr = consumer.Consume();

                        // do job
                       // DoJob(cr.Value);
                    }
                    catch (ConsumeException e)
                    {
                      //  log.Error($"Error occured receiving kafka mex: {e.Error.Reason}");
                    }
                }
                // Ensure the consumer leaves the group cleanly and final offsets are committed.
                c.Close();
            }
        }
    }
}
