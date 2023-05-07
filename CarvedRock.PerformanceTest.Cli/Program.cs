using System.CommandLine;
using CarvedRock.PerformanceTest.Cli;
using NBomber.CSharp;

var (baseUriOption, injectionRateOption) = DefineGlobalOptions();

var rootCommand = new RootCommand("CarvedRock Performance Test CLI")
{
    baseUriOption,
    injectionRateOption
};

rootCommand.SetHandler(RunPerformanceTest, baseUriOption, injectionRateOption);

await rootCommand.InvokeAsync(args);

void RunPerformanceTest(Uri baseUri, int injectionRate)
{
    var httpClient = new HttpClient();
    var urlFormat = $"{baseUri}Product?category={{0}}"; // category provided dynamically/randomly

    var scenario = NBomberHelper.GetScenario("ASYNC requests", 
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
