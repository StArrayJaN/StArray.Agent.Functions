using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using StArray.Agent.Functions.SourceGenerator.Resources;

#pragma warning disable RS2008 // 分析器发布跟踪（本项目中不需要）
#pragma warning disable RS1033 // 中文句号已满足要求，Roslyn 仅检查 ASCII 标点
#pragma warning disable RS1038 // Analyzer 与 CodeFixProvider 同程序集

namespace StArray.Agent.Functions.SourceGenerator;

/// <summary>
/// 分析器：双向检测 [AgentTool] 与 [ToolParameter] 的匹配性。
/// 诊断消息通过 AnalyzerResources.resx 本地化。
/// Analyzer: bidirectional check for [AgentTool] / [ToolParameter] pairing.
/// Diagnostic messages localized via AnalyzerResources.resx.
/// <list type="bullet">
/// <item>SAF0001: [AgentTool] 方法参数缺少 [ToolParameter]</item>
/// <item>SAF0002: 非 [AgentTool] 方法的参数误用了 [ToolParameter]</item>
/// </list>
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AgentToolParameterAnalyzer : DiagnosticAnalyzer
{
    /// <summary>SAF0001: Agent 工具方法参数缺少 [ToolParameter] 特性。</summary>
    public const string MissingParamDiagId = "SAF0001";
    /// <summary>SAF0002: 非 Agent 工具方法的参数误用了 [ToolParameter] 特性。</summary>
    public const string OrphanParamDiagId = "SAF0002";

    private const string Category = "Usage";

    // SAF0001：正向检测——有 [AgentTool] 但参数缺 [ToolParameter]
    // SAF0001: Forward check — [AgentTool] present but param missing [ToolParameter]
    private static readonly DiagnosticDescriptor MissingParamRule = new(
        MissingParamDiagId,
        new LocalizableResourceString("SAF0001_Title",
            AnalyzerResources.ResourceManager, typeof(AnalyzerResources)),
        new LocalizableResourceString("SAF0001_MessageFormat",
            AnalyzerResources.ResourceManager, typeof(AnalyzerResources)),
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        new LocalizableResourceString("SAF0001_Description",
            AnalyzerResources.ResourceManager, typeof(AnalyzerResources)));

    // SAF0002：反向检测——没有 [AgentTool] 但参数误加了 [ToolParameter]
    // SAF0002: Reverse check — no [AgentTool] but param has pointless [ToolParameter]
    private static readonly DiagnosticDescriptor OrphanParamRule = new(
        OrphanParamDiagId,
        new LocalizableResourceString("SAF0002_Title",
            AnalyzerResources.ResourceManager, typeof(AnalyzerResources)),
        new LocalizableResourceString("SAF0002_MessageFormat",
            AnalyzerResources.ResourceManager, typeof(AnalyzerResources)),
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        new LocalizableResourceString("SAF0002_Description",
            AnalyzerResources.ResourceManager, typeof(AnalyzerResources)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [MissingParamRule, OrphanParamRule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        var methodDecl = (MethodDeclarationSyntax)context.Node;

        if (methodDecl.ParameterList.Parameters.Count == 0)
            return;

        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDecl, context.CancellationToken);
        if (methodSymbol is not IMethodSymbol { IsAbstract: false })
            return;

        // 判断方法是否有 [AgentTool] / Check if method has [AgentTool]
        var hasAgentTool = HasOurAttribute(methodSymbol.GetAttributes(), "AgentToolAttribute", "AgentTool");

        // 遍历参数，做双向检查 / Iterate params, do bidirectional check
        foreach (var param in methodSymbol.Parameters)
        {
            var hasToolParameter = HasOurAttribute(param.GetAttributes(), "ToolParameterAttribute", "ToolParameter");

            if (hasAgentTool && !hasToolParameter)
            {
                // SAF0001: 有 [AgentTool] 但参数缺 [ToolParameter]
                context.ReportDiagnostic(Diagnostic.Create(
                    MissingParamRule,
                    param.Locations.FirstOrDefault(),
                    param.Name,
                    methodDecl.Identifier.Text));
            }
            else if (!hasAgentTool && hasToolParameter)
            {
                // SAF0002: 没有 [AgentTool] 但参数误加了 [ToolParameter]
                context.ReportDiagnostic(Diagnostic.Create(
                    OrphanParamRule,
                    param.Locations.FirstOrDefault(),
                    param.Name,
                    methodDecl.Identifier.Text));
            }
        }
    }

    /// <summary>
    /// 检查特性列表中是否存在来自 StArray.Agent.Functions.Annotations 的目标特性。
    /// Checks whether the target attribute from our annotations namespace exists in the list.
    /// </summary>
    private static bool HasOurAttribute(
        System.Collections.Immutable.ImmutableArray<AttributeData> attributes,
        string name1,
        string name2)
    {
        return attributes.Any(attr =>
            attr.AttributeClass is { } ac
            && (ac.Name == name1 || ac.Name == name2)
            && ac.ContainingNamespace?.ToDisplayString() == "StArray.Agent.Functions.Annotations");
    }
}
