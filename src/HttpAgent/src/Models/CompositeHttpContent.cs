// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     复合 <see cref="HttpContent" />
/// </summary>
/// <remarks>内部包含多个 <see cref="HttpContent" />，该类型本身不产生网络数据，仅作为容器使用，框架在发送前会自动展开其内容。</remarks>
public sealed class CompositeHttpContent : HttpContent
{
    /// <summary>
    ///     <see cref="HttpContent" /> 集合
    /// </summary>
    internal readonly List<HttpContent> _contents = [];

    /// <summary>
    ///     <inheritdoc cref="CompositeHttpContent" />
    /// </summary>
    public CompositeHttpContent()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="CompositeHttpContent" />
    /// </summary>
    /// <param name="contents"><see cref="HttpContent" /> 集合</param>
    /// <exception cref="ArgumentNullException"></exception>
    public CompositeHttpContent(params IEnumerable<HttpContent> contents)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(contents);

        AddRange(contents);
    }

    /// <summary>
    ///     <see cref="HttpContent" /> 集合
    /// </summary>
    public IReadOnlyList<HttpContent> Contents => _contents.AsReadOnly();

    /// <summary>
    ///     添加单个 <see cref="HttpContent" />
    /// </summary>
    /// <param name="content">
    ///     <see cref="HttpContent" />
    /// </param>
    /// <returns>
    ///     <see cref="CompositeHttpContent" />
    /// </returns>
    public CompositeHttpContent Add(HttpContent content)
    {
        // 空检查
        if ((HttpContent?)content is null)
        {
            return this;
        }

        // 检查是否是 CompositeHttpContent 实例
        if (content is CompositeHttpContent compositeHttpContent)
        {
            _contents.AddRange(compositeHttpContent._contents);
        }
        else
        {
            _contents.Add(content);
        }

        return this;
    }

    /// <summary>
    ///     批量添加 <see cref="HttpContent" />
    /// </summary>
    /// <param name="contents"><see cref="HttpContent" /> 集合</param>
    /// <returns>
    ///     <see cref="CompositeHttpContent" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public CompositeHttpContent AddRange(params IEnumerable<HttpContent> contents)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(contents);

        // 遍历添加
        foreach (var content in contents)
        {
            Add(content);
        }

        return this;
    }

    /// <inheritdoc />
    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
        throw new NotSupportedException($"{nameof(CompositeHttpContent)} is not serializable.");

    /// <inheritdoc />
    protected override bool TryComputeLength(out long length) =>
        throw new NotSupportedException("Length computation is not supported.");
}