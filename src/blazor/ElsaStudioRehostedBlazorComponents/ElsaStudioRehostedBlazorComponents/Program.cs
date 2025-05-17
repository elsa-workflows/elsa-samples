using ElsaStudioRehostedBlazorComponents.Components;
using ElsaStudioRehostedBlazorComponents.Shared;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();
var bootstrapper = new Bootstrapper(configuration, builder.Services);
bootstrapper.Configure();

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
    .AddAdditionalAssemblies(typeof(ElsaStudioRehostedBlazorComponents.Client._Imports).Assembly);

app.Run();