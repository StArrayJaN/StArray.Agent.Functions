using System.ComponentModel;
using System.Resources;
using StArray.Agent.Functions.Resources;

namespace StArray.Agent.Functions.Annotations;

/// <summary>
/// 标记方法为 Agent 可调用工具，描述通过本地化键从资源文件加载。
/// Marks a method as an Agent-callable tool; description loaded from resource file via localization key.
/// </summary>
public class AgentToolAttribute : DescriptionAttribute
{
    private readonly Type _resourceType;
    private readonly bool _isLocalize;

    public AgentToolAttribute(string key, bool isLocalize = true, Type resourceType = null)
    {
        Description = key;
        _isLocalize = isLocalize;
        _resourceType = resourceType;
    }

    public override string Description
    {
        get
        {
            if (!_isLocalize) return field;
            var rm = _resourceType == null 
                ? Localization.ResourceManager 
                : new ResourceManager(_resourceType);
            return rm.GetString(field) ?? field;
        }
    }
}