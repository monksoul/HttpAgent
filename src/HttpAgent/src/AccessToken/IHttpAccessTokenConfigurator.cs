// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     Access Token 配置器接口
/// </summary>
/// <remarks>用于决定 Access Token 的注入位置（Header、Query、Cookie 等）。</remarks>
public interface IHttpAccessTokenConfigurator
{
    /// <summary>
    ///     将 Access Token 配置到请求中
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="httpAccessToken">
    ///     <see cref="HttpAccessToken" />
    /// </param>
    void Configure(HttpRequestBuilder httpRequestBuilder, HttpAccessToken httpAccessToken);
}