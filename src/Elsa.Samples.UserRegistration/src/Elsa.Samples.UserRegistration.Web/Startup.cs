using Elsa.Activities.Email.Extensions;
using Elsa.Activities.Http.Extensions;
using Elsa.Activities.Timers.Extensions;
using Elsa.Dashboard.Extensions;
using Elsa.Extensions;
using Elsa.Persistence.MongoDb.Extensions;
using Elsa.Samples.UserRegistration.Web.Extensions;
using Elsa.Samples.UserRegistration.Web.Handlers;
using Elsa.Samples.UserRegistration.Web.Models;
using Elsa.Samples.UserRegistration.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Elsa.Samples.UserRegistration.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services
                // Add Elsa services.
                .AddElsa(
                    elsa =>
                    {
                        // Configure Elsa to use the MongoDB provider.
                        elsa.AddMongoDbStores(Configuration, "UserRegistration", "MongoDb");
                    })

                // Add Elsa Dashboard services.
                .AddElsaDashboard()

                // Add the activities we want to use.
                .AddEmailActivities(options => options.Bind(Configuration.GetSection("Elsa:Smtp")))
                .AddHttpActivities(options => options.Bind(Configuration.GetSection("Elsa:Http")))
                .AddTimerActivities(options => options.Bind(Configuration.GetSection("Elsa:Timers")))
                .AddUserActivities()

                // Add our PasswordHasher service.
                .AddSingleton<IPasswordHasher, PasswordHasher>()

                // Add a MongoDB collection for our User model.
                .AddMongoDbCollection<User>("Users")

                // Add our liquid handler.
                .AddNotificationHandlers(typeof(LiquidConfigurationHandler));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();

            // Add Elsa's middleware to handle HTTP requests to workflows.
            app.UseHttpActivities();

            app.UseRouting();

            app.UseEndpoints(
                endpoints =>
                {
                    // Blazor stuff.
                    endpoints.MapBlazorHub();
                    endpoints.MapFallbackToPage("/_Host");

                    // Attribute-based routing stuff.
                    endpoints.MapControllers();
                });
        }
    }
}