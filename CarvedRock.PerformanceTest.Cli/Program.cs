using System.CommandLine;

var (baseUriOption, injectionRateOption) = DefineGlobalOptions();

var rootCommand = new RootCommand("CarvedRock Performance Test CLI")
{
    baseUriOption,
    injectionRateOption
};

rootCommand.SetHandler(DoSomething, baseUriOption, injectionRateOption);

await rootCommand.InvokeAsync(args);

void DoSomething(Uri uriArgument, int injectionRate)
{
    Console.WriteLine($"The base URL is {uriArgument}");
    Console.WriteLine($"The injection rate is {injectionRate} requests per second");
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
