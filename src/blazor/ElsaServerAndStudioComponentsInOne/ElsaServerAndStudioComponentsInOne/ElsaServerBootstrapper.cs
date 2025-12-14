using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;

namespace ElsaServerAndStudioComponentsInOne;

public class ElsaServerBootstrapper(IConfiguration configuration, IServiceCollection services)
{
    public void Configure()
    {
        // Add services to the container.
        services.AddElsa(elsa =>
        {
            // Configure management feature to use EF Core.
            elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseSqlite()));

            elsa.UseWorkflowRuntime(runtime =>
            {
                runtime.UseEntityFrameworkCore(ef => ef.UseSqlite());
            });

            // Expose API endpoints.
            elsa.UseWorkflowsApi();

            // Add services for HTTP activities and workflow middleware.
            elsa.UseHttp();

            // Use timers.
            elsa.UseScheduling();

            // Configure identity so that we can create a default admin user.
            elsa.UseIdentity(identity =>
            {
                identity.UseAdminUserProvider();
                identity.TokenOptions = options =>
                {
                    options.SigningKey = "super-secret-tamper-free-token-signing-key";
                    options.AccessTokenLifetime = TimeSpan.FromDays(1);
                };
            });

            // Use default authentication (JWT).
            elsa.UseDefaultAuthentication(auth => auth.UseAdminApiKey());

            // Register custom activities.
            elsa.AddActivitiesFrom<Program>();

            // Register custom workflows.
            elsa.AddWorkflowsFrom<Program>();
        });
    }
}