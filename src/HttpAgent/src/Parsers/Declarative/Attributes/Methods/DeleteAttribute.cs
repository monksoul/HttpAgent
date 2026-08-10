// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 DELETE 请求方式特性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class DeleteAttribute : HttpMethodAttribute
{
    /// <summary>
    ///     <inheritdoc cref="DeleteAttribute" />
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    public DeleteAttribute(string? requestUri = null)
        : base(HttpMethod.Delete, requestUri)
    {
    }
}