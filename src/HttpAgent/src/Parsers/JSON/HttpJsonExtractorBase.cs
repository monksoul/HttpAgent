// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON 属性提取器抽象基类
/// </summary>
public abstract class HttpJsonExtractorBase : IHttpJsonExtractor
{
    /// <summary>
    ///     当前提取器负责的 JSON 属性名（主键）
    /// </summary>
    /// <remarks>如 <c>"method"</c>、<c>"url"</c>。</remarks>
    protected abstract string PropertyName { get; }

    /// <summary>
    ///     当前提取器负责的 JSON 属性别名集合
    /// </summary>
    /// <remarks>如 <c>["queries", "query"]</c>。默认为 <c>null</c>。</remarks>
    protected virtual string[]? Aliases => null;

    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpJsonParsingContext context)
    {
        // 尝试匹配主属性名
        if (context.TryGetNode(PropertyName, out var node))
        {
            // 调用派生类的提取信息并构建 HttpRequestBuilder 实例
            Extract(httpRequestBuilder, node!, context);

            return;
        }

        // 空检查
        if (Aliases is null)
        {
            return;
        }

        // 尝试匹配别名
        if (!Aliases.Any(alias => context.TryGetNode(alias, out node)))
        {
            return;
        }

        // 调用派生类的提取信息并构建 HttpRequestBuilder 实例
        Extract(httpRequestBuilder, node!, context);
    }

    /// <summary>
    ///     提取信息并构建 <see cref="HttpRequestBuilder" /> 实例
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="node">当前属性对应的 <see cref="JsonNode" /> 节点</param>
    /// <param name="context">
    ///     <see cref="HttpJsonParsingContext" />
    /// </param>
    protected abstract void Extract(HttpRequestBuilder httpRequestBuilder, JsonNode node,
        HttpJsonParsingContext context);
}