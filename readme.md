# Sync vs Async, NBomber, and Cancellation Tokens

This repo is meant for simple exploration of how using asynchronous
code provides better performance under load than synchronous code
does and a little about how cancellation tokens can be used to avoid
continued processing of long read operations when requests are canceled.

## Sync vs Async
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

The first method above is completely async - even with use of
`await Task.Delay(400)` instead of `Thread.Sleep`.  Both methods
do exactly the same thing except for the async nature of the
first method and the synchronous nature of the second.

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

## Cancellation Tokens

This API contains a third controller called `LongReadController` to demonstrate
the usefulness of cancellation tokens.

The `GET` controller action is simple:

```C#
[HttpGet]
public async Task<string> Get(bool includeCancellation, CancellationToken token)
{
    if (!includeCancellation)
    {
        token = CancellationToken.None;
    }
    return await _longReadLogic.GetSequentialLongQueryAsync(token);
}
```

The `includeCancellation` parameter is ONLY meant to help this demo - you
would generally not use it in a real API.  It's only purpose is to allow
the caller to specify whether the real cancellation token should be used
when calling the `GetSequentialLongQueryAsync` method.

The `LongReadLogic` class has a single method that makes 10 calls to a 
repository method that will sleep for a second per call and write a log
entry:

```C#
public async Task<string> GetSequentialLongQuery(int sequenceNumber, CancellationToken token = default)
{
    await Task.Delay(1000, token); // simulates long single query
    Log.Information($"Query {sequenceNumber} completed.");
    return $"Query {sequenceNumber} completed.\n";
}
```

If you execute the `GET https://localhost:7213/LongRead` endpoint, the logging output
(and the API response) will look like this, and the log entries will happen about a second
apart from each other due to the `await Task.Delay(1000, token)` line:

```txt
Query 1 completed.
Query 2 completed.
Query 3 completed.
Query 4 completed.
Query 5 completed.
Query 6 completed.
Query 7 completed.
Query 8 completed.
Query 9 completed.
Query 10 completed.
```

### Enter Cancellation Tokens

Without using cancellation tokens, any time the API endpoint
is called, it will run all 10 iterations of the query.  With cancellation tokens, if 
a request is canceled the iterating query execution will stop at the point of cancellation
and no further iterations will be executed.

This is beneficial because under high-load environments you're not wasting any time / resources /
compute to execute queries whose results will never be used.

### Testing Cancellation Tokens

To test this, you can use the `includeCancellation` parameter on 
the `GET https://localhost:7213/LongRead` and then you also need to actually 
cancel the request.  

* Use a tool like [Postman](https://www.postman.com/) or [Insomnia](https://insomnia.rest/) to make the request and cancel the request while it's running
* Use the Swagger UI from a ***browser that isn't hooked to your IDE*** and either close the tab/browser while the
request is running or navigate to a new URL in the tab -- in either case the browser sends a cancellation
request. (If your Swagger UI browser is connected to the IDE while debugging closing the browser 
will stop the debugger)
* Use the REST Client extension in VS Code (click the spinning "Waiting" icon) or Rider and 
cancel the request while it's running (this feature appears to be missing within Visual Studio HTTP file support)

What you should see in the log output is that the API execution of the queries stops at the point
of cancellation and that an HTTP status of `499 Client Closed Request` is returned to the caller.

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
