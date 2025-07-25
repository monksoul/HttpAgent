﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 文件上传构建器
/// </summary>
/// <remarks>使用 <c>HttpRequestBuilder.UploadFile(requestUri, filePath, name)</c> 静态方法创建。</remarks>
public sealed class HttpFileUploadBuilder
{
    /// <summary>
    ///     <inheritdoc cref="HttpFileUploadBuilder" />
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="filePath">文件路径</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    internal HttpFileUploadBuilder(HttpMethod httpMethod, Uri? requestUri, string filePath, string name,
        string? fileName = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpMethod);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        HttpMethod = httpMethod;
        RequestUri = requestUri;

        FilePath = filePath;
        Name = name;
        FileName = fileName;
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
    ///     文件路径
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    ///     文件的名称
    /// </summary>
    public string? FileName { get; }

    /// <summary>
    ///     表单名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     内容类型
    /// </summary>
    public string? ContentType { get; private set; }

    /// <summary>
    ///     允许的文件拓展名
    /// </summary>
    public string[]? AllowedFileExtensions { get; private set; }

    /// <summary>
    ///     允许的文件大小。以字节为单位
    /// </summary>
    public long? MaxFileSizeInBytes { get; private set; }

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
    ///     设置内容类型（文件类型）
    /// </summary>
    /// <param name="contentType">内容类型</param>
    /// <returns>
    ///     <see cref="HttpFileUploadBuilder" />
    /// </returns>
    public HttpFileUploadBuilder SetContentType(string contentType)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        // 解析内容类型字符串
        var mediaTypeHeaderValue = MediaTypeHeaderValue.Parse(contentType);

        ContentType = mediaTypeHeaderValue.MediaType;

        return this;
    }

    /// <summary>
    ///     设置允许的文件拓展名
    /// </summary>
    /// <param name="allowedFileExtensions">允许的文件拓展名</param>
    /// <returns>
    ///     <see cref="HttpFileUploadBuilder" />
    /// </returns>
    public HttpFileUploadBuilder SetAllowedFileExtensions(string[] allowedFileExtensions)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(allowedFileExtensions);

        AllowedFileExtensions = allowedFileExtensions;

        return this;
    }

    /// <summary>
    ///     设置允许的文件拓展名
    /// </summary>
    /// <param name="allowedFileExtensions">允许的文件扩展名字符串，用分号分隔</param>
    /// <returns>
    ///     <see cref="HttpFileUploadBuilder" />
    /// </returns>
    public HttpFileUploadBuilder SetAllowedFileExtensions(string allowedFileExtensions)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(allowedFileExtensions);

        AllowedFileExtensions = allowedFileExtensions.Split(';', StringSplitOptions.RemoveEmptyEntries);

        return this;
    }

    /// <summary>
    ///     设置允许的文件大小
    /// </summary>
    /// <param name="maxFileSizeInBytes">允许的文件大小。以字节为单位。</param>
    /// <returns>
    ///     <see cref="HttpFileUploadBuilder" />
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public HttpFileUploadBuilder SetMaxFileSizeInBytes(long maxFileSizeInBytes)
    {
        // 小于或等于 0 检查
        if (maxFileSizeInBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxFileSizeInBytes),
                "Max file size in bytes must be greater than zero.");
        }

        MaxFileSizeInBytes = maxFileSizeInBytes;

        return this;
    }

    /// <summary>
    ///     设置文件传输进度（通知）的间隔时间
    /// </summary>
    /// <param name="progressInterval">进度更新（通知）的间隔时间</param>
    /// <returns>
    ///     <see cref="HttpFileUploadBuilder" />
    /// </returns>
    public HttpFileUploadBuilder SetProgressInterval(TimeSpan progressInterval)
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
    ///     <see cref="HttpFileUploadBuilder" />
    /// </returns>
    public HttpFileUploadBuilder SetOnTransferStarted(Action configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        OnTransferStarted = configure;

        return this;
    }

    /// <summary>
    ///     设置用于上传进度发生变化时执行的委托
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpFileUploadBuilder" />
    /// </returns>
    public HttpFileUploadBuilder SetOnProgressChanged(Func<FileTransferProgress, Task> configure)
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
    ///     <see cref="HttpFileUploadBuilder" />
    /// </returns>
    public HttpFileUploadBuilder SetOnTransferCompleted(Action<long> configure)
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
    ///     <see cref="HttpFileUploadBuilder" />
    /// </returns>
    public HttpFileUploadBuilder SetOnTransferFailed(Action<Exception> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        OnTransferFailed = configure;

        return this;
    }

    /// <summary>
    ///     设置 HTTP 文件传输事件处理程序
    /// </summary>
    /// <param name="fileTransferEventHandlerType">实现 <see cref="IHttpFileTransferEventHandler" /> 接口的类型</param>
    /// <returns>
    ///     <see cref="HttpFileUploadBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public HttpFileUploadBuilder SetEventHandler(Type fileTransferEventHandlerType)
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
    ///     <see cref="HttpFileUploadBuilder" />
    /// </returns>
    public HttpFileUploadBuilder SetEventHandler<TFileTransferEventHandler>()
        where TFileTransferEventHandler : IHttpFileTransferEventHandler =>
        SetEventHandler(typeof(TFileTransferEventHandler));

    /// <summary>
    ///     配置 <see cref="HttpRequestBuilder" /> 实例
    /// </summary>
    /// <remarks>支持多次调用。</remarks>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpFileUploadBuilder" />
    /// </returns>
    public HttpFileUploadBuilder WithRequest(Action<HttpRequestBuilder> configure)
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
    /// <param name="progressChannel">文件传输进度信息的通道</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    internal HttpRequestBuilder Build(HttpRemoteOptions httpRemoteOptions,
        Channel<FileTransferProgress> progressChannel)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRemoteOptions);
        ArgumentNullException.ThrowIfNull(progressChannel);

        // 检查文件拓展名和大小合法性
        EnsureLegalData(FilePath, AllowedFileExtensions, MaxFileSizeInBytes);

        // 初始化 HttpRequestBuilder 实例
        var httpRequestBuilder = HttpRequestBuilder.Create(HttpMethod, RequestUri).SetMultipartContent(builder =>
            builder.AddFileWithProgressAsStream(FilePath, progressChannel, Name, FileName, ContentType));

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

    /// <summary>
    ///     检查文件拓展名和大小合法性
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="allowedFileExtensions">允许的文件拓展名</param>
    /// <param name="maxFileSizeInBytes">允许的文件大小。以字节为单位</param>
    internal static void EnsureLegalData(string filePath, string[]? allowedFileExtensions, long? maxFileSizeInBytes)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // 空检查
        if (!allowedFileExtensions.IsNullOrEmpty())
        {
            FileUtility.ValidateExtension(filePath, allowedFileExtensions);
        }

        // 空检查
        if (maxFileSizeInBytes is not null)
        {
            FileUtility.ValidateSize(filePath, maxFileSizeInBytes.Value);
        }
    }
}