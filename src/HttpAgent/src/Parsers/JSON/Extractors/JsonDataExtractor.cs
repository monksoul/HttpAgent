// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON 请求内容提取器
/// </summary>
/// <remarks>
///     <para>当 <c>data</c> 属性存在时，<c>contentType</c> 为可选。</para>
///     <para>若未指定 <c>contentType</c>，将直接传入 <see cref="JsonNode" />，由底层的 <c>GetContentTypeOrDefault</c> 方法推断内容类型。</para>
/// </remarks>
internal sealed class JsonDataExtractor : HttpJsonExtractorBase
{
    /// <inheritdoc />
    protected override string PropertyName => "data";

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, JsonNode node,
        HttpJsonParsingContext context)
    {
        // 获取内容类型
        string? contentType = null;

        // 检查是否配置了内容类型
        if (context.TryGetNode("contentType", out var contentTypeNode) &&
            contentTypeNode is JsonValue contentTypeValue && contentTypeValue.TryGetValue<string>(out var ct) &&
            !string.IsNullOrWhiteSpace(ct))
        {
            contentType = ct;
        }

        // 设置请求内容
        httpRequestBuilder.SetContent(node, contentType).AddStringContentForFormUrlEncodedContentProcessor();

        // 检查是否配置了内容编码
        if (context.TryGetNode("encoding", out var encodingNode) && encodingNode is JsonValue encodingValue &&
            encodingValue.TryGetValue<string>(out var encoding) && !string.IsNullOrWhiteSpace(encoding))
        {
            // 设置内容编码
            httpRequestBuilder.SetContentEncoding(encoding);
        }
    }
}