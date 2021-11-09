using System.Threading.Tasks;
using Xunit;
using Microsoft.Playwright;
using VerifyXunit;

namespace TestingWithBlazorDemo.ServerTests
{
    //[UsesVerify]
    //[Collection("Counter")]
    public class CounterTest_Complete : IClassFixture<PlaywrightFixture>
    {
        PlaywrightFixture fixture;

        public CounterTest_Complete(PlaywrightFixture fixture)
        {
            this.fixture = fixture;
        }

        //[Theory]
        //[InlineData("en-US", "chrome", "")]
        //[InlineData("en-US", "firefox", "")]
        //[InlineData("en-US", "safari", "")]
        public async Task CounterComponentRendersCorrectly(string language, string browser, string device)
        {
            var page = await this.fixture.Setup(language, browser, device);

            await page.ClickAsync("text=Counter");

            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            await this.fixture.VerifyPage(page, language, browser, device);
        }

        //[Theory]
        //[InlineData("en-US", "chrome", "")]
        //[InlineData("en-US", "firefox", "")]
        //[InlineData("en-US", "safari", "")]
        public async Task CounterComponentRendersCorrectlyAfterClicks(string language, string browser, string device)
        {
            var page = await this.fixture.Setup(language, browser, device);

            await page.ClickAsync("text=Counter");

            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            await page.ClickAsync("text=Click me");
            await page.ClickAsync("text=Click me");
            await page.ClickAsync("text=Click me");

            await this.fixture.VerifyPage(page, language, browser, device);
        }
    }
}