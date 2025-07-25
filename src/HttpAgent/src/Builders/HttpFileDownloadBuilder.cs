﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 文件下载构建器
/// </summary>
/// <remarks>使用 <c>HttpRequestBuilder.DownloadFile(requestUri, destinationPath)</c> 静态方法创建。</remarks>
public sealed class HttpFileDownloadBuilder
{
    /// <summary>
    ///     <inheritdoc cref="HttpFileDownloadBuilder" />
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    internal HttpFileDownloadBuilder(HttpMethod httpMethod, Uri? requestUri)
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
    ///     用于传输操作的缓冲区大小
    /// </summary>
    /// <remarks>以字节为单位，默认值为 <c>80 KB</c>。</remarks>
    public int BufferSize { get; private set; } = 80 * 1024;

    /// <summary>
    ///     文件保存的目标路径
    /// </summary>
    public string? DestinationPath { get; private set; }

    /// <summary>
    ///     当目标文件已存在时的行为
    /// </summary>
    /// <remarks>默认值为创建新文件，如果文件已存在则抛出异常。</remarks>
    public FileExistsBehavior FileExistsBehavior { get; private set; } = FileExistsBehavior.CreateNew;

    /// <summary>
    ///     进度更新（通知）的间隔时间
    /// </summary>
    /// <remarks>默认值为 1 秒。</remarks>
    public TimeSpan ProgressInterval { get; private set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    ///     用于处理在文件开始传输时的操作
    /// </summary>
    public Action? OnTransferStarted { get; private set; }

    /// <summary>
    ///     用于处理在文件传输完成时的操作
    /// </summary>
    public Action<long>? OnTransferCompleted { get; private set; }

    /// <summary>
    ///     用于处理在文件传输发生异常时的操作
    /// </summary>
    public Action<Exception>? OnTransferFailed { get; private set; }

    /// <summary>
    ///     用于处理在文件存在且配置为跳过时的操作
    /// </summary>
    public Action? OnFileExistAndSkip { get; private set; }

    /// <summary>
    ///     用于传输进度发生变化时的操作
    /// </summary>
    public Func<FileTransferProgress, Task>? OnProgressChanged { get; private set; }

    /// <summary>
    ///     实现 <see cref="IHttpFileTransferEventHandler" /> 的类型
    /// </summary>
    internal Type? FileTransferEventHandlerType { get; private set; }

    /// <summary>
    ///     <see cref="HttpRequestBuilder" /> 配置委托
    /// </summary>
    internal Action<HttpRequestBuilder>? RequestConfigure { get; private set; }

    /// <summary>
    ///     设置用于传输操作的缓冲区大小
    /// </summary>
    /// <param name="bufferSize">用于传输操作的缓冲区大小</param>
    /// <remarks>以字节为单位，默认值为 <c>80 KB</c>。</remarks>
    /// <returns>
    ///     <see cref="HttpFileDownloadBuilder" />
    /// </returns>
    public HttpFileDownloadBuilder SetBufferSize(int bufferSize)
    {
        // 小于或等于 0 检查
        if (bufferSize <= 0)
        {
            throw new ArgumentException("Buffer size must be greater than 0.", nameof(bufferSize));
        }

        BufferSize = bufferSize;

        return this;
    }

    /// <summary>
    ///     设置文件保存的目标路径
    /// </summary>
    /// <param name="destinationPath">文件保存的目标路径</param>
    /// <remarks>
    ///     如果设置为 <c>null</c>，则尝试获取 HTTP 模块的 <see cref="HttpRemoteBuilder" /> 构建器的 <c>DefaultFileDownloadDirectory</c>
    ///     的属性配置。
    /// </remarks>
    /// <returns>
    ///     <see cref="HttpFileDownloadBuilder" />
    /// </returns>
    public HttpFileDownloadBuilder SetDestinationPath(string? destinationPath)
    {
        // 跳过空检查
        if (destinationPath is null)
        {
            return this;
        }

        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath);

        DestinationPath = destinationPath;

        return this;
    }

    /// <summary>
    ///     设置当目标文件已存在时的行为
    /// </summary>
    /// <param name="fileExistsBehavior">
    ///     <see cref="FileExistsBehavior" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpFileDownloadBuilder" />
    /// </returns>
    public HttpFileDownloadBuilder SetFileExistsBehavior(FileExistsBehavior fileExistsBehavior)
    {
        FileExistsBehavior = fileExistsBehavior;

        return this;
    }

    /// <summary>
    ///     设置文件传输进度（通知）的间隔时间
    /// </summary>
    /// <param name="progressInterval">进度更新（通知）的间隔时间</param>
    /// <returns>
    ///     <see cref="HttpFileDownloadBuilder" />
    /// </returns>
    public HttpFileDownloadBuilder SetProgressInterval(TimeSpan progressInterval)
    {
        // 小于或等于 0 检查
        if (progressInterval <= TimeSpan.Zero)
        {
            throw new ArgumentException("Progress interval must be greater than 0.", nameof(progressInterval));
        }

        ProgressInterval = progressInterval;

        return this;
    }

    /// <summary>
    ///     设置在文件开始传输时的操作
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpFileDownloadBuilder" />
    /// </returns>
    public HttpFileDownloadBuilder SetOnTransferStarted(Action configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        OnTransferStarted = configure;

        return this;
    }

    /// <summary>
    ///     设置用于传输进度发生变化时执行的委托
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpFileDownloadBuilder" />
    /// </returns>
    public HttpFileDownloadBuilder SetOnProgressChanged(Func<FileTransferProgress, Task> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        OnProgressChanged = configure;

        return this;
    }

    /// <summary>
    ///     设置在文件传输完成时的操作
    /// </summary>
    /// <param name="configure">自定义配置委托；委托参数为文件传输总花费时间（毫秒）</param>
    /// <returns>
    ///     <see cref="HttpFileDownloadBuilder" />
    /// </returns>
    public HttpFileDownloadBuilder SetOnTransferCompleted(Action<long> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        OnTransferCompleted = configure;

        return this;
    }

    /// <summary>
    ///     设置在文件传输发生异常时的操作
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpFileDownloadBuilder" />
    /// </returns>
    public HttpFileDownloadBuilder SetOnTransferFailed(Action<Exception> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        OnTransferFailed = configure;

        return this;
    }

    /// <summary>
    ///     设置在文件存在且配置为跳过时的操作
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpFileDownloadBuilder" />
    /// </returns>
    public HttpFileDownloadBuilder SetOnFileExistAndSkip(Action configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        OnFileExistAndSkip = configure;

        return this;
    }

    /// <summary>
    ///     设置 HTTP 文件传输事件处理程序
    /// </summary>
    /// <param name="fileTransferEventHandlerType">实现 <see cref="IHttpFileTransferEventHandler" /> 接口的类型</param>
    /// <returns>
    ///     <see cref="HttpFileDownloadBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public HttpFileDownloadBuilder SetEventHandler(Type fileTransferEventHandlerType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(fileTransferEventHandlerType);

        // 检查类型是否实现了 IHttpFileTransferEventHandler 接口
        if (!typeof(IHttpFileTransferEventHandler).IsAssignableFrom(fileTransferEventHandlerType))
        {
            throw new ArgumentException(
                $"`{fileTransferEventHandlerType}` type is not assignable from `{typeof(IHttpFileTransferEventHandler)}`.",
                nameof(fileTransferEventHandlerType));
        }

        FileTransferEventHandlerType = fileTransferEventHandlerType;

        return this;
    }

    /// <summary>
    ///     设置 HTTP 文件传输事件处理程序
    /// </summary>
    /// <typeparam name="TFileTransferEventHandler">
    ///     <see cref="IHttpFileTransferEventHandler" />
    /// </typeparam>
    /// <returns>
    ///     <see cref="HttpFileDownloadBuilder" />
    /// </returns>
    public HttpFileDownloadBuilder SetEventHandler<TFileTransferEventHandler>()
        where TFileTransferEventHandler : IHttpFileTransferEventHandler =>
        SetEventHandler(typeof(TFileTransferEventHandler));

    /// <summary>
    ///     配置 <see cref="HttpRequestBuilder" /> 实例
    /// </summary>
    /// <remarks>支持多次调用。</remarks>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpFileDownloadBuilder" />
    /// </returns>
    public HttpFileDownloadBuilder WithRequest(Action<HttpRequestBuilder> configure)
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

        // 检查是否设置了文件保存的目标路径，如果没有则设置为默认文件下载保存目录
        DestinationPath ??= httpRemoteOptions.DefaultFileDownloadDirectory;

        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(DestinationPath);

        // 初始化 HttpRequestBuilder 实例；如果请求失败，则应抛出异常。
        var httpRequestBuilder = HttpRequestBuilder.Create(HttpMethod, RequestUri).PerformanceOptimization()
            .EnsureSuccessStatusCode();

        // 检查是否设置了事件处理程序且该处理程序实现了 IHttpRequestEventHandler 接口，如果有则设置给 httpRequestBuilder
        if (FileTransferEventHandlerType is not null &&
            typeof(IHttpRequestEventHandler).IsAssignableFrom(FileTransferEventHandlerType))
        {
            httpRequestBuilder.SetEventHandler(FileTransferEventHandlerType);
        }

        // 调用自定义配置委托
        RequestConfigure?.Invoke(httpRequestBuilder);

        return httpRequestBuilder;
    }
}