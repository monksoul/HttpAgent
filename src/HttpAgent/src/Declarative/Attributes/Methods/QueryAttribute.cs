// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 QUERY 请求方式特性
/// </summary>
/// <remarks>参考文献：https://www.rfc-editor.org/info/rfc10008/</remarks>
[AttributeUsage(AttributeTargets.Method)]
public sealed class QueryAttribute : HttpMethodAttribute
{
    /// <summary>
    ///     <inheritdoc cref="QueryAttribute" />
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    public QueryAttribute(string? requestUri = null)
        : base(Helpers.HttpQuery, requestUri)
    {
    }
}