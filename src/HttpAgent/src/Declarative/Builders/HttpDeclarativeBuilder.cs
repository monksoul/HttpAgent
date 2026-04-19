// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace HttpAgent;

/// <summary>
///     HTTP 声明式远程请求构建器
/// </summary>
/// <remarks>使用 <c>HttpRequestBuilder.Declarative(method, args)</c> 静态方法创建。</remarks>
public sealed class HttpDeclarativeBuilder
{
    /// <summary>
    ///     HTTP 声明式 <see cref="IHttpDeclarativeExtractor" /> 提取器集合
    /// </summary>
    internal static readonly ConcurrentDictionary<Type, IHttpDeclarativeExtractor> _extractors = new([
        new(typeof(BaseAddressDeclarativeExtractor), new BaseAddressDeclarativeExtractor()),
        new(typeof(ValidationDeclarativeExtractor), new ValidationDeclarativeExtractor()),
        new(typeof(AutoSetHostHeaderDeclarativeExtractor), new AutoSetHostHeaderDeclarativeExtractor()),
        new(typeof(PerformanceOptimizationDeclarativeExtractor), new PerformanceOptimizationDeclarativeExtractor()),
        new(typeof(HttpClientNameDeclarativeExtractor), new HttpClientNameDeclarativeExtractor()),
        new(typeof(TraceIdentifierDeclarativeExtractor), new TraceIdentifierDeclarativeExtractor()),
        new(typeof(ProfilerDeclarativeExtractor), new ProfilerDeclarativeExtractor()),
        new(typeof(SimulateBrowserDeclarativeExtractor), new SimulateBrowserDeclarativeExtractor()),
        new(typeof(AcceptLanguageDeclarativeExtractor), new AcceptLanguageDeclarativeExtractor()),
        new(typeof(DisableCacheDeclarativeExtractor), new DisableCacheDeclarativeExtractor()),
        new(typeof(EnsureSuccessStatusCodeDeclarativeExtractor), new EnsureSuccessStatusCodeDeclarativeExtractor()),
        new(typeof(TimeoutDeclarativeExtractor), new TimeoutDeclarativeExtractor()),
        new(typeof(PathSegmentDeclarativeExtractor), new PathSegmentDeclarativeExtractor()),
        new(typeof(QueryDeclarativeExtractor), new QueryDeclarativeExtractor()),
        new(typeof(PathDeclarativeExtractor), new PathDeclarativeExtractor()),
        new(typeof(CookieDeclarativeExtractor), new CookieDeclarativeExtractor()),
        new(typeof(RefererDeclarativeExtractor), new RefererDeclarativeExtractor()),
        new(typeof(HeaderDeclarativeExtractor), new HeaderDeclarativeExtractor()),
        new(typeof(PropertyDeclarativeExtractor), new PropertyDeclarativeExtractor()),
        new(typeof(HttpVersionDeclarativeExtractor), new HttpVersionDeclarativeExtractor()),
        new(typeof(SuppressExceptionsDeclarativeExtractor), new SuppressExceptionsDeclarativeExtractor()),
        new(typeof(RequestEventHandlerDeclarativeExtractor), new RequestEventHandlerDeclarativeExtractor()),
        new(typeof(JsonResponseWrappingDeclarativeExtractor), new JsonResponseWrappingDeclarativeExtractor()),
        new(typeof(BodyDeclarativeExtractor), new BodyDeclarativeExtractor())
    ]);

    /// <summary>
    ///     HTTP 声明式 <see cref="IHttpDeclarativeExtractor" /> 提取器集合（冻结）
    /// </summary>
    /// <remarks>该集合用于确保某些 HTTP 声明式提取器始终位于最后。</remarks>
    internal static readonly ConcurrentDictionary<Type, IFrozenHttpDeclarativeExtractor> _frozenExtractors = new([
        new(typeof(HttpRequestMessageDeclarativeExtractor), new HttpRequestMessageDeclarativeExtractor()),
        new(typeof(MultipartDeclarativeExtractor), new MultipartDeclarativeExtractor()),
        new(typeof(HttpMultipartFormDataBuilderDeclarativeExtractor),
            new HttpMultipartFormDataBuilderDeclarativeExtractor()),
        new(typeof(HttpRequestBuilderDeclarativeExtractor), new HttpRequestBuilderDeclarativeExtractor())
    ]);

    /// <summary>
    ///     标识是否已加载自定义 HTTP 声明式提取器
    /// </summary>
    internal static int _hasLoadedExtractors;

    /// <summary>
    ///     HTTP 声明式提取器缓存
    /// </summary>
    internal static volatile IHttpDeclarativeExtractor[]? _cachedExtractors;

    /// <summary>
    ///     <inheritdoc cref="HttpDeclarativeBuilder" />
    /// </summary>
    /// <param name="method">被调用方法</param>
    /// <param name="args">被调用方法的参数值数组</param>
    /// <param name="interfaceType">实际被代理的接口类型</param>
    internal HttpDeclarativeBuilder(MethodInfo method, object?[] args, Type? interfaceType = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(args);

        Method = method;
        Args = args;
        InterfaceType = interfaceType ?? Method.DeclaringType ?? throw new ArgumentNullException(nameof(interfaceType));
    }

    /// <summary>
    ///     被调用方法
    /// </summary>
    public MethodInfo Method { get; }

    /// <summary>
    ///     被调用方法的参数值数组
    /// </summary>
    public object?[] Args { get; }

    /// <summary>
    ///     实际被代理的接口类型
    /// </summary>
    public Type InterfaceType { get; }

    /// <summary>
    ///     构建 <see cref="HttpRequestBuilder" /> 实例
    /// </summary>
    /// <param name="httpRemoteOptions">
    ///     <see cref="HttpRemoteOptions" />
    /// </param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal HttpRequestBuilder Build(HttpRemoteOptions httpRemoteOptions, IServiceProvider? serviceProvider)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRemoteOptions);

        // 初始化方法信息友好字符串
        var declaringTypeFriendlyString = Method.DeclaringType?.ToFriendlyString();
        var methodFriendlyString = Method.ToFriendlyString();

        // 获取 HttpMethodAttribute 实例并检查被调用方法是否贴有 [HttpMethod] 特性
        var httpMethodAttribute = Method.GetCustomAttribute<HttpMethodAttribute>(true);
        if (httpMethodAttribute is null)
        {
            throw new InvalidOperationException(
                $"No `[HttpMethod]` annotation was found in method `{methodFriendlyString}` of type `{declaringTypeFriendlyString}`.");
        }

        // 初始化 HttpRequestBuilder 实例并添加声明式方法签名
        var httpRequestBuilder = HttpRequestBuilder
            .Create(httpMethodAttribute.HttpMethod, httpMethodAttribute.RequestUri)
            .WithProperty(Constants.DECLARATIVE_METHOD_KEY, $"{methodFriendlyString} | {declaringTypeFriendlyString}");

        // 初始化 HttpDeclarativeExtractorContext 实例
        var httpDeclarativeExtractorContext = new HttpDeclarativeExtractorContext(Method, Args,
            new HttpDeclarativeMethodMetadata(Method, InterfaceType), serviceProvider);

        // 检查是否已加载自定义 HTTP 声明式提取器
        if (Interlocked.CompareExchange(ref _hasLoadedExtractors, 1, 0) == 0)
        {
            // 批量添加自定义 HTTP 声明式提取器列表
            var customExtractors = httpRemoteOptions.HttpDeclarativeExtractors?.SelectMany(u => u.Invoke()) ?? [];
            foreach (var extractor in customExtractors)
            {
                // 获取 HTTP 声明式提取器类型
                var extractorType = extractor.GetType();

                // 检查 HTTP 声明式提取器是否实现 IFrozenHttpDeclarativeExtractor 接口
                if (extractor is IFrozenHttpDeclarativeExtractor frozenExtractor)
                {
                    _frozenExtractors.TryAdd(extractorType, frozenExtractor);
                }
                else
                {
                    _extractors.TryAdd(extractorType, extractor);
                }
            }

            // 自定义提取器已变更，清空缓存
            _cachedExtractors = null;
        }

        // 组合所有 HTTP 声明式提取器
        var extractors = _cachedExtractors;
        if (extractors is null)
        {
            var newExtractors = _extractors.Values
                .Concat(_frozenExtractors.Values.OrderByDescending(e => e.Order).ToArray()).ToArray();
            extractors = Interlocked.CompareExchange(ref _cachedExtractors, newExtractors, null) ?? newExtractors;
        }

        // 遍历 HTTP 声明式提取器集合
        foreach (var extractor in extractors)
        {
            // 提取方法信息构建 HttpRequestBuilder 实例
            extractor.Extract(httpRequestBuilder, httpDeclarativeExtractorContext);
        }

        return httpRequestBuilder;
    }
}