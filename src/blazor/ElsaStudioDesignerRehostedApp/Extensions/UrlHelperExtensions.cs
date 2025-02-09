using Microsoft.AspNetCore.Mvc;

namespace ElsaStudioDesignerRehostedApp.Extensions;

public static class UrlHelperExtensions
{
    public static string ToAbsoluteUrl(this IUrlHelper url, string virtualPath)
    {
        return url.GetBaseUrl() + url.Content(virtualPath);
    }
    
    public static string GetBaseUrl(this IUrlHelper url)
    {
        var request = url.ActionContext.HttpContext.Request;
        return request.Scheme + "://" + request.Host.ToUriComponent();
    }
}