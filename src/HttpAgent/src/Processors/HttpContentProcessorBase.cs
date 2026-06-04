// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="IHttpContentProcessor" /> 内容处理器基类
/// </summary>
public abstract class HttpContentProcessorBase : IHttpContentProcessor, IServiceProvider
{
    /// <inheritdoc />
    public IServiceProvider? ServiceProvider { get; set; }

    /// <inheritdoc />
    public abstract bool CanProcess(HttpContentProcessorContext context);

    /// <inheritdoc />
    public abstract HttpContent? Process(HttpContentProcessorContext context);

    /// <inheritdoc />
    public object? GetService(Type serviceType) => ServiceProvider?.GetService(serviceType);

    /// <summary>
    ///     尝试解析 <see cref="HttpContent" /> 类型
    /// </summary>
    /// <param name="context">
    ///     <see cref="HttpContentProcessorContext" />
    /// </param>
    /// <param name="httpContent">
    ///     <see cref="HttpContent" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpContent" />
    /// </returns>
    public virtual bool TryProcess(HttpContentProcessorContext context, out HttpContent? httpContent)
    {
        switch (context.RawContent)
        {
            case null:
                httpContent = null;
                return true;
            case HttpContent content:
                // 设置 Content-Type
                content.Headers.ContentType ??=
                    new MediaTypeHeaderValue(context.ContentType) { CharSet = context.Encoding?.WebName };

                httpContent = content;
                return true;
            default:
                httpContent = null;
                return false;
        }
    }

    /// <summary>
    ///     解析 JSON 序列化配置
    /// </summary>
    /// <param name="httpClientName"><see cref="HttpClient" /> 实例的配置名称</param>
    /// <returns>
    ///     <see cref="JsonSerializerOptions" />
    /// </returns>
    public virtual JsonSerializerOptions ResolveJsonSerializerOptions(string? httpClientName)
    {
        // 获取 HttpClientOptions 实例
        var httpClientOptions = this.GetService<IOptionsMonitor<HttpClientOptions>>()?.Get(httpClientName);

        // 获取 JsonSerializerOptions 配置
        // 优先级：指定名称的 HttpClientOptions -> HttpRemoteOptions -> 默认值
        var jsonSerializerOptions =
            (httpClientOptions?.IsDefault != false ? null : httpClientOptions.JsonSerializerOptions) ??
            this.GetService<IOptions<HttpRemoteOptions>>()?.Value.JsonSerializerOptions ??
            HttpRemoteOptions.JsonSerializerOptionsDefault;

        return jsonSerializerOptions;
    }
}