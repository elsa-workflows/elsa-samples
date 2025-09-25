using Elsa.Expressions.Models;
using Elsa.Workflows;
using Elsa.Workflows.Runtime.Activities;

namespace Elsa.Samples.AspNet.WorkflowServer.Activities;

public class MyCustomEvent : EventBase<object>
{
    protected override string GetEventName(ExpressionExecutionContext context)
    {
        return "MyCustomEvent";
    }

    protected override void OnEventReceived(ActivityExecutionContext context, object? eventData)
    {
        Console.WriteLine("MyCustomEvent received with data: {0}", eventData);
    }
}