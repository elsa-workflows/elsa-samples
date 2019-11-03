using Elsa.Samples.UserRegistration.Web.Activities;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.Samples.UserRegistration.Web.Extensions
{
    public static class UserServiceCollectionExtensions
    {
        public static IServiceCollection AddUserActivities(this IServiceCollection services)
        {
            return services
                .AddActivity<CreateUser>()
                .AddActivity<ActivateUser>()
                .AddActivity<DeleteUser>();
        }
    }
}