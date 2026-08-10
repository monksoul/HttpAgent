// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON 请求分析工具提取器
/// </summary>
internal sealed class JsonProfilerExtractor : HttpJsonExtractorBase
{
    /// <inheritdoc />
    protected override string PropertyName => "profiler";

    /// <inheritdoc />
    protected override string[]? Aliases => ["debugger"];

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, JsonNode node,
        HttpJsonParsingContext context)
    {
        // 空检查
        if (node is JsonValue jsonValue && jsonValue.TryGetValue<bool>(out var enabled))
        {
            // 设置是否启用请求分析工具
            httpRequestBuilder.Profiler(enabled);
        }
    }
}