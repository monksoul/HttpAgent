// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON 属性提取器接口
/// </summary>
public interface IHttpJsonExtractor
{
    /// <summary>
    ///     从 JSON 解析上下文中提取信息并构建 <see cref="HttpRequestBuilder" /> 实例
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="context">
    ///     <see cref="HttpJsonParsingContext" />
    /// </param>
    void Extract(HttpRequestBuilder httpRequestBuilder, HttpJsonParsingContext context);
}