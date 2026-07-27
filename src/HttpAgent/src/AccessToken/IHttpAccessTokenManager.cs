// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     Access Token 管理器接口
/// </summary>
public interface IHttpAccessTokenManager
{
    /// <summary>
    ///     获取指定 <see cref="HttpClient" /> 实例的配置名称的缓存 Access Token
    /// </summary>
    /// <remarks>此方法不会触发刷新操作。</remarks>
    /// <param name="httpClientName"><see cref="HttpClient" /> 实例的配置名称</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpAccessToken" />
    /// </returns>
    Task<HttpAccessToken?> GetAsync(string? httpClientName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     设置指定 <see cref="HttpClient" /> 实例的配置名称的 Access Token
    /// </summary>
    /// <remarks>用于首次获取或常规获取。</remarks>
    /// <param name="httpClientName"><see cref="HttpClient" /> 实例的配置名称</param>
    /// <param name="httpAccessToken">
    ///     <see cref="HttpAccessToken" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    Task SetAsync(string? httpClientName, HttpAccessToken httpAccessToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     获取或刷新指定 <see cref="HttpClient" /> 实例的配置名称的 Access Token
    /// </summary>
    /// <remarks>
    ///     内部实现中，若缓存中已存在有效的 <see cref="HttpAccessToken" />，则直接返回；否则调用
    ///     <see cref="IHttpAccessTokenProvider.RefreshAsync" /> 方法获取新的 <see cref="HttpAccessToken" /> 实例，并将其缓存后再返回。
    /// </remarks>
    /// <param name="context">
    ///     <see cref="HttpAccessTokenContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpAccessToken" />
    /// </returns>
    Task<HttpAccessToken?> GetOrRefreshAsync(HttpAccessTokenContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     强制刷新指定 <see cref="HttpClient" /> 实例的配置名称的 Access Token
    /// </summary>
    /// <remarks>
    ///     内部实现中，直接调用 <see cref="IHttpAccessTokenProvider.RefreshAsync" /> 方法获取新的 <see cref="HttpAccessToken" />
    ///     实例，并将其缓存后再返回。
    /// </remarks>
    /// <param name="context">
    ///     <see cref="HttpAccessTokenContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpAccessToken" />
    /// </returns>
    Task<HttpAccessToken?> ForceRefreshAsync(HttpAccessTokenContext context,
        CancellationToken cancellationToken = default);
}