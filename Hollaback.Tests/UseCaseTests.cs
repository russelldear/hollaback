using System;
using System.Threading.Tasks;
using Xunit;

namespace Hollaback.Tests
{
    public class UseCaseTests
    {
        [Fact]
        public async Task TestEndToEnd()
        {
            var function = new Function("http://localhost:4566");

            var result = await function.FunctionHandler(string.Empty, null);
        }
    }
}
