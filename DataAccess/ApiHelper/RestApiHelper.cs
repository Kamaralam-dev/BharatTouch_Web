using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ApiHelper
{
    public static class RestApiHelper
    {
        public static ResponseModel CallApi(string apiUrl, string action, Method httpMethod, Dictionary<string, string> dictList)
        {
            RestClient client = new RestClient(apiUrl);

            RestRequest request = new RestRequest(action, httpMethod);
            foreach (var dict in dictList)
            {
                request.AddQueryParameter(dict.Key, dict.Value);
            }
            var response = new RestResponse();
            Task.Run(async () =>
            {
                response = await GetResponseContentAsync(client, request) as RestResponse;
            }).Wait();
            return JsonConvert.DeserializeObject<ResponseModel>(response.Content);
        }

        public static ResponseModel CallApi(string apiUrl, string action, Method httpMethod, string jsonObject)
        {

            RestClient restClient = new RestClient(apiUrl);
            RestRequest request = new RestRequest(action, httpMethod);
            request.RequestFormat = DataFormat.Json;

            request.AddHeader("User-Agent", "Fiddler");
            request.AddParameter("application/json", jsonObject, ParameterType.RequestBody);

            var response = new RestResponse();
            Task.Run(async () =>
            {
                response = await GetResponseContentAsync(restClient, request) as RestResponse;
            }).Wait();

            return JsonConvert.DeserializeObject<ResponseModel>(response.Content);
        }

        public static Task<IRestResponse> GetResponseContentAsync(RestClient restClient, RestRequest restRequest)
        {
            var tcs = new TaskCompletionSource<IRestResponse>();
            restClient.ExecuteAsync(restRequest, response => {
                tcs.SetResult(response);
            });
            return tcs.Task;
        }

        public static bool Getconflict(string Message)
        {
            var Istrue = Message.Contains("The DELETE statement conflicted with the REFERENCE constraint");
            return Istrue;
        }

    }
}
