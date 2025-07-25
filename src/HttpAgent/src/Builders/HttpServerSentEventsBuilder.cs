﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP Server-Sent Events 构建器
/// </summary>
/// <remarks>
///     <para>使用 <c>HttpRequestBuilder.ServerSentEvents(requestUri, onMessage)</c> 静态方法创建。</para>
///     <para>参考文献：https://developer.mozilla.org/zh-CN/docs/Web/API/Server-sent_events/Using_server-sent_events。</para>
/// </remarks>
public sealed class HttpServerSentEventsBuilder
{
    /// <summary>
    ///     <inheritdoc cref="HttpServerSentEventsBuilder" />
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    internal HttpServerSentEventsBuilder(Uri? requestUri)
        : this(HttpMethod.Get, requestUri)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="HttpServerSentEventsBuilder" />
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    internal HttpServerSentEventsBuilder(HttpMethod httpMethod, Uri? requestUri)
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
    /// <remarks>默认请求为：<c>GET</c>。</remarks>
    public HttpMethod HttpMethod { get; }

    /// <summary>
    ///     默认重新连接的间隔时间（毫秒）
    /// </summary>
    /// <remarks>默认值为 2000 毫秒。</remarks>
    public int DefaultRetryInterval { get; private set; } = 2000;

    /// <summary>
    ///     最大重试次数
    /// </summary>
    /// <remarks>默认最大重试次数为 100。</remarks>
    public int MaxRetries { get; private set; } = 100;

    /// <summary>
    ///     用于在与事件源的连接打开时的操作
    /// </summary>
    public Action? OnOpen { get; private set; }

    /// <summary>
    ///     用于在从事件源接收到数据时的操作
    /// </summary>
    public Func<ServerSentEventsData, CancellationToken, Task>? OnMessage { get; private set; }

    /// <summary>
    ///     用于在事件源连接未能打开时的操作
    /// </summary>
    public Action<Exception>? OnError { get; private set; }

    /// <summary>
    ///     实现 <see cref="IHttpServerSentEventsEventHandler" /> 的类型
    /// </summary>
    internal Type? ServerSentEventsEventHandlerType { get; private set; }

    /// <summary>
    ///     <see cref="HttpRequestBuilder" /> 配置委托
    /// </summary>
    internal Action<HttpRequestBuilder>? RequestConfigure { get; private set; }

    /// <summary>
    ///     设置默认重新连接的间隔时间
    /// </summary>
    /// <param name="retryInterval">默认重新连接的间隔时间</param>
    /// <returns>
    ///     <see cref="HttpServerSentEventsBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public HttpServerSentEventsBuilder SetDefaultRetryInterval(int retryInterval)
    {
        // 小于或等于 0 检查
        if (retryInterval <= 0)
        {
            throw new ArgumentException("Retry interval must be greater than 0.", nameof(retryInterval));
        }

        DefaultRetryInterval = retryInterval;

        return this;
    }

    /// <summary>
    ///     设置最大重试次数
    /// </summary>
    /// <param name="maxRetries">最大重试次数</param>
    /// <returns>
    ///     <see cref="HttpServerSentEventsBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public HttpServerSentEventsBuilder SetMaxRetries(int maxRetries)
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
    ///     设置用于在与事件源的连接打开时的操作
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpServerSentEventsBuilder" />
    /// </returns>
    public HttpServerSentEventsBuilder SetOnOpen(Action configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        OnOpen = configure;

        return this;
    }

    /// <summary>
    ///     设置用于在从事件源接收到数据时的操作
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpServerSentEventsBuilder" />
    /// </returns>
    public HttpServerSentEventsBuilder SetOnMessage(Func<ServerSentEventsData, CancellationToken, Task> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        OnMessage = configure;

        return this;
    }

    /// <summary>
    ///     设置用于在事件源连接未能打开时的操作
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpServerSentEventsBuilder" />
    /// </returns>
    public HttpServerSentEventsBuilder SetOnError(Action<Exception> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        OnError = configure;

        return this;
    }

    /// <summary>
    ///     设置 Server-Sent Events 事件处理程序
    /// </summary>
    /// <param name="serverSentEventsEventHandlerType">实现 <see cref="IHttpServerSentEventsEventHandler" /> 接口的类型</param>
    /// <returns>
    ///     <see cref="HttpServerSentEventsBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public HttpServerSentEventsBuilder SetEventHandler(Type serverSentEventsEventHandlerType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(serverSentEventsEventHandlerType);

        // 检查类型是否实现了 IHttpServerSentEventsEventHandler 接口
        if (!typeof(IHttpServerSentEventsEventHandler).IsAssignableFrom(serverSentEventsEventHandlerType))
        {
            throw new ArgumentException(
                $"`{serverSentEventsEventHandlerType}` type is not assignable from `{typeof(IHttpServerSentEventsEventHandler)}`.",
                nameof(serverSentEventsEventHandlerType));
        }

        ServerSentEventsEventHandlerType = serverSentEventsEventHandlerType;

        return this;
    }

    /// <summary>
    ///     设置 Server-Sent Events 事件处理程序
    /// </summary>
    /// <typeparam name="TServerSentEventsEventHandler">
    ///     <see cref="IHttpServerSentEventsEventHandler" />
    /// </typeparam>
    /// <returns>
    ///     <see cref="HttpServerSentEventsBuilder" />
    /// </returns>
    public HttpServerSentEventsBuilder SetEventHandler<TServerSentEventsEventHandler>()
        where TServerSentEventsEventHandler : IHttpServerSentEventsEventHandler =>
        SetEventHandler(typeof(TServerSentEventsEventHandler));

    /// <summary>
    ///     配置 <see cref="HttpRequestBuilder" /> 实例
    /// </summary>
    /// <remarks>支持多次调用。</remarks>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpServerSentEventsBuilder" />
    /// </returns>
    public HttpServerSentEventsBuilder WithRequest(Action<HttpRequestBuilder> configure)
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

        // 初始化 HttpRequestBuilder 实例，并确保请求标头中添加了 Accept: text/event-stream；同时启用 HttpClient 池化管理
        // 如果请求失败，则应抛出异常。
        var httpRequestBuilder = HttpRequestBuilder.Create(HttpMethod, RequestUri)
            .WithHeader(nameof(HttpRequestHeaders.Accept), "text/event-stream", replace: true).DisableCache()
            .UseHttpClientPool().EnsureSuccessStatusCode();

        // 检查是否设置了事件处理程序且该处理程序实现了 IHttpRequestEventHandler 接口，如果有则设置给 httpRequestBuilder
        if (ServerSentEventsEventHandlerType is not null &&
            typeof(IHttpRequestEventHandler).IsAssignableFrom(ServerSentEventsEventHandlerType))
        {
            httpRequestBuilder.SetEventHandler(ServerSentEventsEventHandlerType);
        }

        // 调用自定义配置委托
        RequestConfigure?.Invoke(httpRequestBuilder);

        return httpRequestBuilder;
    }
}