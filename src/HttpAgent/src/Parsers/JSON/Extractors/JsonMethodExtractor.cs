// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON 请求方法提取器
/// </summary>
internal sealed class JsonMethodExtractor : HttpJsonExtractorBase
{
    /// <inheritdoc />
    protected override string PropertyName => "method";

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, JsonNode node,
        HttpJsonParsingContext context)
    {
        // 空检查
        if (node is JsonValue jsonValue && jsonValue.TryGetValue<string>(out var method) &&
            !string.IsNullOrWhiteSpace(method))
        {
            // 设置请求方式
            httpRequestBuilder.SetHttpMethod(Helpers.ParseHttpMethod(method));
        }
    }
}