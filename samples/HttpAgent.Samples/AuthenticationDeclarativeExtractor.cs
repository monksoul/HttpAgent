namespace HttpAgent.Samples;

/// <summary>
///     自定义授权特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public class AuthenticationAttribute : Attribute;

/// <summary>
///     [Authentication] 特性提取器
/// </summary>
public class AuthenticationDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeParsingContext context)
    {
        // 如果贴了 [AllowAnonymous] 特性则跳过
        if (context.IsMethodDefined<AllowAnonymousAttribute>(out _, true)) return;

        // 检查是否已经设置了授权信息
        if (httpRequestBuilder.AuthenticationHeader is not null) return;

        // 添加授权标头（这里可以实现任何授权的逻辑，比如从参数获取 token 等等）
        httpRequestBuilder.AddBearerAuthentication(
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c");
    }
}

/// <summary>
///     [AllowAnonymous] 特性提取器
/// </summary>
public class AllowAnonymousDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeParsingContext context)
    {
        // 如果没有贴 [AllowAnonymous] 特性则跳过
        if (!context.IsMethodDefined<AllowAnonymousAttribute>(out _, true)) return;

        // 移除授权标头
        httpRequestBuilder.RemoveHeaders("Authorization");
    }
}