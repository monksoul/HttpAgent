// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 长轮询构建器
/// </summary>
/// <remarks>使用 <c>HttpRequestBuilder.LongPolling(httpMethod, requestUri, onDataReceived)</c> 静态方法创建。</remarks>
public sealed class HttpLongPollingBuilder
{
    /// <summary>
    ///     <inheritdoc cref="HttpLongPollingBuilder" />
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    internal HttpLongPollingBuilder(HttpMethod httpMethod, Uri? requestUri)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpMethod);

        HttpMethod = httpMethod;
        RequestUri = requestUri;
    }

    /// <summary>
    ///     请求地址
    /// </summary>
    public Uri? RequestUri { get; }

    /// <summary>
    ///     请求方式
    /// </summary>
    public HttpMethod HttpMethod { get; }

    /// <summary>
    ///     超时时间
    /// </summary>
    /// <remarks>可为单次请求设置超时时间。</remarks>
    public TimeSpan? Timeout { get; private set; }

    /// <summary>
    ///     轮询重试间隔
    /// </summary>
    /// <remarks>默认值为 2 秒。</remarks>
    public TimeSpan RetryInterval { get; private set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    ///     最大重试次数
    /// </summary>
    /// <remarks>默认最大重试次数为 100。</remarks>
    public int MaxRetries { get; private set; } = 100;

    /// <summary>
    ///     用于接收服务器返回 <c>200~299</c> 状态码的数据的操作
    /// </summary>
    public Func<HttpResponseMessage, CancellationToken, Task>? OnDataReceived { get; private set; }

    /// <summary>
    ///     用于接收服务器返回非 <c>200~299</c> 状态码的数据的操作
    /// </summary>
    public Func<HttpResponseMessage, CancellationToken, Task>? OnError { get; private set; }

    /// <summary>
    ///     用于响应标头包含 <c>X-End-Of-Stream</c> 时触发的操作
    /// </summary>
    public Func<HttpResponseMessage, CancellationToken, Task>? OnEndOfStream { get; private set; }

    /// <summary>
    ///     实现 <see cref="IHttpLongPollingEventHandler" /> 的类型
    /// </summary>
    internal Type? LongPollingEventHandlerType { get; private set; }

    /// <summary>
    ///     <see cref="HttpRequestBuilder" /> 配置委托
    /// </summary>
    internal Action<HttpRequestBuilder>? RequestConfigure { get; private set; }

    /// <summary>
    ///     设置轮询重试间隔
    /// </summary>
    /// <param name="retryInterval">轮询重试间隔</param>
    /// <returns>
    ///     <see cref="HttpLongPollingBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public HttpLongPollingBuilder SetRetryInterval(TimeSpan retryInterval)
    {
        // 小于或等于 0 检查
        if (retryInterval <= TimeSpan.Zero)
        {
            throw new ArgumentException("Retry interval must be greater than 0.", nameof(retryInterval));
        }

        RetryInterval = retryInterval;

        return this;
    }

    /// <summary>
    ///     设置最大重试次数
    /// </summary>
    /// <param name="maxRetries">最大重试次数</param>
    /// <returns>
    ///     <see cref="HttpLongPollingBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public HttpLongPollingBuilder SetMaxRetries(int maxRetries)
    {
        // 小于或等于 0 检查
        if (maxRetries <= 0)
        {
            throw new ArgumentException("Max retries must be greater than 0.", nameof(maxRetries));
        }

        MaxRetries = maxRetries;

        return this;
    }

    /// <summary>
    ///     设置超时时间
    /// </summary>
    /// <param name="timeout">超时时间</param>
    /// <returns>
    ///     <see cref="HttpLongPollingBuilder" />
    /// </returns>
    public HttpLongPollingBuilder SetTimeout(TimeSpan timeout)
    {
        Timeout = timeout;

        return this;
    }

    /// <summary>
    ///     设置超时时间
    /// </summary>
    /// <param name="timeoutMilliseconds">超时时间（毫秒）</param>
    /// <returns>
    ///     <see cref="HttpLongPollingBuilder" />
    /// </returns>
    public HttpLongPollingBuilder SetTimeout(double timeoutMilliseconds)
    {
        // 检查参数是否小于 0
        if (timeoutMilliseconds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeoutMilliseconds), "Timeout value must be non-negative.");
        }

        Timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

        return this;
    }

    /// <summary>
    ///     设置在接收服务器返回 <c>200~299</c> 状态码的数据的操作
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpLongPollingBuilder" />
    /// </returns>
    public HttpLongPollingBuilder SetOnDataReceived(Func<HttpResponseMessage, CancellationToken, Task> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        OnDataReceived = configure;

        return this;
    }

    /// <summary>
    ///     设置在接收服务器返回非 <c>200~299</c> 状态码的数据的操作
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpLongPollingBuilder" />
    /// </returns>
    public HttpLongPollingBuilder SetOnError(Func<HttpResponseMessage, CancellationToken, Task> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        OnError = configure;

        return this;
    }

    /// <summary>
    ///     设置在响应标头包含 <c>X-End-Of-Stream</c> 时触发的操作
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpLongPollingBuilder" />
    /// </returns>
    public HttpLongPollingBuilder SetOnEndOfStream(Func<HttpResponseMessage, CancellationToken, Task> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        OnEndOfStream = configure;

        return this;
    }

    /// <summary>
    ///     设置长轮询事件处理程序
    /// </summary>
    /// <param name="longPollingEventHandlerType">实现 <see cref="IHttpLongPollingEventHandler" /> 接口的类型</param>
    /// <returns>
    ///     <see cref="HttpLongPollingBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public HttpLongPollingBuilder SetEventHandler(Type longPollingEventHandlerType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(longPollingEventHandlerType);

        // 检查类型是否实现了 IHttpLongPollingEventHandler 接口
        if (!typeof(IHttpLongPollingEventHandler).IsAssignableFrom(longPollingEventHandlerType))
        {
            throw new ArgumentException(
                $"`{longPollingEventHandlerType}` type is not assignable from `{typeof(IHttpLongPollingEventHandler)}`.",
                nameof(longPollingEventHandlerType));
        }

        LongPollingEventHandlerType = longPollingEventHandlerType;

        return this;
    }

    /// <summary>
    ///     设置长轮询事件处理程序
    /// </summary>
    /// <typeparam name="TLongPollingEventHandler">
    ///     <see cref="IHttpLongPollingEventHandler" />
    /// </typeparam>
    /// <returns>
    ///     <see cref="HttpLongPollingBuilder" />
    /// </returns>
    public HttpLongPollingBuilder SetEventHandler<TLongPollingEventHandler>()
        where TLongPollingEventHandler : IHttpLongPollingEventHandler =>
        SetEventHandler(typeof(TLongPollingEventHandler));

    /// <summary>
    ///     配置 <see cref="HttpRequestBuilder" /> 实例
    /// </summary>
    /// <remarks>支持多次调用。</remarks>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpLongPollingBuilder" />
    /// </returns>
    public HttpLongPollingBuilder WithRequest(Action<HttpRequestBuilder> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        // 如果 RequestConfigure 未设置则直接赋值
        if (RequestConfigure is null)
        {
            RequestConfigure = configure;
        }
        // 否则创建级联调用委托
        else
        {
            // 复制一个新的委托避免死循环
            var originalRequestConfigure = RequestConfigure;

            RequestConfigure = httpRequestBuilder =>
            {
                originalRequestConfigure.Invoke(httpRequestBuilder);
                configure.Invoke(httpRequestBuilder);
            };
        }

        return this;
    }

    /// <summary>
    ///     构建 <see cref="HttpRequestBuilder" /> 实例
    /// </summary>
    /// <param name="httpRemoteOptions">
    ///     <see cref="HttpRemoteOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    internal HttpRequestBuilder Build(HttpRemoteOptions httpRemoteOptions)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRemoteOptions);

        // 初始化 HttpRequestBuilder 实例
        var httpRequestBuilder = HttpRequestBuilder.Create(HttpMethod, RequestUri).DisableCache();

        // 设置超时时间
        if (Timeout is not null)
        {
            httpRequestBuilder.SetTimeout(Timeout.Value);
        }

        // 检查是否设置了事件处理程序且该处理程序实现了 IHttpRequestEventHandler 接口，如果有则设置给 httpRequestBuilder
        if (LongPollingEventHandlerType is not null &&
            typeof(IHttpRequestEventHandler).IsAssignableFrom(LongPollingEventHandlerType))
        {
            httpRequestBuilder.SetEventHandler(LongPollingEventHandlerType);
        }

        // 调用自定义配置委托
        RequestConfigure?.Invoke(httpRequestBuilder);

        return httpRequestBuilder;
    }
}