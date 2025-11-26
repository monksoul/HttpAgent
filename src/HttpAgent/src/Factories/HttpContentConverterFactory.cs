// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <inheritdoc cref="IHttpContentConverterFactory" />
internal sealed class HttpContentConverterFactory : IHttpContentConverterFactory
{
    /// <summary>
    ///     <see cref="IHttpContentConverter{TResult}" /> 字典集合
    /// </summary>
    internal readonly Dictionary<Type, IHttpContentConverter> _converters;

    /// <inheritdoc cref="IHttpRemoteLogger" />
    internal readonly IHttpRemoteLogger _logger;

    /// <summary>
    ///     <inheritdoc cref="HttpContentConverterFactory" />
    /// </summary>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="logger">
    ///     <see cref="IHttpRemoteLogger" />
    /// </param>
    /// <param name="converters"><see cref="IHttpContentConverter{TResult}" /> 数组</param>
    public HttpContentConverterFactory(IServiceProvider serviceProvider, IHttpRemoteLogger logger,
        IHttpContentConverter[]? converters)
    {
        ServiceProvider = serviceProvider;
        _logger = logger;

        // 初始化响应内容转换器
        _converters = new Dictionary<Type, IHttpContentConverter>
        {
            [typeof(HttpResponseMessageConverter)] = new HttpResponseMessageConverter(),
            [typeof(StringContentConverter)] = new StringContentConverter(),
            [typeof(ByteArrayContentConverter)] = new ByteArrayContentConverter(),
            [typeof(StreamContentConverter)] = new StreamContentConverter(),
            [typeof(VoidContentConverter)] = new VoidContentConverter()
        };

        // 添加自定义 IHttpContentConverter 数组
        _converters.TryAdd(converters, value => value.GetType());
    }

    /// <inheritdoc />
    public IServiceProvider ServiceProvider { get; }

    /// <inheritdoc />
    public TResult? Read<TResult>(HttpResponseMessage? httpResponseMessage, IHttpContentConverter[]? converters = null,
        CancellationToken cancellationToken = default)
    {
        // 空检查
        if (httpResponseMessage is null)
        {
            return default;
        }

        try
        {
            return GetConverter<TResult>(httpResponseMessage, converters).Read(httpResponseMessage, cancellationToken);
        }
        catch (Exception e)
        {
            // 输出转换异常日志
            LogContentConversionError(typeof(TResult), httpResponseMessage, e);

            throw;
        }
    }

    /// <inheritdoc />
    public object? Read(Type resultType, HttpResponseMessage? httpResponseMessage,
        IHttpContentConverter[]? converters = null, CancellationToken cancellationToken = default)
    {
        // 空检查
        if (httpResponseMessage is null)
        {
            return null;
        }

        try
        {
            return GetConverter(resultType, httpResponseMessage, converters)
                .Read(resultType, httpResponseMessage, cancellationToken);
        }
        catch (Exception e)
        {
            // 输出转换异常日志
            LogContentConversionError(resultType, httpResponseMessage, e);

            throw;
        }
    }

    /// <inheritdoc />
    public async Task<TResult?> ReadAsync<TResult>(HttpResponseMessage? httpResponseMessage,
        IHttpContentConverter[]? converters = null, CancellationToken cancellationToken = default)
    {
        // 空检查
        if (httpResponseMessage is null)
        {
            return default;
        }

        try
        {
            return await GetConverter<TResult>(httpResponseMessage, converters)
                .ReadAsync(httpResponseMessage, cancellationToken);
        }
        catch (Exception e)
        {
            // 输出转换异常日志
            LogContentConversionError(typeof(TResult), httpResponseMessage, e);

            throw;
        }
    }

    /// <inheritdoc />
    public async Task<object?> ReadAsync(Type resultType, HttpResponseMessage? httpResponseMessage,
        IHttpContentConverter[]? converters = null, CancellationToken cancellationToken = default)
    {
        // 空检查
        if (httpResponseMessage is null)
        {
            return null;
        }

        try
        {
            return await GetConverter(resultType, httpResponseMessage, converters)
                .ReadAsync(resultType, httpResponseMessage, cancellationToken);
        }
        catch (Exception e)
        {
            // 输出转换异常日志
            LogContentConversionError(resultType, httpResponseMessage, e);

            throw;
        }
    }

    /// <summary>
    ///     获取 <see cref="IHttpContentConverter{TResult}" /> 实例
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="converters"><see cref="IHttpContentConverter{TResult}" /> 数组</param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <see cref="IHttpContentConverter{TResult}" />
    /// </returns>
    internal IHttpContentConverter<TResult> GetConverter<TResult>(HttpResponseMessage httpResponseMessage,
        params IHttpContentConverter[]? converters)
    {
        // 初始化新的 IHttpContentConverter 字典集合
        var unionConverters = new Dictionary<Type, IHttpContentConverter>(_converters);

        // 添加自定义 IHttpContentConverter 数组
        unionConverters.TryAdd(converters, value => value.GetType());

        // 初始化目标类型响应内容转换器
        IHttpContentConverter<TResult> targetConverter;

        // 检查是否启用 JSON 响应反序列化包装器或目标转换类型为 void/VoidContent
        if (httpResponseMessage.IsEnableJsonResponseWrapping(ServiceProvider) &&
            !(typeof(TResult) == typeof(void) || typeof(VoidContent).IsAssignableFrom(typeof(TResult))))
        {
            // 调用 IObjectContentConverterFactory 实例的 GetConverter<TResult> 返回
            targetConverter = ServiceProvider.GetRequiredService<IObjectContentConverterFactory>()
                .GetConverter<TResult>(httpResponseMessage);
        }
        else
        {
            // 查找可以处理目标类型的响应内容转换器
            var typeConverter = unionConverters.Values.OfType<IHttpContentConverter<TResult>>().LastOrDefault();

            // 如果未找到，则调用 IObjectContentConverterFactory 实例的 GetConverter<TResult> 返回
            targetConverter = typeConverter ?? ServiceProvider.GetRequiredService<IObjectContentConverterFactory>()
                .GetConverter<TResult>(httpResponseMessage);
        }

        // 设置服务提供器
        targetConverter.ServiceProvider ??= ServiceProvider;

        return targetConverter;
    }

    /// <summary>
    ///     获取 <see cref="IHttpContentConverter" /> 实例
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="resultType">转换的目标类型</param>
    /// <param name="converters"><see cref="IHttpContentConverter{TResult}" /> 数组</param>
    /// <returns>
    ///     <see cref="IHttpContentConverter" />
    /// </returns>
    internal IHttpContentConverter GetConverter(Type resultType, HttpResponseMessage httpResponseMessage,
        params IHttpContentConverter[]? converters)
    {
        // 初始化新的 IHttpContentConverter 字典集合
        var unionConverters = new Dictionary<Type, IHttpContentConverter>(_converters);

        // 添加自定义 IHttpContentConverter 数组
        unionConverters.TryAdd(converters, value => value.GetType());

        // 初始化目标类型响应内容转换器
        IHttpContentConverter targetConverter;

        // 检查是否启用 JSON 响应反序列化包装器或目标转换类型为 void/VoidContent
        if (httpResponseMessage.IsEnableJsonResponseWrapping(ServiceProvider) &&
            !(resultType == typeof(void) || typeof(VoidContent).IsAssignableFrom(resultType)))
        {
            // 调用 IObjectContentConverterFactory 实例的 GetConverter 返回
            targetConverter = ServiceProvider.GetRequiredService<IObjectContentConverterFactory>()
                .GetConverter(resultType, httpResponseMessage);
        }
        else
        {
            // 查找可以处理目标类型的响应内容转换器
            var typeConverter = unionConverters.Values
                .OfType(typeof(IHttpContentConverter<>).MakeGenericType(resultType)).Cast<IHttpContentConverter>()
                .LastOrDefault();

            // 如果未找到，则调用 IObjectContentConverterFactory 实例的 GetConverter 返回
            targetConverter = typeConverter ?? ServiceProvider.GetRequiredService<IObjectContentConverterFactory>()
                .GetConverter(resultType, httpResponseMessage);
        }

        // 设置服务提供器
        targetConverter.ServiceProvider ??= ServiceProvider;

        return targetConverter;
    }

    /// <summary>
    ///     输出转换异常日志
    /// </summary>
    /// <param name="resultType">转换的目标类型</param>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="exception">
    ///     <see cref="Exception" />
    /// </param>
    internal void LogContentConversionError(Type resultType, HttpResponseMessage httpResponseMessage,
        Exception exception) =>
        _logger.LogError(exception,
            "Failed to convert HTTP response to type {ResultType}. Status: {StatusCode} {StatusDescription}, URI: {RequestUri}",
            resultType.FullName!, (int)httpResponseMessage.StatusCode, httpResponseMessage.StatusCode.ToString(),
            httpResponseMessage.RequestMessage?.RequestUri?.ToString() ?? "unknown");
}