// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     Access Token 管理器
/// </summary>
internal sealed class HttpAccessTokenManager
{
    /// <summary>
    ///     <see cref="HttpClient" /> 实例的配置名称的 Access Token 缓存字典
    /// </summary>
    internal readonly ConcurrentDictionary<string, AccessTokenCache> _httpClientNameCaches = new();

    /// <summary>
    ///     获取或刷新指定 <see cref="HttpClient" /> 实例的配置名称的 Access Token
    /// </summary>
    /// <param name="context">
    ///     <see cref="HttpAccessTokenContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpAccessToken" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<HttpAccessToken> GetOrRefreshAsync(HttpAccessTokenContext context,
        CancellationToken cancellationToken)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(context);

        // 获取或创建与 HttpClient 实例的配置名称对应的 Access Token 缓存项
        var accessTokenCache = _httpClientNameCaches.GetOrAdd(context.HttpClientName, _ => new AccessTokenCache());

        // 检查 Access Token 是否过期
        if (accessTokenCache.Current?.IsExpired() == false)
        {
            return accessTokenCache.Current;
        }

        return await accessTokenCache.GetOrRefreshAsync(context, cancellationToken);
    }

    /// <summary>
    ///     强制刷新指定 <see cref="HttpClient" /> 实例的配置名称的 Access Token
    /// </summary>
    /// <param name="context">
    ///     <see cref="HttpAccessTokenContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpAccessToken" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<HttpAccessToken> ForceRefreshAsync(HttpAccessTokenContext context,
        CancellationToken cancellationToken)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(context);

        // 获取或创建与 HttpClient 实例的配置名称对应的 Access Token 缓存项
        var accessTokenCache = _httpClientNameCaches.GetOrAdd(context.HttpClientName, _ => new AccessTokenCache());

        return await accessTokenCache.ForceRefreshAsync(context, cancellationToken);
    }

    /// <summary>
    ///     Access Token 缓存项
    /// </summary>
    internal sealed class AccessTokenCache
    {
        /// <summary>
        ///     <see cref="SemaphoreSlim" /> 刷新锁
        /// </summary>
        /// <remarks>确保同一时间只有一个刷新 Access Token 操作。</remarks>
        internal readonly SemaphoreSlim _refreshLock = new(1, 1);

        /// <summary>
        ///     当前有效的 <see cref="HttpAccessToken" /> 实例
        /// </summary>
        internal volatile HttpAccessToken? _current;

        /// <summary>
        ///     当前有效的 <see cref="HttpAccessToken" /> 实例
        /// </summary>
        internal HttpAccessToken? Current => _current;

        /// <summary>
        ///     获取或刷新 Access Token
        /// </summary>
        /// <param name="context">
        ///     <see cref="HttpAccessTokenContext" />
        /// </param>
        /// <param name="cancellationToken">
        ///     <see cref="CancellationToken" />
        /// </param>
        /// <returns>
        ///     <see cref="HttpAccessToken" />
        /// </returns>
        internal async Task<HttpAccessToken> GetOrRefreshAsync(HttpAccessTokenContext context,
            CancellationToken cancellationToken)
        {
            // 等待进入互斥区
            await _refreshLock.WaitAsync(cancellationToken);

            try
            {
                // 检查 Access Token 是否过期
                if (_current?.IsExpired() == false)
                {
                    return _current;
                }

                // 获取新的 Access Token
                var httpAccessToken = await context.HttpAccessTokenProvider.GetTokenAsync(cancellationToken);

                // 更新缓存
                _current = httpAccessToken;

                return httpAccessToken;
            }
            finally
            {
                // 释放锁
                _refreshLock.Release();
            }
        }

        /// <summary>
        ///     强制刷新 Access Token
        /// </summary>
        /// <param name="context">
        ///     <see cref="HttpAccessTokenContext" />
        /// </param>
        /// <param name="cancellationToken">
        ///     <see cref="CancellationToken" />
        /// </param>
        /// <returns>
        ///     <see cref="HttpAccessToken" />
        /// </returns>
        internal async Task<HttpAccessToken> ForceRefreshAsync(HttpAccessTokenContext context,
            CancellationToken cancellationToken)
        {
            // 等待进入互斥区
            await _refreshLock.WaitAsync(cancellationToken);

            try
            {
                // 获取新的 Access Token
                var httpAccessToken = await context.HttpAccessTokenProvider.GetTokenAsync(cancellationToken);

                // 更新缓存
                _current = httpAccessToken;

                return httpAccessToken;
            }
            finally
            {
                // 释放锁
                _refreshLock.Release();
            }
        }
    }
}