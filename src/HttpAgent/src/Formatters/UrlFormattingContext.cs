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
    /// <param name="httpClientName"><see cref="HttpClient" /> 实例的配置名称</param>
    /// <exception cref="ArgumentException"></exception>
    internal UrlFormattingContext(string key, string? httpClientName)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        Key = key;
        HttpClientName = httpClientName ?? string.Empty;
    }

    /// <summary>
    ///     URL 参数名
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     <see cref="HttpClient" /> 实例的配置名称
    /// </summary>
    public string HttpClientName { get; }

    /// <summary>
    ///     是否为原始 URL 参数
    /// </summary>
    public bool IsOriginal { get; internal init; }
}