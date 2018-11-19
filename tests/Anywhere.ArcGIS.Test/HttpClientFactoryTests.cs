using System.Threading.Tasks;
using Xunit;

namespace Anywhere.ArcGIS.Test
{
    public class HttpClientFactoryTests
    {
        [Fact]
        public async Task GetWithoutRetring()
        {
            var handler = new RetryHttpClientHandler();
            HttpClientFactory.InitHttpClientHandler(handler);

            int retryCount = 0;
            handler.Retrying += (s, e) => retryCount = e.CurrentRetryCount;

            var client = HttpClientFactory.CreateHttpClient(handler);
            HttpClientFactory.InitHttpClient(client);

            var message = await client.GetAsync("http://example.com/");

            Assert.True(message.IsSuccessStatusCode);
            Assert.Equal(0, retryCount);
        }
    }
}
