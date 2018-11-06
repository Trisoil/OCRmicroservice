using Avro;
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Com.Paycasso.Divacs.Protocol;

namespace OCRmicroservice
{
    class KafkaProducer
    {
        private ProducerConfig conf;
        private   Action<DeliveryReportResult<Null, string>> handler;

        /// <summary>
        /// Constructor inizialize the kafka producer
        /// </summary>
        public KafkaProducer()
        {
            conf = new ProducerConfig { BootstrapServers = "localhost:9092" };

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
            using (var p = new Producer<Null, string>(conf))
            {
                for (int i = 0; i < 2; ++i)
                {
                    p.BeginProduce("PaoloTest", new Message<Null, string> { Value = envelope.ToString() }, handler);
                }
                // wait for up to 10 seconds for any inflight messages to be delivered.
                p.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}
