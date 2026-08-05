// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     Furion 框架 Access Token 提供器
/// </summary>
/// <remarks>参考文献：https://furion.net/docs/auth-control。</remarks>
/// <param name="accessTokenManager">
///     <see cref="IHttpAccessTokenManager" />
/// </param>
public class FurionAccessTokenProvider(IHttpAccessTokenManager accessTokenManager)
    : IHttpAccessTokenProvider, IHttpAccessTokenConfigurator
{
    // Furion 框架相关常量定义
    internal const string XAuthorizationHeaderName = "X-Authorization";
    internal const string AccessTokenHeaderName = "access-token";
    internal const string XAccessTokenHeaderName = "x-access-token";

    /// <inheritdoc />
    public virtual void Configure(HttpRequestBuilder httpRequestBuilder, HttpAccessToken httpAccessToken)
    {
        // 设置 JWT 身份验证凭据请求授权标头
        httpRequestBuilder.AddBearerAuthentication(httpAccessToken.Value);

        // 检查 Access Token 是否过期且刷新 Token 不为空
        if (httpAccessToken.IsExpired() && httpAccessToken.RefreshToken is not null)
        {
            httpRequestBuilder.AddBearerAuthentication(XAuthorizationHeaderName, httpAccessToken.RefreshToken);
        }

        // 设置在收到 HTTP 响应之后执行的操作
        httpRequestBuilder.SetOnPostReceiveResponse(async (httpResponseMessage, cancellationToken) =>
        {
            // 获取响应标头中的 access-token 和 x-access-token
            if (!httpResponseMessage.Headers.TryGetValues(AccessTokenHeaderName, out var atValues) ||
                !httpResponseMessage.Headers.TryGetValues(XAccessTokenHeaderName, out var rtValues))
            {
                return;
            }

            var newAccessToken = atValues.FirstOrDefault();
            var newRefreshToken = rtValues.FirstOrDefault();

            // 如果任意一个值为空则跳过
            if (string.IsNullOrWhiteSpace(newAccessToken) || string.IsNullOrWhiteSpace(newRefreshToken))
            {
                return;
            }

            // 从新的 Access Token 的 JWT 中解析过期时间
            var expiresAt = JwtTokenUtility.Parse(newAccessToken).GetExpirationTimeUtc()!;

            // 创建新的 HttpAccessToken 实例
            var updatedToken = new HttpAccessToken(newAccessToken, expiresAt.Value) { RefreshToken = newRefreshToken };

            // 更新 Access Token 缓存
            await accessTokenManager.SetAsync(httpRequestBuilder.HttpClientName, updatedToken, cancellationToken);
        });
    }

    /// <inheritdoc />
    public virtual Task<HttpAccessToken?>
        GetAsync(HttpAccessTokenContext context, CancellationToken cancellationToken) =>
        Task.FromResult<HttpAccessToken?>(null);

    /// <inheritdoc />
    public virtual Task<bool> ShouldRefreshAsync(HttpAccessTokenContext context,
        HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken) =>
        Task.FromResult(false);
}