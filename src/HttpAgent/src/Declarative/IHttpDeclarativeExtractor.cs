// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式提取器接口
/// </summary>
public interface IHttpDeclarativeExtractor
{
    /// <summary>
    ///     提取方法信息并构建 <see cref="HttpRequestBuilder" /> 实例
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="context">
    ///     <see cref="HttpDeclarativeExtractorContext" />
    /// </param>
    void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context);
}

/// <summary>
///     支持自定义优先级的 HTTP 声明式提取器接口
/// </summary>
public interface IFrozenHttpDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <summary>
    ///     提取器的优先级
    /// </summary>
    /// <remarks>数值越小优先级越高。</remarks>
    int Order { get; }
}