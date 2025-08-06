using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Web;
using static Google.Apis.Requests.BatchRequest;
using System.Threading.Tasks;

namespace BharatTouch.CommonHelper
{
    public class HuggingFaceTextGenerator
    {
        public static string apiUrl = "https://api-inference.huggingface.co/models/mistralai/Mistral-Nemo-Instruct-2407"; // Example model
        public static string apiKey = "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"; // Replace with your actual key

        public async Task<string> FetchHuggingFaceResponse(string inputText)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;


            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var inputData = $"{{\"inputs\": \"{inputText}\"}}";
                var content = new StringContent(inputData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return $"Error: {response.StatusCode}";
                }
            }
        }
    }



}