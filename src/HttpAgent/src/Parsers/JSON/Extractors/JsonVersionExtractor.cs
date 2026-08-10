// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON HTTP 版本提取器
/// </summary>
internal sealed class JsonVersionExtractor : HttpJsonExtractorBase
{
    /// <inheritdoc />
    protected override string PropertyName => "httpVersion";

    /// <inheritdoc />
    protected override string[]? Aliases => ["version"];

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, JsonNode node,
        HttpJsonParsingContext context)
    {
        // 空检查
        if (node is JsonValue jsonValue && jsonValue.TryGetValue<string>(out var version) &&
            !string.IsNullOrWhiteSpace(version))
        {
            // 设置 HTTP 版本
            httpRequestBuilder.SetVersion(version);
        }
    }
}