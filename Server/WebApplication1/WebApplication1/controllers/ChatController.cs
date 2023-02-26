using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace WebApplication1.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<string>> CreateDiscount([FromBody] string message)
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
            string sModel = " text-davinci-003";
            string messageSend = $"Give me some {message} book and description about it";
            string data = "{";
            data += " \"model\":\"" + sModel + "\",";
            data += " \"prompt\": \"" + messageSend + "\",";
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

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var responseStream = new StreamReader(response.GetResponseStream()))
            {
                string sJson = responseStream.ReadToEnd();
                // Return sJson

                Dictionary<string, object> oJson = (Dictionary<string, object>)JsonConvert.DeserializeObject(sJson);
                Object[] oChoices = (Object[])oJson["choices"];
                Dictionary<string, object> oChoice = (Dictionary<string, object>)oChoices[0];
                string sResponse = (string)oChoice["text"];
                return sResponse;

            }
        }
    }
}
