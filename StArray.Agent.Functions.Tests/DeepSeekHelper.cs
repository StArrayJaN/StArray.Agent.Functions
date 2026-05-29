using System.ClientModel;
using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using MEAI = Microsoft.Extensions.AI;
using OAI = OpenAI.Chat;

#pragma warning disable SCME0001

namespace StArray.Agents.Functions.Tests;

/// <summary>
/// DeepSeek `reasoning_content` 回传补丁：每次响应后从原始 JSON 提取并注入。
/// Patches reasoning_content from raw JSON after each response to avoid 400 on tool calls.
/// </summary>
public static class DeepSeekHelper
{
    public static AIAgent CreateDeepSeekAgent(
        string apiKey, string model, string apiUrl,
        string instructions, string name, IList<AITool>? tools = null)
    {
        var client = new OpenAIClient(new ApiKeyCredential(apiKey),
            new OpenAIClientOptions { Endpoint = new Uri(apiUrl) });

        return client.GetChatClient(model).AsIChatClient().AsBuilder()
            .Use(PatchReasoning, getStreamingResponseFunc: null)
            .BuildAIAgent(new ChatClientAgentOptions
            {
                Name = name,
                ChatOptions = new MEAI.ChatOptions { Instructions = instructions, Tools = tools },
            });
    }

    /// <summary>
    /// 从 ChatResponse 原始 JSON 中提取 reasoning_content 并写回每条 Assistant 消息。
    /// </summary>
    private static async Task<ChatResponse> PatchReasoning(
        IEnumerable<MEAI.ChatMessage> msgs, MEAI.ChatOptions? opts,
        IChatClient inner, CancellationToken ct)
    {
        var resp = await inner.GetResponseAsync(msgs, opts, ct);
        if (resp.RawRepresentation is not OAI.ChatCompletion cc) return resp;

        var json = cc.Patch.GetJson("$.choices[0].message"u8);
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("reasoning_content", out var el)) return resp;

        var text = el.GetString();
        if (string.IsNullOrWhiteSpace(text)) return resp;

        foreach (var msg in resp.Messages)
        {
            if (msg.Role != MEAI.ChatRole.Assistant) continue;
            var patched = OAI.ChatMessage.CreateAssistantMessage(cc);
            patched.Patch.Set("$.reasoning_content"u8, text);
            msg.RawRepresentation = patched;
        }
        return resp;
    }
}
