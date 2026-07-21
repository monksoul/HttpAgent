// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     Access Token 提供器
/// </summary>
/// <remarks>
///     <para>负责获取新的 Access Token 并告知过期时间。</para>
///     <para>实现该接口的类型也可以同时实现 <see cref="IHttpAccessTokenConfigurator" />。</para>
/// </remarks>
public interface IHttpAccessTokenProvider
{
    /// <summary>
    ///     获取 Access Token
    /// </summary>
    /// <remarks>若使用 <see cref="IHttpAccessTokenManager.SetTokenAsync" /> 设置 Access Token，那么该方法可以空实现或抛出异常。</remarks>
    /// <param name="context">
    ///     <see cref="HttpAccessTokenContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpAccessToken" />
    /// </returns>
    Task<HttpAccessToken?> GetTokenAsync(HttpAccessTokenContext context, CancellationToken cancellationToken);

    /// <summary>
    ///     刷新 Access Token
    /// </summary>
    /// <remarks>默认实现直接调用 <see cref="GetTokenAsync" />。若认证流程中“首次获取 Token”与“刷新 Token”使用不同的接口（如登录接口 vs 刷新专用接口），可重写此方法并提供专用的刷新逻辑。</remarks>
    /// <param name="context">
    ///     <see cref="HttpAccessTokenContext" />
    /// </param>
    /// <param name="currentToken">当前已缓存的 <see cref="HttpAccessToken" /></param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpAccessToken" />
    /// </returns>
    Task<HttpAccessToken?> RefreshTokenAsync(HttpAccessTokenContext context, HttpAccessToken? currentToken,
        CancellationToken cancellationToken) => GetTokenAsync(context, cancellationToken);

    /// <summary>
    ///     指示是否需要强制刷新 Access Token 并重试请求
    /// </summary>
    /// <remarks>
    ///     默认实现仅在状态码为 <see cref="HttpStatusCode.Unauthorized" />（401）时返回 <c>true</c>。可重写此方法以自定义刷新策略（例如检查 403、响应头、响应体等）。
    /// </remarks>
    /// <param name="context">
    ///     <see cref="HttpAccessTokenContext" />
    /// </param>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    Task<bool> ShouldRefreshTokenAsync(HttpAccessTokenContext context, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken) =>
        Task.FromResult(httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized);
}