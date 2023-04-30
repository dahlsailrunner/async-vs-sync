using NBomber.CSharp;
using NBomber.Data;
using NBomber.Data.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;

namespace CarvedRock.PerformanceTest;

internal class Program
{
    private const string BaseUrl = "https://localhost:7213";
    private static readonly IDataFeed<string> ApiParams = DataFeed.Random(new List<string> { "all", "boots", "equip", "kayak" });

    static void Main()
    {
        var asyncHttpClient = new HttpClient();
        var asyncScenario = Scenario.Create("ASYNC requests", async context =>
        {
            var requestParameter = ApiParams.GetNextItem(context.ScenarioInfo);

            var request = Http.CreateRequest("GET", $"{BaseUrl}/Product?Category={requestParameter}");

            var clientArgs = new HttpClientArgs(
                httpCompletion: HttpCompletionOption.ResponseContentRead,
                cancellationToken: CancellationToken.None
            );

            //Console.WriteLine($"Sending request for {requestParameter}");

            return await Http.Send(asyncHttpClient, clientArgs, request);
        });

        var httpClient = new HttpClient();
        var scenario = Scenario.Create("SYNCHRONOUS requests", async context =>
        {
            var requestParameter = ApiParams.GetNextItem(context.ScenarioInfo);

            var request = Http.CreateRequest("GET", $"{BaseUrl}/SyncProduct?Category={requestParameter}");

            var clientArgs = new HttpClientArgs(
                httpCompletion: HttpCompletionOption.ResponseContentRead,
                cancellationToken: CancellationToken.None
            );

            //Console.WriteLine($"Sending request for {requestParameter}");

            return await Http.Send(httpClient, clientArgs, request);
        });

        asyncScenario = asyncScenario.WithLoadSimulations(Simulation.Inject(rate: 100, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)));
        scenario = scenario.WithLoadSimulations(Simulation.Inject(rate: 100, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)));

        NBomberRunner
            .RegisterScenarios(  // comment out scenarios you don't want to run below
                scenario
                //asyncScenario
                )
            .Run();
    }
}
