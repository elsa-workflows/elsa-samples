using Elsa.Studio.Contracts;
using Elsa.Studio.Models;
using Elsa.Studio.Options;
using Microsoft.Extensions.Options;

namespace ElsaStudioRehostedBlazorComponents.Client.Services;

/// <summary>
/// A default implementation of <see cref="IRemoteBackendAccessor"/> that uses the <see cref="BackendOptions"/> to determine the URL of the remote backend.
/// </summary>
public class ComponentRemoteBackendAccessor(IOptions<BackendOptions> options) : IRemoteBackendAccessor
{
    /// <inheritdoc />
    public RemoteBackend RemoteBackend => new(options.Value.Url);
}