using Microsoft.Extensions.AI;
using StArray.Agent.Functions.Annotations;
using StArray.Agent.Functions.Core;

namespace StArray.Agent.Functions.Tools;

/// <summary>
/// 文本工具集：提供文本长度计算、大小写转换、字符串拼接等功能。
/// Text tools: provides text length calculation, case conversion, string concatenation, etc.
/// </summary>
public partial class TextTools : ITools
{
    [AgentTool("Tool_Text_Length")]
    public ToolResult<int> GetLength(
        [ToolParameter("Tool_Text_Length_text")] string text)
    {
        return ToolResult<int>.Success(text.Length);
    }

    [AgentTool("Tool_Text_ToUpper")]
    public ToolResult<string> ToUpper(
        [ToolParameter("Tool_Text_ToUpper_text")] string text)
    {
        return ToolResult<string>.Success(text.ToUpperInvariant());
    }

    [AgentTool("Tool_Text_ToLower")]
    public ToolResult<string> ToLower(
        [ToolParameter("Tool_Text_ToLower_text")] string text)
    {
        return ToolResult<string>.Success(text.ToLowerInvariant());
    }

    [AgentTool("Tool_Text_Concat")]
    public ToolResult<string> Concat(
        [ToolParameter("Tool_Text_Concat_first")] string first,
        [ToolParameter("Tool_Text_Concat_second")] string second)
    {
        return ToolResult<string>.Success(first + second);
    }
}