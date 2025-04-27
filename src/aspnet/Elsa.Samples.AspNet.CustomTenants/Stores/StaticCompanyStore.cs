using Elsa.Samples.AspNet.CustomTenants.Entities;

namespace Elsa.Samples.AspNet.CustomTenants.Stores;

public class StaticCompanyStore : ICompanyStore
{
    public Task<IEnumerable<Company>> ListAsync(CancellationToken cancellationToken = default)
    {
        var companies = new List<Company>
        {
            CreateCompany(1, "Acme"),
            CreateCompany(2, "Contoso"),
            CreateCompany(3, "Fabrikam")
        };

        return Task.FromResult<IEnumerable<Company>>(companies);
    }
    
    private Company CreateCompany(int id, string name)
    {
        return new()
        {
            Id = id,
            Name = name,
            ConnectionString = $"Data Source={name.ToLowerInvariant()}.sqlite.db;Cache=Shared;"
        };
    }
}