using Elsa.Extensions;
using Elsa.Persistence.EFCore.Extensions;
using Elsa.Persistence.EFCore.Modules.Identity;
using Elsa.Persistence.EFCore.Modules.Management;
using Elsa.Persistence.EFCore.Modules.Runtime;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add Elsa services.
services.AddElsa(elsa => elsa
    // Add the Fluent Storage workflow definition provider.
    .UseFluentStorageProvider()

    // Expose API endpoints.
    .UseWorkflowsApi()
    
    .UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseSqlite()))
    .UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore(ef => ef.UseSqlite()))

    // Configure identity so that we can create a default admin user.
    .UseIdentity(identity =>
    {
        identity.UseAdminUserProvider();
        identity.UseEntityFrameworkCore(ef => ef.UseSqlite());
        identity.TokenOptions = options =>
        {
            options.SigningKey = "secret-token-signing-key";
            options.AccessTokenLifetime = TimeSpan.FromDays(1);
        };
    })

    // HTTP workflows.
    .UseHttp()

    // Use default authentication (JWT).
    .UseDefaultAuthentication(auth => auth.UseAdminApiKey())
);

services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

// Configure middleware pipeline.
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.UseWorkflowsApi();
app.UseWorkflows();
app.Run();