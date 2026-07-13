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
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpAccessToken" />
    /// </returns>
    Task<HttpAccessToken> GetAccessTokenAsync(CancellationToken cancellationToken);
}