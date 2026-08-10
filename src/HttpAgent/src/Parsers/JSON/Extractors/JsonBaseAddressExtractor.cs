// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON 基地址提取器
/// </summary>
internal sealed class JsonBaseAddressExtractor : HttpJsonExtractorBase
{
    /// <inheritdoc />
    protected override string PropertyName => "baseURL";

    /// <inheritdoc />
    protected override string[]? Aliases => ["baseAddress", "baseUrl"];

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, JsonNode node,
        HttpJsonParsingContext context)
    {
        // 空检查
        if (node is JsonValue jsonValue && jsonValue.TryGetValue<string>(out var baseAddress) &&
            !string.IsNullOrWhiteSpace(baseAddress))
        {
            // 设置请求基地址
            httpRequestBuilder.SetBaseAddress(baseAddress);
        }
    }
}