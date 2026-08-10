// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON Cookies 提取器
/// </summary>
internal sealed class JsonCookiesExtractor : HttpJsonExtractorBase
{
    /// <inheritdoc />
    protected override string PropertyName => "cookies";

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, JsonNode node,
        HttpJsonParsingContext context) =>
        // 设置 Cookies
        httpRequestBuilder.WithCookies(node);
}