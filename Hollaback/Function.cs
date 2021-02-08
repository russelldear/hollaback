using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using static Hollaback.Constants.EnvironmentVariables;
using static System.Net.WebUtility;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Hollaback
{
    public class Function
    {
        private HttpClient _client = new HttpClient();
        private string _pageToken;

        private List<string> feedUrls = new List<string>
        {
            "https://www.etymologynerd.com/1/feed",
            "http://maryholm.com/feed/",
            "https://www.kleptones.com/blog/feed/",
            "https://xkcd.com/rss.xml",
            "https://alladodeisabel.blogspot.com/feeds/posts/default?alt=rss",
            "http://london-underground.blogspot.com/atom.xml",
            "http://explosm-feed.antonymale.co.uk/feed",
            "https://stackoverflow.com/feeds/tag/xero-api",
            "https://weirdinwellington.com/rss#_=_",
            "http://feeds.feedburner.com/futilitycloset",
            "http://craigjparker.blogspot.com/feeds/posts/default",
            "https://mjtsai.com/blog/feed/"
        };


        public async Task<string> FunctionHandler(string input, ILambdaContext context)
        {
            try
            {
                _pageToken = Environment.GetEnvironmentVariable(PageToken);

                foreach (var feedUrl in feedUrls)
                {
                    try
                    {
                        using var reader = XmlReader.Create(feedUrl);

                        var feed = SyndicationFeed.Load(reader);

                        //var recentItems = feed.Items.OrderByDescending(i => i.PublishDate).Take(1);

                        var recentItems = feed.Items.Where(i => i.PublishDate > DateTime.UtcNow.AddMinutes(-6));

                        foreach (var item in recentItems)
                        {
                            await PostItem(item, feed.Title.Text);
                        }

                        Console.WriteLine($"All items posted for {feedUrl}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Posting failed for {feedUrl}: {ex.Message} {ex.StackTrace}");
                    }
                }

                Console.WriteLine($"All items posted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Posting RSS feeds to Facebook failed: {ex.Message} {ex.StackTrace}");
            }

            return "Done";
        }

        private async Task PostItem(SyndicationItem item, string feedTitle)
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
                var responseContent = await response?.Content?.ReadAsStringAsync();

                Console.WriteLine($"Facebook post failed: {response.StatusCode} {responseContent}");
            }
        }
    }
}
