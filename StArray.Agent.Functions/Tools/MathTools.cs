using Microsoft.Extensions.AI;
using StArray.Agent.Functions.Annotations;
using StArray.Agent.Functions.Core;

namespace StArray.Agent.Functions.Tools;

/// <summary>
/// 数学工具集：提供加减乘除等基本运算。
/// Math tools: provides basic arithmetic operations (add, subtract, multiply, divide).
/// </summary>
public partial class MathTools : ITools
{
    [AgentTool("Tool_Math_Add")]
    public ToolResult<int> Add(
        [ToolParameter("Tool_Math_Add_a")] int a,
        [ToolParameter("Tool_Math_Add_b")] int b)
    {
        return ToolResult<int>.Success(a + b);
    }

    [AgentTool("Tool_Math_Subtract")]
    public ToolResult<int> Subtract(
        [ToolParameter("Tool_Math_Subtract_a")] int a,
        [ToolParameter("Tool_Math_Subtract_b")] int b)
    {
        return ToolResult<int>.Success(a - b);
    }

    [AgentTool("Tool_Math_Multiply")]
    public ToolResult<int> Multiply(
        [ToolParameter("Tool_Math_Multiply_a")] int a,
        [ToolParameter("Tool_Math_Multiply_b")] int b)
    {
        return ToolResult<int>.Success(a * b);
    }

    [AgentTool("Tool_Math_Divide")]
    public ToolResult<double> Divide(
        [ToolParameter("Tool_Math_Divide_a")] double a,
        [ToolParameter("Tool_Math_Divide_b")] double b)
    {
        return b != 0
            ? ToolResult<double>.Success(a / b)
            : ToolResult<double>.Failure("除数不能为零。");
    }
}