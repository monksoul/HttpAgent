// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式启用 JSON 响应内容字符串的解包处理（双重序列化）
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class JsonResponseStringUnwrapAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="JsonResponseStringUnwrapAttribute" />
    /// </summary>
    public JsonResponseStringUnwrapAttribute()
        : this(true)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="JsonResponseStringUnwrapAttribute" />
    /// </summary>
    /// <param name="enabled">是否启用</param>
    public JsonResponseStringUnwrapAttribute(bool enabled) => Enabled = enabled;

    /// <summary>
    ///     是否启用
    /// </summary>
    public bool Enabled { get; set; }
}