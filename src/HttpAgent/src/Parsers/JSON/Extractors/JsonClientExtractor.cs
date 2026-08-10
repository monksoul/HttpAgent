// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON 客户端名称提取器
/// </summary>
internal sealed class JsonClientExtractor : HttpJsonExtractorBase
{
    /// <inheritdoc />
    protected override string PropertyName => "client";

    /// <inheritdoc />
    protected override string[]? Aliases => ["clientName", "httpClientName"];

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, JsonNode node,
        HttpJsonParsingContext context)
    {
        // 空检查
        if (node is JsonValue jsonValue && jsonValue.TryGetValue<string>(out var clientName))
        {
            // 设置 HttpClient 实例的配置名称
            httpRequestBuilder.SetHttpClientName(clientName);
        }
    }
}