using Elsa.Common.Features;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.Identity.Multitenancy;
using Elsa.Samples.AspNet.CustomTenants.Extensions;
using Elsa.Samples.AspNet.CustomTenants.Providers;
using Elsa.Samples.AspNet.CustomTenants.Stores;
using Elsa.Tenants.AspNetCore;
using Elsa.Tenants.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

services.AddScoped<ICompanyStore, StaticCompanyStore>();
services.AddControllers();
services.AddElsa(elsa =>
{
    // The custom UseTenantSqlite method dynamically resolves the connection string based on the tenant at runtime.
    elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseTenantSqlite()));
    elsa.UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore(ef => ef.UseTenantSqlite()));
    
    elsa.UseIdentity(identity =>
        {
            var identitySection = configuration.GetSection("Identity");
            var identityTokenSection = identitySection.GetSection("Tokens");
            identity.TokenOptions = options => identityTokenSection.Bind(options);
            identity.UseConfigurationBasedUserProvider(options => identitySection.Bind(options));
            identity.UseConfigurationBasedApplicationProvider(options => identitySection.Bind(options));
            identity.UseConfigurationBasedRoleProvider(options => identitySection.Bind(options));
        })
        .UseDefaultAuthentication();

    elsa.UseTenants(tenantsFeature =>
    {
        tenantsFeature.ConfigureMultitenancy(options =>
        {
            // Setup tenant resolver pipeline.
            options.TenantResolverPipelineBuilder
                .Append<CurrentUserTenantResolver>()
                .Append<HeaderTenantResolver>();
        });
    });
    
    // Install custom tenants provider.
    elsa.Use<MultitenancyFeature>(multitenancy => multitenancy.UseTenantsProvider<CustomTenantsProvider>());

    elsa
        .UseHttp()
        .AddFastEndpointsAssembly<Program>()
        .UseWorkflowsApi();

    elsa.AddWorkflowsFrom<Program>();
});

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseTenants();
app.MapControllers();
app.UseWorkflows();
app.UseWorkflowsApi();
app.Run();
