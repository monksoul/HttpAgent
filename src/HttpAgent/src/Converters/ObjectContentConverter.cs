// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="ObjectContentConverter{TResult}" /> 默认基类
/// </summary>
public class ObjectContentConverter : IHttpContentConverter
{
    /// <inheritdoc />
    public IServiceProvider? ServiceProvider { get; set; }

    /// <inheritdoc />
    public virtual object? Read(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default)
    {
        // 检查 HTTP 响应的内容类型是否为 XML 媒体类型
        if (!HasXmlContentType(httpResponseMessage))
        {
            return httpResponseMessage.Content
                .ReadFromJsonAsync(resultType, GetJsonSerializerOptions(httpResponseMessage), cancellationToken)
                .GetAwaiter().GetResult();
        }

        // 将 XML 字符串反序列化为转换的目标类型
        return DeserializeXml(resultType,
            httpResponseMessage.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult());
    }

    /// <inheritdoc />
    public virtual async Task<object?> ReadAsync(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default)
    {
        // 检查 HTTP 响应的内容类型是否为 XML 媒体类型
        if (!HasXmlContentType(httpResponseMessage))
        {
            return await httpResponseMessage.Content.ReadFromJsonAsync(resultType,
                GetJsonSerializerOptions(httpResponseMessage), cancellationToken);
        }

        // 将 XML 字符串反序列化为转换的目标类型
        return DeserializeXml(resultType, await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken));
    }

    /// <summary>
    ///     获取 JSON 序列化选项实例
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <returns>
    ///     <see cref="JsonSerializerOptions" />
    /// </returns>
    protected virtual JsonSerializerOptions GetJsonSerializerOptions(HttpResponseMessage httpResponseMessage)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpResponseMessage);

        // 获取 HttpClient 实例的配置名称
        if (httpResponseMessage.RequestMessage?.Options.TryGetValue(
                new HttpRequestOptionsKey<string>(Constants.HTTP_CLIENT_NAME), out var httpClientName) != true)
        {
            httpClientName = string.Empty;
        }

        // 获取 HttpClientOptions 实例
        var httpClientOptions = ServiceProvider?.GetService<IOptionsMonitor<HttpClientOptions>>()?.Get(httpClientName);

        // 优先级：指定名称的 HttpClientOptions -> HttpRemoteOptions -> 默认值
        return (httpClientOptions?.IsDefault != false ? null : httpClientOptions.JsonSerializerOptions) ??
               ServiceProvider?.GetRequiredService<IOptions<HttpRemoteOptions>>().Value.JsonSerializerOptions ??
               HttpRemoteOptions.JsonSerializerOptionsDefault;
    }

    /// <summary>
    ///     检查 HTTP 响应的内容类型是否为 XML 媒体类型
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    protected virtual bool HasXmlContentType(HttpResponseMessage httpResponseMessage) =>
        httpResponseMessage.Content.Headers.ContentType?.MediaType.IsIn(
            [MediaTypeNames.Application.Xml, MediaTypeNames.Application.XmlPatch, MediaTypeNames.Text.Xml],
            StringComparer.OrdinalIgnoreCase) == true;

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

    /// <summary>
    ///     将 XML 字符串反序列化为 <typeparamref name="TResult" /> 类型
    /// </summary>
    /// <param name="xmlString">XML 字符串</param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    protected virtual TResult? DeserializeXml<TResult>(string xmlString) =>
        (TResult?)DeserializeXml(typeof(TResult), xmlString);
}

/// <summary>
///     对象转换器
/// </summary>
/// <typeparam name="TResult">转换的目标类型</typeparam>
public class ObjectContentConverter<TResult> : ObjectContentConverter, IHttpContentConverter<TResult>
{
    /// <inheritdoc />
    public virtual TResult? Read(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default)
    {
        // 检查 HTTP 响应的内容类型是否为 XML 媒体类型
        if (!HasXmlContentType(httpResponseMessage))
        {
            return httpResponseMessage.Content
                .ReadFromJsonAsync<TResult>(GetJsonSerializerOptions(httpResponseMessage), cancellationToken)
                .GetAwaiter().GetResult();
        }

        // 将 XML 字符串反序列化为转换的目标类型
        return DeserializeXml<TResult>(httpResponseMessage.Content.ReadAsStringAsync(cancellationToken).GetAwaiter()
            .GetResult());
    }

    /// <inheritdoc />
    public virtual async Task<TResult?> ReadAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default)
    {
        // 检查 HTTP 响应的内容类型是否为 XML 媒体类型
        if (!HasXmlContentType(httpResponseMessage))
        {
            return await httpResponseMessage.Content.ReadFromJsonAsync<TResult>(
                GetJsonSerializerOptions(httpResponseMessage), cancellationToken);
        }

        // 将 XML 字符串反序列化为转换的目标类型
        return DeserializeXml<TResult>(await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken));
    }
}