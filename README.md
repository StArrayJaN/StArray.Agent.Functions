# StArray.Agent.Functions

.NET Agent 工具函数库，基于 Microsoft Agent Framework (MAF)。

## 项目结构

```
StArray.Agent.Functions/
  StArray.Agent.Functions/           -- 主项目
    Annotations/                     -- AgentToolAttribute, ToolParameterAttribute, NotToolAttribute
    Core/                            -- FunctionFactory, ToolResult<T>
    Tools/                           -- 工具实现 (MathTools, FileTools, TimeTools, TextTools, CodeTools)
      Win32/                         -- WindowTools
    Resources/                       -- 本地化 (.resx)
  StArray.Agent.Functions.Diagnostics/  -- 诊断项目
    FunctionFactoryGenerator         -- 源生成器：扫描 partial ITools 类，自动生成 GetTools()
    AgentToolParameterAnalyzer       -- 分析器：SAF0001 / SAF0002 诊断
    AgentToolParameterCodeFixProvider -- CodeFix：自动添加/移除特性
    Resources/                       -- 分析器本地化
  StArray.Agent.Functions.Tests/     -- 测试项目
```

## 快速开始

```csharp
// 1. 定义一个 partial 工具类，实现 ITools
public partial class MyTools : ITools
{
    [AgentTool("Tool_My_DoSomething")]
    public ToolResult<string> DoSomething(
        [ToolParameter("Tool_My_DoSomething_input")] string input)
    {
        return ToolResult<string>.Success(input.ToUpper());
    }
}

// 2. 源生成器自动生成 GetTools()

// 3. 注册到 Agent
var tools = FunctionFactory.MyTools.GetTools();
var agent = new ChatClientAgent(...) { Tools = tools };
```

## 工具方法规范

- 类必须为 `partial class` 并实现 `ITools`
- 方法标记 `[AgentTool("Tool_XXX_YYY")]` ，键名格式 `Tool_{类名}_{方法名}`
- 每个参数标记 `[ToolParameter("Tool_XXX_YYY_paramName")]` ，键名格式 `{AgentTool键}_{参数名}`
- 返回类型使用 `ToolResult<T>` 统一包装成功/失败

## 分析器诊断

| ID | 描述 |
|----|------|
| SAF0001 | [AgentTool] 方法参数缺少 [ToolParameter] |
| SAF0002 | 非 [AgentTool] 方法的参数误用 [ToolParameter] |

两种警告均提供 CodeFix（Alt+Enter）一键修复。

## 本地化

工具描述和参数说明通过 `.resx` 资源文件本地化：

- `Resources/Localization.resx` -- 中文（默认）
- `Resources/Localization.en.resx` -- 英文

分析器消息：

- `Resources/AnalyzerResources.resx` -- 中文（默认）
- `Resources/AnalyzerResources.en.resx` -- 英文
