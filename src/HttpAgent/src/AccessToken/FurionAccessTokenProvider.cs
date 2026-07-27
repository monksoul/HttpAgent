// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     Furion 框架 Access Token 提供器
/// </summary>
/// <remarks>参考文献：https://furion.net/docs/auth-control。</remarks>
public class FurionAccessTokenProvider : IHttpAccessTokenProvider, IHttpAccessTokenConfigurator
{
    // Furion 框架相关常量定义
    internal const string XAuthorizationHeaderName = "X-Authorization";
    internal const string AccessTokenHeaderName = "access-token";
    internal const string XAccessTokenHeaderName = "x-access-token";
    internal const string BearerTokenFormat = "Bearer {0}";

    /// <inheritdoc />
    public virtual void Configure(HttpRequestBuilder httpRequestBuilder, HttpAccessToken httpAccessToken)
    {
        // 设置 JWT 身份验证凭据请求授权标头
        httpRequestBuilder.AddJwtBearerAuthentication(httpAccessToken.Value);

        // 检查 Access Token 是否过期且刷新 Token 不为空
        if (httpAccessToken.IsExpired() && httpAccessToken.RefreshToken is not null)
        {
            httpRequestBuilder.WithHeader(XAuthorizationHeaderName,
                string.Format(BearerTokenFormat, httpAccessToken.RefreshToken), replace: true);
        }

        // 设置在收到 HTTP 响应之后执行的操作
        httpRequestBuilder.SetOnPostReceiveResponse(httpResponseMessage =>
        {
            // 获取响应标头中的 access-token 和 x-access-token
            var newAccessToken = httpResponseMessage.Headers.GetValues(AccessTokenHeaderName).FirstOrDefault();
            var newRefreshToken = httpResponseMessage.Headers.GetValues(XAccessTokenHeaderName).FirstOrDefault();

            // 空检查
            // ReSharper disable once InvertIf
            if (!string.IsNullOrWhiteSpace(newAccessToken) && !string.IsNullOrWhiteSpace(newRefreshToken))
            {
                httpAccessToken.Value = newAccessToken;
                httpAccessToken.RefreshToken = newRefreshToken;

                httpAccessToken.ExpiresAt = JwtTokenUtility.Parse(newRefreshToken).GetExpirationTimeUtc()!.Value;
            }
        });
    }

    /// <inheritdoc />
    public virtual Task<HttpAccessToken?> GetAsync(HttpAccessTokenContext context,
        CancellationToken cancellationToken) => Task.FromResult<HttpAccessToken?>(null);

    /// <inheritdoc />
    public virtual Task<HttpAccessToken?> RefreshAsync(HttpAccessTokenContext context,
        HttpAccessToken? currentToken, CancellationToken cancellationToken) => Task.FromResult(currentToken);
}