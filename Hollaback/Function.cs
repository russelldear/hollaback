using System;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using static Hollaback.Constants.EnvironmentVariables;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Hollaback
{
    public class Function
    {
        public async Task<string> FunctionHandler(string input, ILambdaContext context)
        {
            try
            {
                var url = "https://www.etymologynerd.com/1/feed";

                using var reader = XmlReader.Create(url);

                var feed = SyndicationFeed.Load(reader);

                var recentItems = feed.Items.Where(i => i.PublishDate > DateTime.UtcNow.AddDays(-28));

                var pageToken = Environment.GetEnvironmentVariable(PageToken);

                var client = new HttpClient();

                foreach (var item in recentItems)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(item));

                    var postMessage = $"{feed.Title.Text} - {item.Title.Text} {Environment.NewLine} {item.Summary.Text} {Environment.NewLine}";

                    var postLink = item.Links.FirstOrDefault().Uri;

                    var response = await client.PostAsync($"https://graph.facebook.com/russfeeder/feed?message={postMessage}&link={postLink}&access_token={pageToken}", new StringContent(""));

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Facebook post complete.");
                    }
                    else
                    {
                        var responseContent = await response?.Content?.ReadAsStringAsync();

                        Console.WriteLine($"Facebook post failed: {response.StatusCode} {responseContent}");
                    }
                }

                Console.WriteLine($"All items posted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Facebook post failed: {ex.Message} {ex.StackTrace}");
            }

            return "Done";
        }
    }
}
