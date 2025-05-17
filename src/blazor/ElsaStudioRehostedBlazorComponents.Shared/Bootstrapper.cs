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
using Elsa.Studio.Workflows.Extensions;
using ElsaStudioRehostedBlazorComponents.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ElsaStudioRehostedBlazorComponents.Shared;

public class Bootstrapper(IConfiguration configuration, IServiceCollection services)
{
    public void Configure()
    {
        var clientOptions = new ElsaClientOptions();
        configuration.GetSection("Backend").Bind(clientOptions);
        services.Configure<BackendOptions>(options => options.Url = clientOptions.BaseAddress);

        var backendApiConfig = new BackendApiConfig
        {
            ConfigureHttpClientBuilder = options =>
            {
                options.ApiKey = clientOptions.ApiKey;
                options.BaseAddress = clientOptions.BaseAddress;
                options.AuthenticationHandler = typeof(ApiKeyHttpMessageHandler); // TODO: replace with your own handler that e.g. attaches a bearer token when implementing a login procedure.
            }, 
        };

        var localizationConfig = new LocalizationConfig
        {
            ConfigureLocalizationOptions = options => configuration.GetSection("Localization").Bind(options),
        };

        services.AddCore();
        services.AddShell();
        services.AddRemoteBackend(backendApiConfig);
        services.Replace(ServiceDescriptor.Scoped<IRemoteBackendAccessor, ComponentRemoteBackendAccessor>());
        services.AddWorkflowsModule();
        services.AddLocalizationModule(localizationConfig);
        services.AddScoped<ITimeZoneProvider, LocalTimeZoneProvider>();
    }   
}