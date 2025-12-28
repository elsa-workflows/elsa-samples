using JetBrains.Annotations;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace Elsa.Samples.AspNet.CodeFirstAgents.Agents;

/// <summary>
/// Represents an AI-driven story-writing agent that leverages an external chat client
/// to generate stories based on the specified author, topic, and genre.
/// </summary>
[UsedImplicitly]
public class StoryWriterAgent(IChatClient chatClient)
{
    public async Task<string> WriteStoryAsync(string topic, string genre, CancellationToken cancellationToken = default)
    {
        var writer = chatClient.CreateAIAgent(
            name: "Writer",
            instructions: "Write a short story based on the provided topic."
        );

        var editor = chatClient.CreateAIAgent(
            name: "Editor",
            instructions: "Improve the draft: fix grammar, improve flow, and tighten the plot."
        );
        var workflow = AgentWorkflowBuilder.BuildSequential(writer, editor);
        var workflowAgent = workflow.AsAgent();
        var result = await workflowAgent.RunAsync(
            $"Write a short story about {topic} in the genre of {genre}.", 
            cancellationToken: cancellationToken);
        return result.Text;
    }
}

