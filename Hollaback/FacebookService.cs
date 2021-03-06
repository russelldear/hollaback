using System;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static System.Net.WebUtility;

namespace Hollaback
{
    public class FacebookService
    {
        private HttpClient _client;
        private string _pageToken;

        public FacebookService(HttpClient client, string pageToken)
        {
            _client = client;
            _pageToken = pageToken;
        }

        public async Task PostItem(SyndicationItem item, string feedTitle)
        {
            Console.WriteLine(JsonConvert.SerializeObject(item));

            var postMessage = UrlEncode($"{feedTitle} - {item.Title.Text} {Environment.NewLine} {item.Summary.Text} {Environment.NewLine}");

            var postLink = UrlEncode(item.Links.FirstOrDefault().Uri.ToString());

            var response = await _client.PostAsync($"https://graph.facebook.com/russfeeder/feed?message={postMessage}&link={postLink}&access_token={_pageToken}", new StringContent(""));

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Facebook post complete.");
            }
            else
            {
                var responseHeaders = string.Join(" | ", response?.Headers.Select(h => $"{h.Key}:{string.Join(", ", h.Value)}"));

                var responseContent = await response?.Content?.ReadAsStringAsync();

                Console.WriteLine($"Facebook post failed: {response.StatusCode} {responseContent} {responseHeaders}");
            }
        }
    }
}
