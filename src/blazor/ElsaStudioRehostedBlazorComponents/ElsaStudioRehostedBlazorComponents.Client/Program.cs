using Elsa.Studio.Workflows.Designer.Extensions;
using ElsaStudioRehostedBlazorComponents.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var configuration = builder.Configuration;
var bootstrapper = new Bootstrapper(configuration, builder.Services);
builder.RootComponents.RegisterCustomElsaStudioElements();
bootstrapper.Configure();

await builder.Build().RunAsync();