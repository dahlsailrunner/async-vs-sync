using Hellang.Middleware.ProblemDetails;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;
using System.Reflection;
using CarvedRock.Api.BusinessLogic;
using CarvedRock.Api.Data;
using CarvedRock.Api.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, loggerConfig) => {
    loggerConfig
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.WithProperty("Application", Assembly.GetExecutingAssembly().GetName().Name ?? "API")
    .Enrich.WithExceptionDetails()
    .Enrich.FromLogContext()
    .Enrich.With<ActivityEnricher>()
    //.WriteTo.Seq("http://localhost:5341")
    .WriteTo.Console()
    .WriteTo.Debug();
});

builder.Services.AddProblemDetails(opts => 
{
    opts.IncludeExceptionDetails = (_, _) => false;
    
    opts.OnBeforeWriteDetails = (_, details) => {
        if (details.Status == 500)
        {
            details.Detail = "An error occurred in our API. Use the trace id when contacting us.";
        }
    }; 
    opts.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductLogic, ProductLogic>();
builder.Services.AddDbContext<LocalContext>();
builder.Services.AddScoped<ICarvedRockRepository, CarvedRockRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<LocalContext>();
    context.MigrateAndCreateData();
}

app.UseProblemDetails();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.MapControllers();
app.MapFallback(() => Results.Redirect("/swagger"));

app.Run();
