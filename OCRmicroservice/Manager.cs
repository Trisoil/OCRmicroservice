using Com.Paycasso.Divacs.Protocol;
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRmicroservice
{
    class Manager
    {
        private ConsumerConfig Consumerconf;
        private ProducerConfig Producerconf;
        private Action<DeliveryReportResult<Null, string>> handler;
        public Manager()
        {
            //configuration kfka consumer
            Consuming();
            Producer();
        }

        /// <summary>
        /// Configure kafka consumer
        /// </summary>
        public void Consuming()
        {
            Consumerconf = new Confluent.Kafka.ConsumerConfig
            {
                GroupId = "test-consumer-group",
                BootstrapServers = "localhost:9092",
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // eariest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetResetType.Earliest
            };
            using (var c = new Consumer<Ignore, string>(Consumerconf))
            {
                //topic
                c.Subscribe("PaoloTest");

                bool consuming = true;
                // The client will automatically recover from non-fatal errors. You typically
                // don't need to take any action unless an error is marked as fatal.
                c.OnError += (_, e) => consuming = !e.IsFatal;

                while (consuming)
                {
                    try
                    {
                        var cr = c.Consume();
                        Envelope NewTransactio = Envelope.Parser.ParseJson(cr.Value);
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occured: {e.Error.Reason}");
                    }
                }

                // Ensure the consumer leaves the group cleanly and final offsets are committed.
                c.Close();
            }
        }


        /// <summary>
        /// Configure Kafka Producer
        /// </summary>
        public void Producer()
        {
            Producerconf = new ProducerConfig { BootstrapServers = "localhost:9092" };

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
                    p.BeginProduce("PaoloTest", new Message<Null, string> { Value = envelope.ToString() }, handler);
                }
                // wait for up to 10 seconds for any inflight messages to be delivered.
                p.Flush(TimeSpan.FromSeconds(10));
            }
        }


        /// <summary>
        /// start ocr process
        /// </summary>
        /// <param name="NewTransaction"></param>
        public void StartOCR(Envelope NewTransaction)
        {
            Com.Paycasso.Divacs.Protocol.Message EnvelopeMessage = Com.Paycasso.Divacs.Protocol.Message.Parser.ParseFrom(NewTransaction.Payload.Value);
            
        }
    }
}
