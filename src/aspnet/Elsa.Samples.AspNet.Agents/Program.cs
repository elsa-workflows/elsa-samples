using Elsa.Agents;
using Elsa.Extensions;
using Elsa.Persistence.EFCore.Extensions;
using Elsa.Persistence.EFCore.Modules.Management;
using Elsa.Persistence.EFCore.Modules.Runtime;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddElsa(elsa =>
{
    elsa.UseWorkflowManagement(management =>
    {
        management.UseEntityFrameworkCore(ef => ef.UseSqlite());
        management.UseCache();
    });
    elsa.UseWorkflowRuntime(runtime =>
    {
        runtime.UseEntityFrameworkCore(ef => ef.UseSqlite());
        runtime.UseCache();
    });
    elsa.UseWorkflowsApi();
    elsa.UseHttp(http =>
    {
        http.ConfigureHttpOptions = options => builder.Configuration.GetSection("Http").Bind(options);
        http.UseCache();
    });
    elsa.UseScheduling();
    elsa.UseIdentity(identity =>
    {
        identity.UseAdminUserProvider();
        identity.TokenOptions = options => builder.Configuration.GetSection("Identity:Tokens").Bind(options);
    });

    elsa.UseDefaultAuthentication(auth => auth.UseAdminApiKey());
    elsa.UseJavaScript();
    elsa.UseLiquid();
    elsa.UseWorkflowContexts();
    elsa.AddActivitiesFrom<Program>();
    elsa.AddWorkflowsFrom<Program>();

    elsa
        .UseAgentActivities()
        .UseAgentPersistence(persistence => persistence.UseEntityFrameworkCore(ef => ef.UseSqlite()))
        .UseAgentsApi();
});

builder.Services.Configure<AgentOptions>(options => builder.Configuration.GetSection("Agents").Bind(options));
builder.Services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("*")));

var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.UseWorkflows();
app.Run();