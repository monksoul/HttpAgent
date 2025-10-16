// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     URL 参数格式化上下文
/// </summary>
public sealed class UrlFormattingContext
{
    /// <summary>
    ///     <inheritdoc cref="UrlFormattingContext" />
    /// </summary>
    /// <param name="key">URL 参数名（键）</param>
    public UrlFormattingContext(string key)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        Key = key;
    }

    /// <summary>
    ///     URL 参数名（键）
    /// </summary>
    public string Key { get; }
}