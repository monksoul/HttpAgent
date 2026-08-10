// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON 解析选项
/// </summary>
public sealed class HttpJsonParsingOptions
{
    /// <summary>
    ///     <inheritdoc cref="HttpJsonParsingOptions" />
    /// </summary>
    internal HttpJsonParsingOptions() => Extractors.AddRange(GetDefaultExtractors());

    /// <summary>
    ///     <see cref="IHttpJsonExtractor" /> 提取器集合
    /// </summary>
    public List<IHttpJsonExtractor> Extractors { get; } = [];

    /// <summary>
    ///     移除指定类型的 <see cref="IHttpJsonExtractor" /> 提取器
    /// </summary>
    /// <typeparam name="TExtractor">
    ///     <see cref="IHttpJsonExtractor" />
    /// </typeparam>
    /// <returns>
    ///     <see cref="HttpJsonParsingOptions" />
    /// </returns>
    public HttpJsonParsingOptions RemoveExtractor<TExtractor>() where TExtractor : IHttpJsonExtractor
    {
        Extractors.RemoveAll(u => u is TExtractor);

        return this;
    }

    /// <summary>
    ///     添加自定义 <see cref="IHttpJsonExtractor" /> 提取器
    /// </summary>
    /// <param name="extractor">
    ///     <see cref="IHttpJsonExtractor" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpJsonParsingOptions" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpJsonParsingOptions AddExtractor(IHttpJsonExtractor extractor)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(extractor);

        Extractors.Add(extractor);

        return this;
    }

    /// <summary>
    ///     批量添加自定义 <see cref="IHttpJsonExtractor" /> 提取器
    /// </summary>
    /// <param name="extractors">
    ///     <see cref="IHttpJsonExtractor" /> 集合
    /// </param>
    /// <returns>
    ///     <see cref="HttpJsonParsingOptions" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpJsonParsingOptions AddExtractors(params IEnumerable<IHttpJsonExtractor> extractors)
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
    internal static IEnumerable<IHttpJsonExtractor> GetDefaultExtractors()
    {
        yield return new JsonMethodExtractor();
        yield return new JsonUrlExtractor();
        yield return new JsonBaseAddressExtractor();
        yield return new JsonHeadersExtractor();
        yield return new JsonParamsExtractor();
        yield return new JsonCookiesExtractor();
        yield return new JsonTimeoutExtractor();
        yield return new JsonClientExtractor();
        yield return new JsonVersionExtractor();
        yield return new JsonAuthExtractor();
        yield return new JsonDataExtractor();
        yield return new JsonMultipartExtractor();
        yield return new JsonProfilerExtractor();
    }
}