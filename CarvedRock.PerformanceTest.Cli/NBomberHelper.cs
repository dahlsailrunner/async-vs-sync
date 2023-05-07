using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Data;
using NBomber.Data.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;

namespace CarvedRock.PerformanceTest.Cli;
internal static class NBomberHelper
{
    private static readonly IDataFeed<string> ApiParams = DataFeed.Random(new List<string>
    {
        "all", "boots", "equip", "kayak"
    });
    internal static ScenarioProps GetScenario(string scenarioName, string urlFormat, 
        int injectionRate, HttpClient httpClient)
    {
        var scenario = Scenario.Create(scenarioName, async context =>
        {
            var requestParameter = ApiParams.GetNextItem(context.ScenarioInfo);
            var request = Http.CreateRequest("GET", string.Format(urlFormat, requestParameter));

            var clientArgs = new HttpClientArgs(
                httpCompletion: HttpCompletionOption.ResponseContentRead,
                cancellationToken: CancellationToken.None);

            return await Http.Send(httpClient, clientArgs, request);
        });
        scenario = scenario.WithLoadSimulations(Simulation.Inject(rate: injectionRate,
                       interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)));
        return scenario;
    }
}
