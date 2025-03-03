using Elsa.Common.Models;
using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Filters;
using Elsa.Workflows.Options;
using Elsa.Workflows.Runtime;
using Microsoft.AspNetCore.Mvc;

namespace Elsa.Samples.AspNet.WorkflowInvocation.Controllers;

[Route("my-workflows")]
[ApiController]
public class MyWorkflowsController(IWorkflowDefinitionService workflowDefinitionService, IWorkflowInvoker workflowInvoker) : ControllerBase
{
    [Route("products/{id}")]
    public async Task<IActionResult> GetProduct(int id, CancellationToken cancellationToken)
    {
        var filter = new WorkflowDefinitionFilter
        {
            Name = "Get Product",
            VersionOptions = VersionOptions.Published
        };
        var getProductWorkflow = await workflowDefinitionService.FindWorkflowGraphAsync(filter, cancellationToken) ?? throw new("Could not find workflow.");
        var options = new RunWorkflowOptions { Input = new Dictionary<string, object> { ["ProductId"] = id } };
        var result = await workflowInvoker.InvokeAsync(getProductWorkflow, options, cancellationToken);
        result.WorkflowState.Output.TryGetValue("Product", out var product);
        return Ok(product);
    }
}