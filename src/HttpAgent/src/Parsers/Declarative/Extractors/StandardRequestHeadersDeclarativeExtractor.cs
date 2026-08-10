// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="StandardRequestHeadersAttribute" /> 特性提取器
/// </summary>
internal sealed class StandardRequestHeadersDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeParsingContext context)
    {
        // 检查方法或接口是否贴有 [StandardRequestHeaders] 特性
        if (!context.IsMethodDefined<StandardRequestHeadersAttribute>(out var standardRequestHeadersAttribute, true))
        {
            return;
        }

        // 设置是否启用标准请求标头
        httpRequestBuilder.UseStandardRequestHeaders(standardRequestHeadersAttribute.Enabled);
    }
}