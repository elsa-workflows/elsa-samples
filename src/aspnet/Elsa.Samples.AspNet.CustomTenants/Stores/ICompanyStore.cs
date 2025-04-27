using Elsa.Samples.AspNet.CustomTenants.Entities;

namespace Elsa.Samples.AspNet.CustomTenants.Stores;

public interface ICompanyStore
{
    Task<IEnumerable<Company>> ListAsync(CancellationToken cancellationToken = default);
}