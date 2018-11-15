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
                try
                {
                    return ConfigurationManager.AppSettings["GroupID"].ToString();
                }
                catch
                {
                    //this is just for unit test project
                    return "test-consumer-group";
                }
               
            }
        }

        public static string ConsumerTopic
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ConsumerTopic"].ToString();
                }
                
                catch 
                {
                    //this is just for unit test project
                    return "CE_Results";
                }
              
            }
        }

        public static string ProducerTopic
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ProducerTopic"].ToString();
                }
                catch
                {
                    //this is just for unit test project
                    return "OCR_Results";
                }
              
            }
        }

        public static Boolean TestingMode
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["TestingMode"].ToLower());
                }
                catch
                {
                    //this is just for unit test project
                    return false;
                }
                
            }
        }
        public static string KafkaBootstrapServers
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["KafkaBootstrapServers"].ToString();
                }
                catch
                {
                    //this is just for unit test project
                    return "localhost:9092";
                }            
            }
        }
    }
}
