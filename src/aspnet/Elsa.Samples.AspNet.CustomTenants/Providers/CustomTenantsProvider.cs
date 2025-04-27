using Elsa.Common.Multitenancy;
using Elsa.Samples.AspNet.CustomTenants.Entities;
using Elsa.Samples.AspNet.CustomTenants.Stores;

namespace Elsa.Samples.AspNet.CustomTenants.Providers;

public class CustomTenantsProvider(ICompanyStore companyStore) : ITenantsProvider
{
    public async Task<IEnumerable<Tenant>> ListAsync(CancellationToken cancellationToken = default)
    {
        var defaultTenant = Tenant.Default;
        var companies = await companyStore.ListAsync(cancellationToken);

        return companies.Select(x => new Tenant
        {
            Id = x.Id.ToString(),
            Name = x.Name,
            Configuration = CreateConfiguration(x)
        }).Prepend(defaultTenant);
    }

    public async Task<Tenant?> FindAsync(TenantFilter filter, CancellationToken cancellationToken = default)
    {
        var tenants = await ListAsync(cancellationToken);
        return filter.Apply(tenants.AsQueryable()).FirstOrDefault();
    }

    private IConfiguration CreateConfiguration(Company company)
    {
        var connectionString = company.ConnectionString;
        var dictionary = new Dictionary<string, string?> { ["CONNECTIONSTRINGS:SQLITE"] = connectionString };
        return new ConfigurationBuilder().AddInMemoryCollection(dictionary).Build();
    }
}