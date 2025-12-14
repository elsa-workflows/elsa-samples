using Elsa.Extensions;
using Elsa.Persistence.EFCore.Modules.Management;
using Elsa.Persistence.EFCore.Modules.Runtime;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add Elsa services.
services.AddElsa(elsa => elsa
    // Add the Fluent Storage workflow definition provider.
    .UseFluentStorageProvider()

    // Enable the Elsa DSL.
    .UseWorkflowManagement(management =>
    {
        management.UseEntityFrameworkCore();
    })
    .UseWorkflowRuntime(runtime =>
    {
        runtime.UseEntityFrameworkCore();
    })

    // Expose API endpoints.
    .UseWorkflowsApi()

    // Configure identity so that we can create a default admin user.
    .UseIdentity(identity =>
    {
        identity.UseAdminUserProvider();
        identity.TokenOptions = options =>
        {
            options.SigningKey = "secret-token-signing-key-that-is-very-long-and-secure";
            options.AccessTokenLifetime = TimeSpan.FromDays(1);
        };
    })

    // Use default authentication (JWT).
    .UseDefaultAuthentication(auth => auth.UseAdminApiKey())

    // Use HTTP activities.
    .UseHttp()
);


// Configure middleware pipeline.
var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.UseWorkflows();
app.Run();