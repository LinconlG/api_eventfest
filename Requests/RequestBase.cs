using System.Collections;
using System.Net;
using System.Text;
using System.Text.Json;

namespace API_EventFest.Requests {
    public abstract class RequestBase {

        protected readonly HttpClient _httpClient;
        protected readonly IConfiguration _configuration;

        protected readonly string _apiEventFestUrl;
        public RequestBase(IConfiguration configuration) {
            _httpClient = new HttpClient();
            _configuration = configuration;
            _apiEventFestUrl = _configuration.GetConnectionString("apieventfest");
        }


        internal static HttpRequestMessage Get(string path, Dictionary<string, object> queryParams = null) {
            return new HttpRequestMessage() {
                RequestUri = new Uri(path + ParamsToString(queryParams)),
                Method = HttpMethod.Get,
            };
        }
        internal static HttpRequestMessage Put(string path, string content, Dictionary<string, object> queryParams = null) {
            return new HttpRequestMessage() {
                RequestUri = new Uri(path + ParamsToString(queryParams)),
                Method = HttpMethod.Put,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
        }
        internal static HttpRequestMessage Post(string path, string content, Dictionary<string, object> queryParams = null) {
            return new HttpRequestMessage() {
                RequestUri = new Uri(path + ParamsToString(queryParams)),
                Headers = {
                    { HttpRequestHeader.Authorization.ToString(), "No Auth"}
                },
                Method = HttpMethod.Post,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
        }
        internal static HttpRequestMessage Delete(string path, string content, Dictionary<string, object> queryParams = null) {
            return new HttpRequestMessage() {
                RequestUri = new Uri(path + ParamsToString(queryParams)),
                Method = HttpMethod.Delete,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
        }
        internal static HttpRequestMessage Patch(string path, string content, Dictionary<string, object> queryParams = null) {
            return new HttpRequestMessage() {
                RequestUri = new Uri(path + ParamsToString(queryParams)),
                Method = HttpMethod.Patch,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
        }
        private static string ParamsToString(Dictionary<string, object> queryParams) {
            if (queryParams == null || queryParams.Count == 0) {
                return "";
            }
            string stringParams = "?";
            foreach (var param in queryParams) {
                if (param.Value != null && param.Value != "") {

                    if (param.Value is IList) {
                        foreach (object item in param.Value as IList) {
                            stringParams += $"{param.Key}={item}&";
                        }
                    }
                    else {
                        stringParams += param.Key + "=" + param.Value + "&";
                    }
                }
            }
            return stringParams;
        }
    }
}
