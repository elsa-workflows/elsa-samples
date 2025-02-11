using Microsoft.AspNetCore.Components;

namespace ElsaStudioBlazorWebAssemblyApp.Services;

public class TenantIdAccessor(NavigationManager navigationManager)
{
    public string? GetTenantIdFromUrl()
    {
        // Extract tenant ID from the URL.
        var uri = new Uri(navigationManager.Uri);
        var segments = uri.AbsolutePath.Trim('/').Split('/');

        // Example: Tenant ID is the first path segment.
        return segments.Length > 0 ? segments[0] : null;
    }
}