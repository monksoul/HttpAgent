// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     Access Token 请求上下文
/// </summary>
public sealed class HttpAccessTokenContext
{
    /// <summary>
    ///     <inheritdoc cref="HttpAccessTokenContext" />
    /// </summary>
    /// <param name="httpClientName"><see cref="HttpClient" /> 实例的配置名称</param>
    /// <param name="httpAccessTokenProvider"><see cref="IHttpAccessTokenProvider" /> 实例</param>
    /// <exception cref="ArgumentNullException"></exception>
    internal HttpAccessTokenContext(string? httpClientName, IHttpAccessTokenProvider httpAccessTokenProvider)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpAccessTokenProvider);

        HttpClientName = httpClientName ?? string.Empty;
        HttpAccessTokenProvider = httpAccessTokenProvider;
    }

    /// <summary>
    ///     <see cref="HttpClient" /> 实例的配置名称
    /// </summary>
    public string HttpClientName { get; }

    /// <summary>
    ///     共享数据字典
    /// </summary>
    /// <remarks>
    ///     <para>用于存储与 Access Token 相关的自定义数据。</para>
    ///     <para>可通过 <c>HttpRequestBuilder.WithAccessTokenData</c> 方法进行设置。</para>
    /// </remarks>
    public IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();

    /// <summary>
    ///     <see cref="IHttpAccessTokenProvider" /> 实例
    /// </summary>
    internal IHttpAccessTokenProvider HttpAccessTokenProvider { get; }
}