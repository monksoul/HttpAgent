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
    ///     设置 Access Token
    /// </summary>
    /// <remarks>用于首次获取或常规获取。</remarks>
    /// <param name="httpClientName"><see cref="HttpClient" /> 实例的配置名称</param>
    /// <param name="httpAccessToken">
    ///     <see cref="HttpAccessToken" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="Task" />
    /// </returns>
    Task SetTokenAsync(string? httpClientName, HttpAccessToken httpAccessToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     获取当前缓存的 Access Token
    /// </summary>
    /// <remarks>此方法不会触发刷新操作。</remarks>
    /// <param name="httpClientName"><see cref="HttpClient" /> 实例的配置名称</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpAccessToken" />
    /// </returns>
    Task<HttpAccessToken?> GetTokenAsync(string? httpClientName, CancellationToken cancellationToken = default);
}