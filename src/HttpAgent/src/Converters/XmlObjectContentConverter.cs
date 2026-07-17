// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     XML 对象内容转换器
/// </summary>
public class XmlObjectContentConverter : IHttpContentConverter
{
    /// <inheritdoc />
    public virtual bool KeepsResponseAlive => false;

    /// <inheritdoc />
    public IServiceProvider? ServiceProvider { get; set; }

    /// <inheritdoc />
    public virtual object? Read(Type resultType, HttpContentConverterContext context,
        CancellationToken cancellationToken = default) =>
        AsyncUtility.RunSync(() => ReadAsync(resultType, context, cancellationToken));

    /// <inheritdoc />
    public virtual async Task<object?> ReadAsync(Type resultType, HttpContentConverterContext context,
        CancellationToken cancellationToken = default) =>
        DeserializeXml(resultType, await context.ResponseMessage.Content.ReadAsStringAsync(cancellationToken));

    /// <summary>
    ///     将 XML 字符串反序列化为转换的目标类型
    /// </summary>
    /// <param name="resultType">转换的目标类型</param>
    /// <param name="xmlString">XML 字符串</param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    protected virtual object? DeserializeXml(Type resultType, string xmlString)
    {
        // 初始化 XmlSerializer 实例
        var xmlSerializer = new XmlSerializer(resultType);

        // 初始化 StringReader 实例
        using var stringReader = new StringReader(xmlString);

        // XML 反序列化为对象
        return xmlSerializer.Deserialize(stringReader);
    }
}

/// <inheritdoc cref="XmlObjectContentConverter" />
/// <typeparam name="TResult">转换的目标类型</typeparam>
public class XmlObjectContentConverter<TResult> : XmlObjectContentConverter, IHttpContentConverter<TResult>
{
    /// <inheritdoc />
    public virtual TResult? Read(HttpContentConverterContext context, CancellationToken cancellationToken = default) =>
        (TResult?)base.Read(typeof(TResult), context, cancellationToken);

    /// <inheritdoc />
    public virtual async Task<TResult?> ReadAsync(HttpContentConverterContext context,
        CancellationToken cancellationToken = default) =>
        (TResult?)await base.ReadAsync(typeof(TResult), context, cancellationToken);
}