using Elsa.Extensions;
using Elsa.Persistence.EFCore.Extensions;
using Elsa.Persistence.EFCore.Modules.Management;
using Elsa.Persistence.EFCore.Modules.Runtime;
using Elsa.Samples.AspNet.CodeFirstAgents.Agents;
using Elsa.Workflows.Runtime.Distributed.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;
var openApiKey = configuration["OpenAI:ApiKey"]!;

services.AddOpenAIChatClient("gpt-4o", openApiKey);
services.AddSingleton<StoryWriterAgent>();

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
        identity.TokenOptions = options => options.SigningKey = "secret-signing-key-4d28f39b-7761-42ea-8985-a38faaba4b2d";
    });

    elsa.AddActivityHost<StoryWriterAgent>();
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

app.MapGet("/", () => "Hello World!");
app.MapPost("/write-story", async (StoryWriterAgent agent, CancellationToken cancellationToken) => await agent.WriteStoryAsync("A haunted lighthouse", "thriller", cancellationToken));

app.Run();