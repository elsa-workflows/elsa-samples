using Elsa.Studio.Core.BlazorServer.Extensions;
using Elsa.Studio.Dashboard.Extensions;
using Elsa.Studio.Extensions;
using Elsa.Studio.Localization.Time;
using Elsa.Studio.Localization.Time.Providers;
using Elsa.Studio.Login.BlazorServer.Extensions;
using Elsa.Studio.Login.HttpMessageHandlers;
using Elsa.Studio.Models;
using Elsa.Studio.Shell;
using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.Webhooks.Extensions;
using Elsa.Studio.WorkflowContexts.Extensions;
using Elsa.Studio.Workflows.Designer.Extensions;
using Elsa.Studio.Workflows.Extensions;
using ElsaStudioBlazorServerWithTenantInRouteApp;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

var backendApiConfig = new BackendApiConfig
{
    ConfigureBackendOptions = options => configuration.GetSection("Backend").Bind(options),
    ConfigureHttpClientBuilder = options =>
    {
        options.AuthenticationHandler = typeof(AuthenticatingApiHttpMessageHandler);
        options.ConfigureHttpClientBuilder = clientBuilder => clientBuilder.AddHttpMessageHandler<TenantDelegatingHandler>();
    },
};

services.AddRazorPages();
services.AddServerSideBlazor();
services.AddCore();
services.AddShell(options => configuration.GetSection("Shell").Bind(options));
services.AddRemoteBackend(backendApiConfig);
services.AddTransient<TenantDelegatingHandler>();
services.AddLoginModule();
services.AddDashboardModule();
services.AddWorkflowsModule();
services.AddWorkflowContextsModule();
services.AddWebhooksModule();
services.AddScoped<ITimeZoneProvider, LocalTimeZoneProvider>();
services.AddSingleton<TenantIdAccessor>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseResponseCompression();
    app.UseHsts();
}

// Dynamically detect and apply the base path, such as "/tenant-1".
app.Use((context, next) =>
{
    var tenantIdAccessor = app.Services.GetRequiredService<TenantIdAccessor>();
    var tenantId = tenantIdAccessor.GetTenantIdFromUrl();
    if (!string.IsNullOrEmpty(tenantId))
    {
        context.Request.PathBase = $"/{tenantId}";
        context.Request.Path = context.Request.Path.Value[$"/{tenantId}".Length..];
    }
    return next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();