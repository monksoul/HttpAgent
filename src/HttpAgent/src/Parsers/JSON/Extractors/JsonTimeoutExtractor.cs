// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON 超时时间提取器
/// </summary>
internal sealed class JsonTimeoutExtractor : HttpJsonExtractorBase
{
    /// <inheritdoc />
    protected override string PropertyName => "timeout";

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, JsonNode node,
        HttpJsonParsingContext context)
    {
        // 空检查
        if (node is JsonValue jsonValue && jsonValue.TryGetValue<double>(out var milliseconds))
        {
            // 设置超时时间
            httpRequestBuilder.SetTimeout(milliseconds);
        }
    }
}