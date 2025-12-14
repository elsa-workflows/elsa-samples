using Elsa.Studio.Workflows.Designer.Extensions;
using ElsaServerAndStudioComponentsInOne.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var configuration = builder.Configuration;
var bootstrapper = new ElsaStudioBootstrapper(configuration, builder.Services);
builder.RootComponents.RegisterCustomElsaStudioElements();
bootstrapper.Configure();

await builder.Build().RunAsync();