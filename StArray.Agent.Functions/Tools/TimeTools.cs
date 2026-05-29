using Microsoft.Extensions.AI;
using StArray.Agent.Functions.Annotations;
using StArray.Agent.Functions.Core;

namespace StArray.Agent.Functions.Tools;

/// <summary>
/// 时间工具集：提供当前时间、UTC 时间和日期运算。
/// Time tools: provides current time, UTC time, and date arithmetic.
/// </summary>
public partial class TimeTools : ITools
{
    [AgentTool("Tool_Time_Now")]
    public ToolResult<string> GetNow()
    {
        return ToolResult<string>.Success(DateTime.Now.ToString("O"));
    }

    [AgentTool("Tool_Time_UtcNow")]
    public ToolResult<string> GetUtcNow()
    {
        return ToolResult<string>.Success(DateTime.UtcNow.ToString("O"));
    }

    [AgentTool("Tool_Time_AddDays")]
    public ToolResult<string> AddDays(
        [ToolParameter("Tool_Time_AddDays_dateString")] string dateString,
        [ToolParameter("Tool_Time_AddDays_days")] int days)
    {
        if (DateTime.TryParse(dateString, out var date))
        {
            return ToolResult<string>.Success(date.AddDays(days).ToString("O"));
        }
        return ToolResult<string>.Failure("无效的日期格式。");
    }
}