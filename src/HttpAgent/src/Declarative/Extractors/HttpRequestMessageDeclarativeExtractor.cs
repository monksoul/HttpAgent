// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="HttpRequestMessage" /> 自定义配置提取器
/// </summary>
internal sealed class HttpRequestMessageDeclarativeExtractor : IFrozenHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 尝试解析单个 Action<HttpRequestMessage> 类型参数
        if (context.Args.SingleOrDefault(u => u is Action<HttpRequestMessage>) is Action<HttpRequestMessage> configure)
        {
            httpRequestBuilder.SetOnPreSendRequest(configure);
        }
    }

    /// <inheritdoc />
    public int Order => 4;
}