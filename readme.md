# Sync vs Async and NBomber

This repo is meant for simple exploration of how using asynchronous
code porovides better performance under load than synchronous code
does.

It has two API controller actions:

* `GET /Product`: Uses async code for everything
* `GET /SyncProduct`: Uses synchronous code for everything

Both of the above methods eventually code some simple code that
retrieves some data from a SQLite database and also sleeps to simulate
a somewhat heavy query.

The following code is in the `CarvedRock.Data/CarvedRockRepository` class:

```C#
public async Task<List<Product>> GetProductListAsync(string category)
{
    await Task.Delay(400); // simulates heavy query
    return await _ctx.Products.Where(p => p.Category == category || category == "all")
        .ToListAsync();
}

public List<Product> GetProductList(string category)
{
    Thread.Sleep(400); // simulates heavy query
    return _ctx.Products.Where(p => p.Category == category || category == "all")
        .ToList();
}
```

The first method above is completely async - even with the `await Task.Delay(400)` instead of `Thread.Sleep`.  Both methods do exactly the same
thing except for the async nature of the first method and the
syncrhonous nature of the second.

## Performance Testing

Wihtout concurrency, both of the controller actions will perform with
similar results - just under a half second for each call, with the vast
majority of the time simply in the Sleep/Delay calls.

To really measure the difference here, we need to somehow create
concurrent load against the running API, and that's where
[NBomber](https://nbomber.com/) comes in.

This is a simple NuGet package that can be used for exactly what
we're looking for, and the `CarvedRock.PerformanceTest` project
is a simple console app and defines the load tests.  All of the
logic for the tests is in the `Program.cs` file.

### Running the tests

To get more real-life metrics, neither the API nor the tests should
be run straight from Visual Studio (or Rider or another IDE).  Rather
they should be run from the command line from a published (or
Release configruation) build.

To run the API in support for the tests, go into the API project
directory, and run the following command:

```bash
dotnet run -c Release > stdout.log
```

This will run the API in release mode and dump the output from the
console into the `stdout.log` file.

To run the performance test, use a terminal in the
`CarvedRock.PerformanceTest` directory.

```bash
dotnet run -c Release
```

The command above will run the NBomber test against the API.

It will display some standard output about the tests as they
run but it will also create some reports in the `reports` directory
that summarize the results as it finishes.

## VS Code Setup

If you're using VS Code, the `C#` extension is required to use this repo.  I have some other settings that you may be curious about
and they are described in my [VS Code gist](https://gist.github.com/dahlsailrunner/1765b807940e29951ea6bdfb36cd85dd).

## Logging to Seq (Disabled by default)

Seq logging is commented OUT by default, but can easily be enabled
by commenting in the following line from `Program.cs`:

```C#
//.WriteTo.Seq("http://localhost:5341")
```

The Docker image for Seq can be pulled and started with the following commands:

```bash
docker pull datalust/seq
docker run -d --name seq --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq
```

The NuGet package used for this is [Serilog.Sinks.Seq](https://www.nuget.org/packages/Serilog.Sinks.Seq)
