using System;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
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
                var pageToken = Environment.GetEnvironmentVariable(PageToken);

                var client = new HttpClient();

                var postMessage = $"Post at {DateTime.UtcNow:O} UTC";

                //var response = await client.PostAsync($"https://graph.facebook.com/russfeeder/feed?message={postMessage}&access_token={pageToken}", new StringContent(""));

                //if (response.IsSuccessStatusCode)
                //{
                //    Console.WriteLine("Facebook post complete.");
                //}
                //else
                //{
                //    var responseContent = await response?.Content?.ReadAsStringAsync();

                //    Console.WriteLine($"Facebook post failed: {response.StatusCode} {responseContent}");
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Facebook post failed: {ex.Message} {ex.StackTrace}");
            }

            return "Done";
        }
    }
}
