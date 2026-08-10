// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式模拟浏览器环境特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class SimulateBrowserAttribute : Attribute
{
    /// <summary>
    ///     是否为移动端，默认值为：<c>false</c>（即桌面端）
    /// </summary>
    public bool IsMobile { get; set; }
}