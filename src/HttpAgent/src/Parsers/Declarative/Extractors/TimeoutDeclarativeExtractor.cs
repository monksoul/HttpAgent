// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="TimeoutAttribute" /> 特性提取器
/// </summary>
internal sealed class TimeoutDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeParsingContext context)
    {
        // 检查方法或接口是否贴有 [Timeout] 特性
        if (!context.IsMethodDefined<TimeoutAttribute>(out var timeoutAttribute, true))
        {
            return;
        }

        // 设置超时时间
        httpRequestBuilder.SetTimeout(timeoutAttribute.Timeout);
    }
}