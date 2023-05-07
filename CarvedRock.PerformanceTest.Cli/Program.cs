using System.CommandLine;
using CarvedRock.PerformanceTest.Cli;
using NBomber.CSharp;

var (baseUriOption, injectionRateOption) = DefineGlobalOptions();

var rootCommand = new RootCommand("CarvedRock Performance Test CLI");
rootCommand.AddGlobalOption(baseUriOption);
rootCommand.AddGlobalOption(injectionRateOption);

var asyncCommand = new Command("async", "Run load test against the async API endpoint");
asyncCommand.SetHandler((baseUri, injRate) =>
    RunPerformanceTest(baseUri, injRate, "ASYNC scenario", "Product?category={0}"),
    baseUriOption, injectionRateOption);

rootCommand.AddCommand(asyncCommand);

var syncCommand = new Command("sync", "Run load test against the synchronous API endpoint");
syncCommand.SetHandler((baseUri, injRate) =>
        RunPerformanceTest(baseUri, injRate, "SYNCHRONOUS scenario", "SyncProduct?category={0}"),
    baseUriOption, injectionRateOption);
rootCommand.AddCommand(syncCommand);

await rootCommand.InvokeAsync(args);

void RunPerformanceTest(Uri baseUri, int injectionRate, string scenarioName, string apiRoute)
{
    var httpClient = new HttpClient();
    var urlFormat = $"{baseUri}{apiRoute}"; // category provided dynamically/randomly

    var scenario = NBomberHelper.GetScenario(scenarioName,
        urlFormat, injectionRate, httpClient);

    NBomberRunner.RegisterScenarios(scenario)
        .Run();
}

(Option<Uri> BaseUrlOption, Option<int> InjectionRateOption) DefineGlobalOptions()
{
    var baseUrlOption = new Option<Uri>(
        "--url", "The base URL to test, e.g. https://localhost:7213")
    {
        IsRequired = true
    };
    baseUrlOption.AddAlias("-u");

    var injectionRate = new Option<int>("--rate", 
        "Injection rate. Number of new requests to generate each second.");
    injectionRate.SetDefaultValue(30);
    injectionRate.AddAlias("-r");

    return (baseUrlOption, injectionRate);
}
