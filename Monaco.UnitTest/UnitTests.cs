using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Monaco.Endpiont;
using System;
using Xunit;

namespace Monaco.UnitTest
{
    /// <summary>
    /// Test stragety for Azure Function as laid out here: https://docs.microsoft.com/en-us/azure/azure-functions/functions-test-a-function
    /// </summary>
    public class UnitTests
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void Http_trigger_should_return_valid_string()
        {
            var request = TestFactory.CreateHttpRequest("lat", "37.72366127", "lon", "-122.43594382");
            var response = (OkObjectResult)await TrucksFunc.Run(request, logger);
            Assert.False(response.Value == null);
        }
    }
}


