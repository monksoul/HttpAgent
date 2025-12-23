// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="RequestEventHandlerAttribute" /> 特性提取器
/// </summary>
internal sealed class RequestEventHandlerDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 检查方法或接口是否贴有 [RequestEventHandler] 特性
        if (!context.IsMethodDefined<RequestEventHandlerAttribute>(out var requestEventHandlerAttribute, true))
        {
            return;
        }

        // 设置 HTTP 远程请求事件处理程序
        httpRequestBuilder.SetEventHandler(requestEventHandlerAttribute.HandlerType);
    }
}