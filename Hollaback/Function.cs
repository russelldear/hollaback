using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Hollaback
{
    public class Function
    {
        public async Task<string> FunctionHandler(string input, ILambdaContext context)
        {
            Console.WriteLine("Done.");

            return await Task.FromResult("Done;");
        }
    }
}
