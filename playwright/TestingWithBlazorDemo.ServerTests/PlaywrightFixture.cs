using System;
using System.Threading.Tasks;
using AngleSharp.Diffing;
using AngleSharp.Diffing.Core;
using Microsoft.Playwright;
using VerifyTests;
using VerifyXunit;
using ImageMagick;
using Xunit;

public class PlaywrightFixture :
    IAsyncLifetime
{
    IPlaywright playwright;
    Lazy<Task<IBrowser>> chromeBrowser;
    Lazy<Task<IBrowser>> firefoxBrowser;
    Lazy<Task<IBrowser>> safariBrowser;
    VerifySettings verifierSettings;

    string baseUrl;

    public VerifySettings VerifySettings { get => verifierSettings!; }

    public async Task InitializeAsync()
    {
        Console.WriteLine("Creating Playwright");
        playwright = await Playwright.CreateAsync();
        chromeBrowser = new Lazy<Task<IBrowser>>(() => 
        {
            return playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
            {
                Headless = Environment.GetEnvironmentVariable("PLAYWRIGHT_PREVIEW") != "1"
            });
        });
        firefoxBrowser = new Lazy<Task<IBrowser>>(() => 
        {
            return playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions()
            {
                Headless = Environment.GetEnvironmentVariable("PLAYWRIGHT_PREVIEW") != "1"
            });
        });
        safariBrowser = new Lazy<Task<IBrowser>>(() => 
        {
            return playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions()
            {
                Headless = Environment.GetEnvironmentVariable("PLAYWRIGHT_PREVIEW") != "1"
            });
        });

        baseUrl = Environment.GetEnvironmentVariable("SITE_URL");

        if(string.IsNullOrEmpty(baseUrl))
            throw new ArgumentNullException("SITE_URL not set");
        VerifyPlaywright.Enable();
        VerifyImageMagick.Initialize();
        VerifyImageMagick.RegisterComparers(.05, ErrorMetric.Fuzz);
        VerifyAngleSharpDiffing.Initialize();

        verifierSettings = new();
        verifierSettings.UseDirectory("TestData");
        if(Environment.GetEnvironmentVariable("VERIFY_DISABLEDIFF") == "1")
            verifierSettings.DisableDiff();

        verifierSettings.ScrubInlineGuids();
        verifierSettings.AngleSharpDiffingSettings(
            action =>
            {
                static FilterDecision ScriptFilter(
                    in ComparisonSource source,
                    FilterDecision decision)
                {
                    if (source.Node.NodeName == "SCRIPT")
                    {
                        return FilterDecision.Exclude;
                    }

                    return decision;
                }

                var options = action.AddDefaultOptions();
                options.AddFilter(ScriptFilter);
            });
    }

    public async Task<IPage> Setup(string language = "en-US", string desiredBrowser = "chrome", string device = "")
    {
        Console.WriteLine($"Setup - {language}|{desiredBrowser}|{device}");
        // Open new page
        IPage page = null;
        if(string.IsNullOrEmpty(device))
        {
            Console.WriteLine("Creating context");
            var context = await (await GetBrowser(desiredBrowser)).NewContextAsync(new BrowserNewContextOptions
            {
                Locale = language
            });
            Console.WriteLine("Creating page");
            page = await context.NewPageAsync();
        }
        else
        {
            var deviceSettings = playwright.Devices[device];
            deviceSettings.Locale = language;
            Console.WriteLine("Creating context");
            var context = await (await GetBrowser(desiredBrowser)).NewContextAsync(deviceSettings);
            Console.WriteLine("Creating page");
            page = await context.NewPageAsync();
        }
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Console.WriteLine($"Navigating to {baseUrl}");
        await page.GotoAsync(baseUrl);

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        return page;
    }

    public Task VerifyPage(IPage page, params object[] parameters)
    {
        this.VerifySettings.UseParameters(parameters);
        return Verifier.Verify(page, VerifySettings);
    }

    public async Task DisposeAsync()
    {
        if (chromeBrowser?.IsValueCreated == true)
        {
            await (await chromeBrowser.Value).DisposeAsync();
        }
        if (firefoxBrowser?.IsValueCreated == true)
        {
            await (await firefoxBrowser.Value).DisposeAsync();
        }
        if (safariBrowser?.IsValueCreated == true)
        {
            await (await safariBrowser.Value).DisposeAsync();
        }

        playwright?.Dispose();
    }

    Task<IBrowser> GetBrowser(string desiredBrowser)
    {
        switch(desiredBrowser)
        {
            case "chrome":
                return chromeBrowser.Value;
            case "safari":
                return safariBrowser.Value;
            case "firefox":
                return firefoxBrowser.Value;
            default:
                throw new ArgumentException("Browser not supported");
        }
    }
}