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
    public IServiceProvider? ServiceProvider { get; set; }

    /// <inheritdoc />
    public virtual object? Read(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        DeserializeXml(resultType,
            httpResponseMessage.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult());

    /// <inheritdoc />
    public virtual async Task<object?> ReadAsync(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        DeserializeXml(resultType, await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken));

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
    public virtual TResult? Read(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        (TResult?)DeserializeXml(typeof(TResult),
            httpResponseMessage.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult());

    /// <inheritdoc />
    public virtual async Task<TResult?> ReadAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        (TResult?)DeserializeXml(typeof(TResult),
            await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken));
}