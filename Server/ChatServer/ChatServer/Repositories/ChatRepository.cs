using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using ChatServer.Models;

namespace ChatServer.Repositories
{
    public class ChatRepository : IChatRepository
    {
        public readonly IConfiguration _configuration;

        public ChatRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        //handleText
        private string PadQuotes(string s)
        {
            if (s.IndexOf("\\") != -1)
                s = s.Replace("\\", @"\\");

            if (s.IndexOf("\n\r") != -1)
                s = s.Replace("\n\r", @"\n");

            if (s.IndexOf("\r") != -1)
                s = s.Replace("\r", @"\r");

            if (s.IndexOf("\n") != -1)
                s = s.Replace("\n", @"\n");

            if (s.IndexOf("\t") != -1)
                s = s.Replace("\t", @"\t");

            if (s.IndexOf("\"") != -1)
                return s.Replace("\"", @"""");
            else
                return s;
        }


        //GetAnswer
        public async Task<string> SendMsg(Message message)
        {
            try
            {
                string OPENAI_API_KEY = "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"; // 
                string maxToken = "2048";
                string temperature = "0.5";
                System.Net.ServicePointManager.SecurityProtocol =
                  System.Net.SecurityProtocolType.Tls12 |
                  System.Net.SecurityProtocolType.Tls11 |
                  System.Net.SecurityProtocolType.Tls;

                string apiEndpoint = "https://api.openai.com/v1/completions";
                var request = WebRequest.Create(apiEndpoint);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + OPENAI_API_KEY);

                int iMaxTokens = int.Parse(maxToken);

                double dTemperature = double.Parse(temperature); // 0.5


                string sUserId = "1"; // 1
                string sModel = "text-davinci-003";
                string messageSend = $"Give me some {message.MessageText} book and description about it";
                string data = "{";
                data += " \"model\":\"" + sModel + "\",";
                data += " \"prompt\": \"" + PadQuotes(messageSend) + "\",";
                data += " \"max_tokens\": " + iMaxTokens + ",";
                data += " \"user\": \"" + sUserId + "\", ";
                data += " \"temperature\": " + dTemperature + ", ";
                data += " \"frequency_penalty\": 0.0" + ", "; // Number between -2.0 and 2.0  Positive value decrease the model's likelihood to repeat the same line verbatim.
                data += " \"presence_penalty\": 0.0" + ", "; // Number between -2.0 and 2.0. Positive values increase the model's likelihood to talk about new topics.
                data += " \"stop\": [\"#\", \";\"]"; // Up to 4 sequences where the API will stop generating further tokens. The returned text will not contain the stop sequence.
                data += "}";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }


                var response = request.GetResponse();
                var streamReader = new StreamReader(response.GetResponseStream());
                string sJson = streamReader.ReadToEnd();
                Dictionary<string, object> oJson = (Dictionary<string, object>)JsonConvert.DeserializeObject<Dictionary<string, object>>(sJson);
                var oChoices = oJson["choices"];
                object objectResponse = (object)oChoices;
                var json  = JsonConvert.SerializeObject(objectResponse);
              
                List<Object> res = JsonConvert.DeserializeObject<List<Object>>(json);
                object textResponse = res[0];
                Dictionary<string, string> dictResponse = (Dictionary<string, string>)JsonConvert.DeserializeObject<Dictionary<string, string>>(textResponse.ToString());
                string Rresponse = dictResponse["text"];

                return Rresponse;
            }


            catch (WebException ex)
            {

                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string Fail404 = reader.ReadToEnd();
                    return Fail404;
                }


            }
        }
    }
}
