namespace StArray.Agent.Functions.Annotations;

/// <summary>
/// 标注方法不是工具方法（不被注册为 Agent Tool）。
/// Marks a method as not a tool method (not registered as an Agent Tool).
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class NotToolAttribute : Attribute
{
}
