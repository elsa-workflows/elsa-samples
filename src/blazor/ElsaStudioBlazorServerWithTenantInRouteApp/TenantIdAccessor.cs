namespace ElsaStudioBlazorServerWithTenantInRouteApp;

public class TenantIdAccessor(IHttpContextAccessor httpContextAccessor)
{
    public string? GetTenantIdFromUrl()
    {
        // Extract tenant ID from the URL.
        var path = httpContextAccessor.HttpContext!.Request.Path.Value!;
        var segments = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);

        // The first segment is the tenant ID.
        return segments.Length > 0 ? segments[0] : null;
    }
}