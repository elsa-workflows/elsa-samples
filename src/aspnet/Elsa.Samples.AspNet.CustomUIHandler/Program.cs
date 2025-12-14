using Elsa.Extensions;
using Elsa.Persistence.EFCore.Extensions;
using Elsa.Persistence.EFCore.Modules.Management;
using Elsa.Persistence.EFCore.Modules.Runtime;
using Elsa.Samples.AspNet.CustomUIHandler;
using Elsa.Workflows;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddElsa(elsa =>
{
    elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseSqlite()));
    elsa.UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore(ef=>ef.UseSqlite()));
    
    elsa.UseWorkflowsApi();
    elsa.UseHttp();
    elsa.UseJavaScript();
    elsa.UseLiquid();
    
    elsa.UseIdentity(identity =>
    {
        identity.UseAdminUserProvider();
        identity.TokenOptions = options =>
        {
            options.SigningKey = "c7dc81876a782d502084763fa322429fca015941eac90ce8ca7ad95fc8752035";
            options.AccessTokenLifetime = TimeSpan.FromDays(1);
        };
    });
    
    elsa.UseDefaultAuthentication();
    elsa.AddActivity<VehicleActivity>();
});

builder.Services.AddScoped<IPropertyUIHandler, VehicleUIHandler>();
builder.Services.AddScoped<IPropertyUIHandler, RefreshUIHandler>();
builder.Services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.Map("/test",c=> c.UseWorkflows());
app.Run();

