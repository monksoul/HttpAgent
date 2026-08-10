// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 TRACE 请求方式特性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class TraceAttribute : HttpMethodAttribute
{
    /// <summary>
    ///     <inheritdoc cref="TraceAttribute" />
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    public TraceAttribute(string? requestUri = null)
        : base(HttpMethod.Trace, requestUri)
    {
    }
}