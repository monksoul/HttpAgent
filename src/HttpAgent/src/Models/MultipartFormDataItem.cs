// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="MultipartFormDataContent" /> 条目
/// </summary>
public sealed class MultipartFormDataItem
{
    /// <summary>
    ///     <inheritdoc cref="MultipartFormDataItem" />
    /// </summary>
    /// <param name="name">表单名称</param>
    internal MultipartFormDataItem(string name)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    /// <summary>
    ///     表单名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     内容类型
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    ///     内容编码
    /// </summary>
    public Encoding? ContentEncoding { get; init; }

    /// <summary>
    ///     原始请求内容
    /// </summary>
    /// <remarks>此属性值最终将转换为 <see cref="HttpContent" /> 类型实例。</remarks>
    public object? RawContent { get; init; }

    /// <summary>
    ///     文件的名称
    /// </summary>
    public string? FileName { get; init; }
}