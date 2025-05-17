using Elsa.Api.Client.HttpMessageHandlers;
using Elsa.Api.Client.Options;
using Elsa.Studio.Contracts;
using Elsa.Studio.Core.BlazorWasm.Extensions;
using Elsa.Studio.Extensions;
using Elsa.Studio.Localization.BlazorWasm.Extensions;
using Elsa.Studio.Localization.Models;
using Elsa.Studio.Localization.Time;
using Elsa.Studio.Localization.Time.Providers;
using Elsa.Studio.Models;
using Elsa.Studio.Options;
using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.Workflows.Designer.Extensions;
using Elsa.Studio.Workflows.Extensions;
using ElsaStudioBlazorComponents.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var configuration = builder.Configuration;
builder.Services.Configure<ElsaClientOptions>(configuration.GetSection("WorkflowServer"));
builder.Services.Configure<BackendOptions>(configuration.GetSection("Backend"));
builder.RootComponents.RegisterCustomElsaStudioElements();

var backendApiConfig = new BackendApiConfig
{
    ConfigureBackendOptions = options => configuration.GetSection("WorkflowServer").Bind(options),
    ConfigureHttpClientBuilder = options =>
    {
        options.ApiKey = configuration["Backend:ApiKey"];
        options.BaseAddress = new("https://localhost:5001/elsa/api");
        options.AuthenticationHandler = typeof(ApiKeyHttpMessageHandler); // TODO: replace with your own handler that e.g. attaches a bearer token.
    }, 
};

var localizationConfig = new LocalizationConfig
{
    ConfigureLocalizationOptions = options => configuration.GetSection("Localization").Bind(options),
};

builder.Services.AddCore();
builder.Services.AddShell();
builder.Services.AddRemoteBackend(backendApiConfig);
builder.Services.Replace(ServiceDescriptor.Scoped<IRemoteBackendAccessor, ComponentRemoteBackendAccessor>());
builder.Services.AddWorkflowsModule();
builder.Services.AddLocalizationModule(localizationConfig);
builder.Services.AddScoped<ITimeZoneProvider, LocalTimeZoneProvider>();
await builder.Build().RunAsync();