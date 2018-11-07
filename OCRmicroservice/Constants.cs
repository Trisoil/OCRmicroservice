using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace OCRmicroservice
{
    /// <summary>
    /// This class call the variables set in the app config
    /// </summary>
    public class Constants
    {

        public static string GroupID
        {
            get
            {
                return ConfigurationManager.AppSettings["GroupID"].ToString();
            }
        }

        public static string ConsumerTopic
        {
            get
            {
                return ConfigurationManager.AppSettings["ConsumerTopic"].ToString();
            }
        }

        public static string ProducerTopic
        {
            get
            {
                return ConfigurationManager.AppSettings["ProducerTopic"].ToString();
            }
        }

        public static Boolean TestingMode
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["TestingMode"].ToLower());
            }
        }
        public static string KafkaBootstrapServers
        {
            get
            {
                return ConfigurationManager.AppSettings["KafkaBootstrapServers"].ToString();
            }
        }
    }
}
