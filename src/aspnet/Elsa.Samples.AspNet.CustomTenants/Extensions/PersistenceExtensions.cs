using Elsa.Common.Multitenancy;
using Elsa.EntityFrameworkCore;
using Elsa.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Elsa.Samples.AspNet.CustomTenants.Extensions;

public static class PersistenceExtensions
{
    public static TFeature UseTenantSqlite<TFeature, TDbContext>(this PersistenceFeatureBase<TFeature, TDbContext> feature, 
        Action<SqliteDbContextOptionsBuilder>? configure = null)
        where TDbContext : ElsaDbContextBase
        where TFeature : PersistenceFeatureBase<TFeature, TDbContext>
    {
        var options = new ElsaDbContextOptions();
        return feature.UseSqlite(GetConnectionString, options, configure);
    }

    private static string GetConnectionString(IServiceProvider serviceProvider)
    {
        var tenantAccessor = serviceProvider.GetRequiredService<ITenantAccessor>();
        var currentTenant = tenantAccessor.Tenant;
        var connectionString = currentTenant?.Configuration.GetConnectionString("Sqlite") ?? serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("Sqlite")!;
        return connectionString;
    }
}