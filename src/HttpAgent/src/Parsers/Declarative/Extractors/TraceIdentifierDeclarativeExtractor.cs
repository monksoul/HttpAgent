// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="TraceIdentifierAttribute" /> 特性提取器
/// </summary>
internal sealed class TraceIdentifierDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeParsingContext context)
    {
        // 检查方法或接口是否贴有 [TraceIdentifier] 特性
        if (!context.IsMethodDefined<TraceIdentifierAttribute>(out var traceIdentifierAttribute, true))
        {
            return;
        }

        // 设置跟踪标识
        httpRequestBuilder.SetTraceIdentifier(traceIdentifierAttribute.Identifier);
    }
}