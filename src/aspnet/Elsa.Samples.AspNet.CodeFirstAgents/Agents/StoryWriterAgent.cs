using Elsa.Agents;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace Elsa.Samples.AspNet.CodeFirstAgents.Agents;

/// <summary>
/// Represents an AI-driven story-writing agent that leverages an external chat client
/// to generate stories based on the specified author, topic, and genre.
/// This agent is completely unrelated to Elsa and can be instantiated and executed by any component.
/// </summary>
public class StoryWriterAgent(IChatClient chatClient, ILoggerFactory loggerFactory) : IAgent
{
    public string Author { get; set; }
    public string Topic { get; set; }
    public string Genre { get; set; }
    
    public async Task<AgentRunResponse> RunAsync(AgentExecutionContext context)
    {
        // Local tools for the writer agent.
        string GetAuthor() => Author;
        string FormatStory(string title, string a, string story) => $"Title: {title}\nAuthor: {a}\n\n{story}";
        
        var cancellationToken = context.CancellationToken;
        var writer = chatClient.CreateAIAgent(
            new()
            {
                Name = "Writer",
                ChatOptions = new()
                {
                    Instructions = """
                                   You are a creative writing assistant who crafts vivid, 
                                   well-structured stories with compelling characters based on user prompts, 
                                   and formats them after writing.
                                   """,
                    Tools =
                    [
                        AIFunctionFactory.Create(GetAuthor),
                        AIFunctionFactory.Create(FormatStory)
                    ],
                }
            },
            loggerFactory);

        var editor = chatClient.CreateAIAgent(
            new()
            {
                Name = "Editor",
                ChatOptions = new()
                {
                    Instructions = """
                                   You are an editor who improves a writer’s draft by providing 4–8 concise recommendations and 
                                   a fully revised Markdown document, focusing on clarity, coherence, accuracy, and alignment.
                                   """
                }
            },
            loggerFactory);

        var narrativeOrchestrator = AgentWorkflowBuilder.BuildSequential(writer, editor);
        var narrativeOrchestratorAgent = narrativeOrchestrator.AsAgent();
        var response = await narrativeOrchestratorAgent.RunAsync($"Write a story about {Topic} in the genre of {Genre} written by {Author}.", cancellationToken: cancellationToken);
        return response;
    }
}

