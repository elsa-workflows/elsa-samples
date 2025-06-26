using Elsa.Studio.Core.BlazorServer.Extensions;
using Elsa.Studio.Dashboard.Extensions;
using Elsa.Studio.Extensions;
using Elsa.Studio.Localization.Time;
using Elsa.Studio.Localization.Time.Providers;
using Elsa.Studio.Login.BlazorServer.Extensions;
using Elsa.Studio.Login.Extensions;
using Elsa.Studio.Login.HttpMessageHandlers;
using Elsa.Studio.Models;
using Elsa.Studio.Shell;
using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.Webhooks.Extensions;
using Elsa.Studio.WorkflowContexts.Extensions;
using Elsa.Studio.Workflows.Designer.Extensions;
using Elsa.Studio.Workflows.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

var backendApiConfig = new BackendApiConfig
{
    ConfigureBackendOptions = options => configuration.GetSection("Backend").Bind(options),
    ConfigureHttpClientBuilder = options => options.AuthenticationHandler = typeof(AuthenticatingApiHttpMessageHandler),
};

services.AddRazorPages();
services.AddRazorComponents().AddInteractiveServerComponents(options =>
{
    options.RootComponents.RegisterCustomElsaStudioElements();
    options.RootComponents.MaxJSRootComponents = 1000;
});
services.AddCore();
services.AddShell(options => configuration.GetSection("Shell").Bind(options));
services.AddRemoteBackend(backendApiConfig);
services.AddLoginModule();

var identityProvider = configuration.GetValue<string>("Provider");

switch (identityProvider)
{
    case "Elsa":
        services.UseElsaIdentity();
        break;
    case "OAuth2":
        services.UseOAuth2(options => configuration.GetSection("Providers:OAuth2").Bind(options));
        break;
}

services.AddDashboardModule();
services.AddWorkflowsModule();
services.AddWorkflowContextsModule();
services.AddWebhooksModule();
services.AddAgentsModule(backendApiConfig);
builder.Services.AddScoped<ITimeZoneProvider, LocalTimeZoneProvider>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseResponseCompression();
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapFallbackToPage("/_Host");

app.Run();