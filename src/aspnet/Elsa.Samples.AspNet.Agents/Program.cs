using Elsa.Agents;
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddElsa(elsa =>
{
    elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseSqlite()));
    elsa.UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore(ef => ef.UseSqlite()));
    elsa.UseWorkflowsApi();
    elsa.UseHttp();
    elsa.UseScheduling();
    elsa.UseIdentity(identity =>
    {
        identity.UseAdminUserProvider();
        identity.TokenOptions = options => builder.Configuration.GetSection("Identity:Tokens").Bind(options);
    });

    elsa.UseDefaultAuthentication(auth => auth.UseAdminApiKey());
    elsa.UseJavaScript();
    elsa.UseLiquid();
    elsa.AddActivitiesFrom<Program>();
    elsa.AddWorkflowsFrom<Program>();

    elsa
        .UseAgentActivities()
        .UseAgentPersistence(persistence => persistence.UseEntityFrameworkCore(ef => ef.UseSqlite()))
        .UseAgentsApi();
});

builder.Services.Configure<AgentsOptions>(options => builder.Configuration.GetSection("Agents").Bind(options));
builder.Services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("*")));

var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.UseWorkflows();
app.Run();