using Elsa.Agents;
using Elsa.Extensions;
using Elsa.Persistence.EFCore.Extensions;
using Elsa.Persistence.EFCore.Modules.Management;
using Elsa.Persistence.EFCore.Modules.Runtime;
using Elsa.Workflows.Runtime.Distributed.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Setup the Elsa Workflows Engine.
builder.Services.AddElsa(elsa =>
{
    elsa.UseWorkflowManagement(management =>
    {
        management.UseEntityFrameworkCore(ef => ef.UseSqlite());
        management.UseCache();
    });

    elsa.UseWorkflowRuntime(runtime =>
    {
        runtime.UseDistributedRuntime();
        runtime.UseEntityFrameworkCore(ef => ef.UseSqlite());
        runtime.UseCache();
    });

    elsa.UseWorkflowsApi();

    elsa.UseIdentity(identity =>
    {
        identity.UseAdminUserProvider();
        identity.TokenOptions = options => options.SigningKey = "super-secret-tamper-free-token-signing-key";
    });

    elsa.UseAgentActivities();
    elsa.UseDefaultAuthentication();
});

// Configure CORS to allow designer app hosted on a different origin to invoke the APIs.
services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("*")));
services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.Run();