using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using static Hollaback.Constants.EnvironmentVariables;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Hollaback
{
    public class Function
    {
        private HttpClient _client;
        private string _pageToken;
        private FacebookService _facebookService;
        private DynamoDbService _dynamoDbService;

        private readonly List<string> feedUrls = new List<string>
        {
            "https://github.com/russelldear/catharsis/commits.atom",
            //"https://www.etymologynerd.com/1/feed",
            //"http://maryholm.com/feed/",
            //"https://www.kleptones.com/blog/feed/",
            //"https://xkcd.com/rss.xml",
            //"https://alladodeisabel.blogspot.com/feeds/posts/default?alt=rss",
            //"http://london-underground.blogspot.com/atom.xml",
            //"https://stackoverflow.com/feeds/tag/xero-api",
            //"https://weirdinwellington.com/rss#_=_",
            //"http://feeds.feedburner.com/futilitycloset",
            //"http://craigjparker.blogspot.com/feeds/posts/default",
            //"https://mjtsai.com/blog/feed/",
            //"https://www.rnz.co.nz/rss/national.xml",
            //"https://www.youtube.com/feeds/videos.xml?channel_id=UCIRiWCPZoUyZDbydIqitHtQ",
            //"https://www.theredhandfiles.com/feed/",
            //"https://theoatmeal.com/feed/rss",
            //"https://awsteele.com/feed.xml",
            //"https://toroabrewing.com/feed/"
        };

        public Function()
        {
            _client = new HttpClient();
            _pageToken = Environment.GetEnvironmentVariable(PageToken);
            _facebookService = new FacebookService(_client, _pageToken);

            var dynamoDbClient = new AmazonDynamoDBClient();
            _dynamoDbService = new DynamoDbService(dynamoDbClient);
        }

        public Function(string awsEndpoint) : this()
        {
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig
            {
                ServiceURL = awsEndpoint
            };

            var dynamoDbClient = new AmazonDynamoDBClient(clientConfig);
            _dynamoDbService = new DynamoDbService(dynamoDbClient);
        }

        public async Task<string> FunctionHandler(string input, ILambdaContext context)
        {
            try
            {
                foreach (var feedUrl in feedUrls)
                {
                    var unpostedItems = new List<SyndicationItem>();

                    SyndicationFeed feed = null;

                    try
                    {
                        using var reader = XmlReader.Create(feedUrl);
                        feed = SyndicationFeed.Load(reader);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Retrieval failed for {feedUrl}: {ex.Message}");
                        continue;
                    }

                    try
                    {
                        foreach (var item in feed.Items)
                        {
                            var isPosted = await _dynamoDbService.IsPosted(item.Id);

                            if (!isPosted)
                            {
                                unpostedItems.Add(item);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Queueing failed for {feedUrl}: {ex.Message}");
                        continue;
                    }

                    //Console.WriteLine($"Captured {unpostedItems.Count} unposted items in {feed.Title.Text} feed.");

                    try
                    {
                        foreach (var item in unpostedItems)
                        {
                            if (!string.IsNullOrWhiteSpace(_pageToken))
                            {
                                await _facebookService.PostItem(item, feed.Title.Text);
                            }

                            await _dynamoDbService.SetPosted(item.Id);
                        }

                        //Console.WriteLine($"All items posted for {feedUrl}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Posting failed for {feedUrl}: {ex.Message} {ex.StackTrace}");
                    }
                }

                //Console.WriteLine($"All items posted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Posting RSS feeds to Facebook failed: {ex.Message} {ex.StackTrace}");
            }

            return "Done";
        }
    }
}
