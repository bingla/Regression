using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Regression.Application.Handlers;
using Regression.Application.Interfaces;
using Regression.Application.Services;
using Regression.Data.Interfaces;
using Regression.Data.Repositories;
using Regression.Domain;
using Regression.Web.Client.Pages;
using Regression.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add Database (for long time persistance)
builder.Services.AddDbContext<RegressionContext>(options =>
{
    options.UseInMemoryDatabase("RegressionDb");
    options.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
}, ServiceLifetime.Scoped);

// Add InMemory cache
builder.Services.AddMemoryCache();

// Add services and handlers to the container.
builder.Services
    .AddSingleton<ICacheRepository, CacheRepository>()
    .AddTransient<IScheduleRepository, ScheduleRepository>()
    .AddTransient<ITestCollectionRepository, TestCollectionRepository>()
    .AddTransient<ITestResultRepository, TestResultRepository>()
    .AddTransient<ITestRepository, TestRepository>()
    .AddTransient<ITestRunRepository, TestRunRepository>()
    .AddTransient<IRequestService, RequestService>()
    .AddTransient<ITestRunService, TestRunService>()
    .AddTransient<RequestHandler>()
    .AddTransient<ResponseHandler>();

// Add HttpClient Factory and register handlers on it
builder.Services
    .AddHttpClient<IRequestService, RequestService>(options =>
    {
        options.Timeout = TimeSpan.FromMinutes(1); // Fail calls if no answer within this timeframe 
    })
    .AddHttpMessageHandler<ResponseHandler>()  // Log inbound request
    .AddHttpMessageHandler<RequestHandler>();  // Log outbound request


// Add Hangfire background server (use InMemory while testing, use SQL/Redis for persistence)
builder.Services
    .AddHangfire(config =>
    {
        config.UseMemoryStorage();
    })
    .AddHangfireServer();

// Add Blazor
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

app.UseHangfireDashboard();

app.Run();
