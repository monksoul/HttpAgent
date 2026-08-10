// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     cURL 解析选项
/// </summary>
public sealed class HttpCurlParsingOptions
{
    /// <summary>
    ///     <inheritdoc cref="HttpCurlParsingOptions" />
    /// </summary>
    internal HttpCurlParsingOptions() => Extractors.AddRange(GetDefaultExtractors());

    /// <summary>
    ///     <see cref="IHttpCurlExtractor" /> 提取器集合
    /// </summary>
    public List<IHttpCurlExtractor> Extractors { get; } = [];

    /// <summary>
    ///     移除指定类型的 <see cref="IHttpCurlExtractor" /> 提取器
    /// </summary>
    /// <typeparam name="TExtractor">
    ///     <see cref="IHttpCurlExtractor" />
    /// </typeparam>
    /// <returns>
    ///     <see cref="HttpCurlParsingOptions" />
    /// </returns>
    public HttpCurlParsingOptions RemoveExtractor<TExtractor>() where TExtractor : IHttpCurlExtractor
    {
        Extractors.RemoveAll(u => u is TExtractor);

        return this;
    }

    /// <summary>
    ///     添加自定义 <see cref="IHttpCurlExtractor" /> 提取器
    /// </summary>
    /// <param name="extractor">
    ///     <see cref="IHttpCurlExtractor" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpCurlParsingOptions" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpCurlParsingOptions AddExtractor(IHttpCurlExtractor extractor)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(extractor);

        Extractors.Add(extractor);

        return this;
    }

    /// <summary>
    ///     批量添加自定义 <see cref="IHttpCurlExtractor" /> 提取器
    /// </summary>
    /// <param name="extractors">
    ///     <see cref="IHttpCurlExtractor" /> 集合
    /// </param>
    /// <returns>
    ///     <see cref="HttpCurlParsingOptions" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpCurlParsingOptions AddExtractors(params IEnumerable<IHttpCurlExtractor> extractors)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(extractors);

        Extractors.AddRange(extractors);

        return this;
    }

    /// <summary>
    ///     获取默认的内置提取器集合
    /// </summary>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    internal static IEnumerable<IHttpCurlExtractor> GetDefaultExtractors()
    {
        yield return new CurlMethodExtractor();
        yield return new CurlHeadExtractor();
        yield return new CurlHeaderExtractor();
        yield return new CurlCookieExtractor();
        yield return new CurlDataExtractor();
        yield return new CurlAuthExtractor();
        yield return new CurlUserAgentExtractor();
        yield return new CurlRefererExtractor();
        yield return new CurlFormExtractor();
        yield return new CurlTimeoutExtractor();
        yield return new CurlVersionExtractor();
        yield return new CurlUrlExtractor();
    }
}