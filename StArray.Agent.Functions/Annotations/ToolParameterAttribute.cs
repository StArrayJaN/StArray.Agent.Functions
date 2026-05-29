using System.ComponentModel;
using System.Resources;
using StArray.Agent.Functions.Resources;

namespace StArray.Agent.Functions.Annotations;

/// <summary>
/// 标记方法参数，描述通过本地化键从资源文件加载。
/// Marks a method parameter; description loaded from resource file via localization key.
/// </summary>
public class ToolParameterAttribute : DescriptionAttribute
{
    private readonly Type _resourceType;
    private readonly bool _isLocalize;
    
    public ToolParameterAttribute(string key,bool isLocalize = true,Type resourceType = null)
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