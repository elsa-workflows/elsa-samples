using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Identity;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add Elsa services.
services.AddElsa(elsa => elsa
    // Add the Fluent Storage workflow definition provider for reading workflows stored as JSON in the Workflows folder.
    .UseFluentStorageProvider()
    .UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseSqlite()))
    .UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore(ef => ef.UseSqlite()))
    .UseHttp()
    .UseJavaScript()
    .UseLiquid()

    // Configure identity with a default admin user.
    .UseIdentity(identity =>
    {
        identity.UseAdminUserProvider();
        identity.UseEntityFrameworkCore(ef => ef.UseSqlite());
        identity.TokenOptions = options =>
        {
            options.SigningKey = "very-long-secret-token-signing-key";
            options.AccessTokenLifetime = TimeSpan.FromDays(1);
        };
    })
    .UseDefaultAuthentication(auth => auth.UseAdminApiKey())
);

services.AddControllers();
services.AddHttpContextAccessor();
services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

// Configure middleware pipeline.
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// Configure the HTTP request pipeline.
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.MapControllers();
app.Run();