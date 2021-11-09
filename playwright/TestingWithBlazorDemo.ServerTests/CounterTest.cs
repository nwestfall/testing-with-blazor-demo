using System.Threading.Tasks;
using Xunit;
using Microsoft.Playwright;
using VerifyXunit;

namespace TestingWithBlazorDemo.ServerTests
{
    [UsesVerify]
    [Collection("Counter")]
    public class CounterTest : IClassFixture<PlaywrightFixture>
    {
        PlaywrightFixture fixture;

        public CounterTest(PlaywrightFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("en-US", "chrome", "")]
        [InlineData("en-US", "firefox", "")]
        [InlineData("en-US", "safari", "")]
        public async Task CounterComponentRendersCorrectly(string language, string browser, string device)
        {
            
        }

        [Theory]
        [InlineData("en-US", "chrome", "")]
        [InlineData("en-US", "firefox", "")]
        [InlineData("en-US", "safari", "")]
        public async Task CounterComponentRendersCorrectlyAfterClicks(string language, string browser, string device)
        {
            
        }
    }
}