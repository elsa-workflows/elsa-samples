using Elsa.Api.Client.HttpMessageHandlers;
using Elsa.Api.Client.Options;
using Elsa.Studio.Contracts;
using Elsa.Studio.Core.BlazorWasm.Extensions;
using Elsa.Studio.Extensions;
using Elsa.Studio.Localization.BlazorWasm.Extensions;
using Elsa.Studio.Localization.Models;
using Elsa.Studio.Models;
using Elsa.Studio.Options;
using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.Workflows.Extensions;
using ElsaStudioRehostedBlazorComponents.Client.Services;
using ElsaStudioRehostedBlazorComponents.Components;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();
builder.Services.Configure<ElsaClientOptions>(configuration.GetSection("WorkflowServer"));
builder.Services.Configure<BackendOptions>(configuration.GetSection("Backend"));

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ElsaStudioRehostedBlazorComponents.Client._Imports).Assembly);

app.Run();