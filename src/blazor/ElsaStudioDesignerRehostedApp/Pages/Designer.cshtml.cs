using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElsaStudioDesignerRehostedApp.Pages;

public class DesignerModel(ILogger<IndexModel> logger) : PageModel
{
    private readonly ILogger<IndexModel> _logger = logger;

    [FromRoute] public string CompanyId { get; set; } = null!;
    [FromRoute] public string WorkflowDefinitionId { get; set; } = null!;

    public void OnGet()
    {
    }
}