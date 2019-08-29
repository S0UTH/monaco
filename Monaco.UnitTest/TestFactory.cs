using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Monaco.UnitTest
{
    public class TestFactory
    {
        private static Dictionary<string, StringValues> CreateDictionary(string key1, string value1, string key2= null, string value2 = null)
        {
            var qs = new Dictionary<string, StringValues>
            {
                { key1, value1 }
            };

            if(key2 != null)
            {
                qs.Add(key2, value2);
            }
            return qs;
        }

        public static DefaultHttpRequest CreateHttpRequest(string queryStringKey1, string queryStringValue1, string queryStringKey2=null, string queryStringValue2=null)
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Query = new QueryCollection(CreateDictionary(queryStringKey1, queryStringValue1, queryStringKey2, queryStringValue2))
            };
            return request;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;

            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }
    }
}