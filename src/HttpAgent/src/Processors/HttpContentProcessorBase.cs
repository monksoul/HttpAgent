// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="IHttpContentProcessor" /> 内容处理器基类
/// </summary>
public abstract class HttpContentProcessorBase : IHttpContentProcessor
{
    /// <inheritdoc />
    public IServiceProvider? ServiceProvider { get; set; }

    /// <inheritdoc />
    public abstract bool CanProcess(object? rawContent, string contentType);

    /// <inheritdoc />
    public abstract HttpContent? Process(object? rawContent, string contentType, Encoding? encoding);

    /// <summary>
    ///     尝试解析 <see cref="HttpContent" /> 类型
    /// </summary>
    /// <param name="rawContent">原始请求内容</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="encoding">内容编码</param>
    /// <param name="httpContent">
    ///     <see cref="HttpContent" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpContent" />
    /// </returns>
    public virtual bool TryProcess([NotNullWhen(false)] object? rawContent, string contentType, Encoding? encoding,
        out HttpContent? httpContent)
    {
        switch (rawContent)
        {
            case null:
                httpContent = null;
                return true;
            case HttpContent content:
                // 设置 Content-Type
                content.Headers.ContentType ??=
                    new MediaTypeHeaderValue(contentType) { CharSet = encoding?.BodyName };

                httpContent = content;
                return true;
            default:
                httpContent = null;
                return false;
        }
    }

    /// <summary>
    ///     解析服务
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TService" />
    /// </returns>
    public TService? GetService<TService>() where TService : class => ServiceProvider?.GetService<TService>();
}