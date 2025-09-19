using Elsa.Studio.Contracts;
using Elsa.Studio.Core.BlazorWasm.Extensions;
using Elsa.Studio.Dashboard.Extensions;
using Elsa.Studio.Extensions;
using Elsa.Studio.Localization.Time;
using Elsa.Studio.Localization.Time.Providers;
using Elsa.Studio.Login.BlazorWasm.Extensions;
using Elsa.Studio.Login.Extensions;
using Elsa.Studio.Login.HttpMessageHandlers;
using Elsa.Studio.Models;
using Elsa.Studio.Shell;
using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.WorkflowContexts.Extensions;
using Elsa.Studio.Workflows.Designer.Extensions;
using Elsa.Studio.Workflows.Extensions;
using ElsaStudioBlazorWebAssemblyApp.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var configuration = builder.Configuration;

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.RootComponents.RegisterCustomElsaStudioElements();

var backendApiConfig = new BackendApiConfig
{
    ConfigureBackendOptions = options => configuration.GetSection("Backend").Bind(options),
    ConfigureHttpClientBuilder = options =>
    {
        options.AuthenticationHandler = typeof(AuthenticatingApiHttpMessageHandler);
        options.ConfigureHttpClient = (sp, httpClient) =>
        {
            var tenantIdAccessor = sp.GetRequiredService<TenantIdAccessor>();
            var tenantId = tenantIdAccessor.GetTenantIdFromUrl();
            
            if (tenantId != null)
                httpClient.DefaultRequestHeaders.Add("X-Company-Id", tenantId);
        };
    },
};

builder.Services.AddCore();
builder.Services.AddShell();
builder.Services.AddRemoteBackend(backendApiConfig);
builder.Services.AddLoginModule().UseElsaIdentity();
builder.Services.AddDashboardModule();
builder.Services.AddWorkflowsModule();
builder.Services.AddWorkflowContextsModule();
builder.Services.AddScoped<ITimeZoneProvider, LocalTimeZoneProvider>();
builder.Services.AddSingleton<TenantIdAccessor>();

var app = builder.Build();
var startupTaskRunner = app.Services.GetRequiredService<IStartupTaskRunner>();
await startupTaskRunner.RunStartupTasksAsync();
await app.RunAsync();