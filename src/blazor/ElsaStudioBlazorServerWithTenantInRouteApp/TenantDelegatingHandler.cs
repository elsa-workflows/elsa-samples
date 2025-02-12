namespace ElsaStudioBlazorServerWithTenantInRouteApp;

/// <summary>
/// A delegating handler that inspects the HTTP context to retrieve the tenant ID from the PathBase
/// and includes it as an "X-Tenant-ID" header in the outgoing HTTP request.
/// </summary>
public class TenantDelegatingHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Add tenant ID from PathBase as a header
        var tenantId = httpContextAccessor.HttpContext?.Request.PathBase.Value?.Trim('/');
        if (!string.IsNullOrEmpty(tenantId))
        {
            request.Headers.Add("X-Tenant-ID", tenantId);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}