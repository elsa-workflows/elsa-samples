using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.Samples.AspNet.Onboarding.WorkflowServer.Workflows;
using WebhooksCore.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddElsa(elsa =>
{
    // Add workflow.
    elsa.AddWorkflow<OnboardingWorkflow>();

    // Configure management feature to use EF Core.
    elsa.UseWorkflowManagement();

    elsa.UseWorkflowRuntime();

    elsa.UseIdentity(identity =>
    {
        identity.TokenOptions = options =>
        {
            options.SigningKey = "c7dc81876a782d502084763fa322429fca015941eac90ce8ca7ad95fc8752035";
            options.AccessTokenLifetime = TimeSpan.FromDays(1);
        };
        
        identity.UseAdminUserProvider();
    });

    // Expose API endpoints.
    elsa.UseWorkflowsApi();

    // Use default authentication (JWT + ApiKey).
    elsa.UseDefaultAuthentication(auth => auth.UseAdminApiKey());

    elsa.UseJavaScript();

    // Enable SignalR for sending events to Elsa Studio for real-time updates.
    // Disabled until https://github.com/elsa-workflows/elsa-core/releases/tag/3.2.3
    // elsa.UseRealTimeWorkflows();
    
    // Use Webhooks feature.
    elsa.UseWebhooks(webhooks => webhooks.ConfigureSinks = options => builder.Configuration.GetSection("Webhooks:Sinks").Bind(options));
});

builder.Services.Configure<WebhookSinksOptions>(options => builder.Configuration.GetSection("Webhooks").Bind(options));

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithExposedHeaders("*")));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors();
app.UseHttpsRedirection();
app.MapHealthChecks("/");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.UseJsonSerializationErrorHandler();
app.UseWorkflows();

// Disabled until https://github.com/elsa-workflows/elsa-core/releases/tag/3.2.3
//app.UseWorkflowsSignalRHubs();
app.Run();