using Elsa.Extensions;
using ElsaServerAndStudioComponentsInOne;
using ElsaServerAndStudioComponentsInOne.Client;
using ElsaServerAndStudioComponentsInOne.Components;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();
new ElsaServerBootstrapper(configuration, builder.Services).Configure();
new ElsaStudioBootstrapper(configuration, builder.Services).Configure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ElsaServerAndStudioComponentsInOne.Client._Imports).Assembly);
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.UseWorkflows();

app.Run();