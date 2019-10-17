using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace fingerprint
{
    class Api
    {
        private static readonly HttpClient client = new HttpClient();
        public async Task<dynamic> Post(String url, Dictionary<string, string> data)
        {
            Dictionary<string, string> body = new Dictionary<string, string>
            {
                { "token", ";(6un4N/*XY1-f8" }
            };
            data.ToList().ForEach(x => body.Add(x.Key, x.Value));
            String content = new JavaScriptSerializer().Serialize(body);
            StringContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Headers = {
                    { "Origin", "https://afterschooltek.com" }
                },
                Content = httpContent
            };
            HttpResponseMessage response = await client.SendAsync(request);
            String responseString = await response.Content.ReadAsStringAsync();
            dynamic responseObject = new {};
            try
            {
                responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString);
            }
            catch (Exception e)
            {
                responseObject = new { notJson = responseString };
            }
            return responseObject;
        }
    }
}
