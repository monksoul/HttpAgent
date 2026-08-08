// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     cURL 提取器接口
/// </summary>
public interface IHttpCurlExtractor
{
    /// <summary>
    ///     从当前 Token 位置提取信息并构建 <see cref="HttpRequestBuilder" /> 实例
    /// </summary>
    /// <remarks>
    ///     <para>如果当前 Token 属于此提取器的管辖范围并成功消费，则返回 <c>true</c>；否则返回 <c>false</c>。</para>
    ///     <para>注意：当返回 <c>true</c> 时，实现类必须负责调用 <see cref="HttpCurlTokenExtractorContext.Advance" /> 推进游标，否则将导致解析死循环。</para>
    /// </remarks>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="context">
    ///     <see cref="HttpCurlTokenExtractorContext" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    bool TryExtract(HttpRequestBuilder httpRequestBuilder, HttpCurlTokenExtractorContext context);
}

/// <summary>
///     支持自定义优先级的 cURL 提取器接口
/// </summary>
public interface IOrderedHttpCurlExtractor : IHttpCurlExtractor
{
    /// <summary>
    ///     提取器的优先级
    /// </summary>
    /// <remarks>数值越小优先级越高。</remarks>
    int Order { get; }
}