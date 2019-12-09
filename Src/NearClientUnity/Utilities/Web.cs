using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NearClientUnity.Utilities
{
    public static class Web
    {
        public static async Task<dynamic> FetchJsonAsync(string url, string json = "")
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Content-type", "application/json; charset=utf-8");

                HttpResponseMessage response;

                if (!string.IsNullOrEmpty(json))
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(url, content);
                }
                else
                {
                    response = await client.GetAsync(url);
                }

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    dynamic result = JObject.Parse(jsonString);
                    return result;
                }
                else
                {
                    throw new HttpException((int) response.StatusCode, response.Content.ToString());
                }
            }
        }

        public static async Task<string> FetchJsonAsync(IConnectionInfo connection, string json = "")
        {
            var url = connection.Url;
            var result = await FetchJsonAsync(url, json);
            return result;
        }
    }
}