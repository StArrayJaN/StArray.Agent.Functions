using Microsoft.Extensions.AI;
using StArray.Agent.Functions.Annotations;
using StArray.Agent.Functions.Core;

namespace StArray.Agent.Functions.Tools;

/// <summary>
/// 代码工具集：提供 JSON 校验、代码行数统计等功能。
/// Code tools: provides JSON validation, line counting, and other code utilities.
/// </summary>
public partial class CodeTools : ITools
{
    [AgentTool("Tool_Code_ValidateJson")]
    public ToolResult<string> ValidateJson(
        [ToolParameter("Tool_Code_ValidateJson_json")] string json)
    {
        try
        {
            System.Text.Json.JsonDocument.Parse(json);
            return ToolResult<string>.Success("有效的 JSON。");
        }
        catch (Exception ex)
        {
            return ToolResult<string>.Failure($"无效的 JSON：{ex.Message}");
        }
    }

    [AgentTool("Tool_Code_CountLines")]
    public ToolResult<int> CountLines(
        [ToolParameter("Tool_Code_CountLines_code")] string code)
    {
        if (string.IsNullOrEmpty(code))
            return ToolResult<int>.Success(0);
        return ToolResult<int>.Success(
            code.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries).Length);
    }
}