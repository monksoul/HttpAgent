// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <inheritdoc cref="IHttpContentProcessorFactory" />
internal sealed class HttpContentProcessorFactory : IHttpContentProcessorFactory
{
    /// <summary>
    ///     <see cref="IHttpContentProcessor" /> 字典集合
    /// </summary>
    internal readonly Dictionary<Type, IHttpContentProcessor> _processors;

    /// <summary>
    ///     <inheritdoc cref="HttpContentProcessorFactory" />
    /// </summary>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="processors"><see cref="IHttpContentProcessor" /> 集合</param>
    public HttpContentProcessorFactory(IServiceProvider serviceProvider, IEnumerable<IHttpContentProcessor> processors)
    {
        ServiceProvider = serviceProvider;

        // 初始化请求内容处理器
        _processors = new Dictionary<Type, IHttpContentProcessor>
        {
            [typeof(StringContentProcessor)] = new StringContentProcessor(),
            [typeof(FormUrlEncodedContentProcessor)] = new FormUrlEncodedContentProcessor(),
            [typeof(ByteArrayContentProcessor)] = new ByteArrayContentProcessor(),
            [typeof(StreamContentProcessor)] = new StreamContentProcessor(),
            [typeof(MultipartFormDataContentProcessor)] = new MultipartFormDataContentProcessor(),
            [typeof(ReadOnlyMemoryContentProcessor)] = new ReadOnlyMemoryContentProcessor(),
            [typeof(JsonLinesContentProcessor)] = new JsonLinesContentProcessor(),
            [typeof(FileInfoContentProcessor)] = new FileInfoContentProcessor()
        };

        // 添加自定义 IHttpContentProcessor 集合
        _processors.TryAdd(processors, value => value.GetType());
    }

    /// <inheritdoc />
    public IServiceProvider ServiceProvider { get; }

    /// <inheritdoc />
    public HttpContent? Build(HttpContentProcessorContext context, params IHttpContentProcessor[]? processors)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(context);

        // 查找可以处理指定内容类型或数据类型的 IHttpContentProcessor 实例
        var httpContentProcessor = GetProcessor(context, processors);

        // 将原始请求内容转换为 HttpContent 实例
        return httpContentProcessor.Process(context);
    }

    /// <summary>
    ///     查找可以处理指定内容类型或数据类型的 <see cref="IHttpContentProcessor" /> 实例
    /// </summary>
    /// <param name="context">
    ///     <see cref="HttpContentProcessorContext" />
    /// </param>
    /// <param name="processors">自定义 <see cref="IHttpContentProcessor" /> 数组</param>
    /// <returns>
    ///     <see cref="IHttpContentProcessor" />
    /// </returns>
    internal IHttpContentProcessor GetProcessor(HttpContentProcessorContext context,
        params IHttpContentProcessor[]? processors)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(context);

        // 初始化新的 IHttpContentProcessor 字典集合
        var unionProcessors = new Dictionary<Type, IHttpContentProcessor>(_processors);

        // 添加自定义 IHttpContentProcessor 数组
        unionProcessors.TryAdd(processors, value => value.GetType());

        // 查找可以处理指定内容类型或数据类型的 IHttpContentProcessor 实例
        var processor = unionProcessors.Values.LastOrDefault(u => u.CanProcess(context)) ??
                        throw new InvalidOperationException(
                            $"No processor found that can handle the content type `{context.ContentType}` and the provided raw content of type `{context.RawContent?.GetType()}`. " +
                            "Please ensure that the correct content type is specified and that a suitable processor is registered.");

        // 设置服务提供器
        processor.ServiceProvider ??= ServiceProvider;

        return processor;
    }
}