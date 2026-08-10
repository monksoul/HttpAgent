// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式请求方式特性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class HttpMethodAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="HttpMethodAttribute" />
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    public HttpMethodAttribute(string httpMethod, string? requestUri = null)
        : this(Helpers.ParseHttpMethod(httpMethod), requestUri)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="HttpMethodAttribute" />
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    public HttpMethodAttribute(HttpMethod httpMethod, string? requestUri = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpMethod);

        HttpMethod = httpMethod;
        RequestUri = requestUri;
    }

    /// <summary>
    ///     请求方式
    /// </summary>
    public HttpMethod HttpMethod { get; }

    /// <summary>
    ///     请求地址
    /// </summary>
    public string? RequestUri { get; }
}