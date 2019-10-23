/*********************************************************************************************
 * 
 * This demonstration application gives a very simple example of how to get count data from 
 * Estate Manager using the REST API.
 * 
 * The code below should be used with the API documentation provided within Estate Manager.
 * 
 * The following code is not an exhaustive list of the capabilities of the REST API and is only
 * meant as a demonstration on how to get up and running.
 * 
 * 
 * 
 * *******************************************************************************************/

namespace EM_REST_API
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.IO;
    using System.Net;

    /// <summary>
    /// Demonstration of how to get count data from Estate Manager
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Base address of the EM instance
            string baseAddress = "https://<EM address goes here>/";

            // Web API key for EM instance (can be found on the My Account page)
            string webApiKey = "<Web API Key goes here>";

            // Get a list of the first 1000 devices in the system and then collect the last 10 counts from each device
            var devices = GetListOfDevices(baseAddress, webApiKey);

            foreach (var device in devices)
            {
                // Get the serial number of the device
                var deviceSerial = (string)device["FriendlySerial"];

                // Get data out of the system
                var countData = GetPageOfCounts(baseAddress, webApiKey, deviceSerial);

                // Display the data
                Console.WriteLine("Got data for device " + deviceSerial);

                // Loop around each log entry
                foreach (var logEntry in countData)
                {
                    // Extract the timestamp and the log entry ID, all timestamps will be in UTC
                    var timestamp = (string)logEntry["Timestamp"];
                    var logEntryId = (long)logEntry["LogEntryId"];

                    Console.WriteLine("Timestamp " + timestamp + " entry id " + logEntryId);

                    // Loop around the registers
                    foreach (var count in logEntry["RegisterCountLogs"])
                    {
                        // Name of the register
                        var name = (string)count["Name"];

                        // Register index
                        var index = (long)count["Index"];

                        // Count value for the register
                        var countValue = (long)count["Value"];

                        Console.WriteLine("Register name: " + name + " Index: " + index + " value: " + countValue);
                    }
                }

                Console.WriteLine("Collection for device " + deviceSerial + " complete");
            }
        }

        /// <summary>
        /// Get the details of the first 1000 devices registered in EM
        /// </summary>
        /// <param name="baseAddress">EM base address</param>
        /// <param name="webApiKey">Web API Key</param>
        /// <returns></returns>
        static JToken GetListOfDevices(string baseAddress, string webApiKey)
        {
            string data;

            string url = baseAddress + "api/v1/device/list?page=1&pageSize=1000&apiKey=" + webApiKey;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.MediaType = "application/json";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "GET";

            using (var response = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        data = streamReader.ReadToEnd();
                    }
                }
            }

            return JToken.Parse(data);
        }

        /// <summary>
        /// Gets the most recent 10 log entries from a device
        /// </summary>
        /// <param name="baseAddress">Base address of the device</param>
        /// <param name="webApiKey">Web API Key</param>
        /// <param name="deviceSerial">Serial number of the device we want the data from</param>
        /// <returns></returns>
        static JToken GetPageOfCounts(string baseAddress, string webApiKey, string deviceSerial)
        {
            string data;

            string url = baseAddress + "api/v1/device/countlogs/" + deviceSerial + "?page=1&pageSize=10&apiKey=" + webApiKey;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.MediaType = "application/json";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "GET";

            using (var response = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        data = streamReader.ReadToEnd();
                    }
                }
            }

            return JToken.Parse(data);
        }
    }
}
