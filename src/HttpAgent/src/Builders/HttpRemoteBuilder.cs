﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求构建器
/// </summary>
public sealed class HttpRemoteBuilder
{
    /// <summary>
    ///     <see cref="IHttpContentConverter" /> 集合
    /// </summary>
    internal IList<Func<IEnumerable<IHttpContentConverter>>>? _httpContentConverterProviders;

    /// <summary>
    ///     <see cref="IHttpContentProcessor" /> 集合
    /// </summary>
    internal IList<Func<IEnumerable<IHttpContentProcessor>>>? _httpContentProcessorProviders;

    /// <summary>
    ///     <see cref="IObjectContentConverterFactory" /> 实现类型
    /// </summary>
    internal Type? _objectContentConverterFactoryType;

    /// <summary>
    ///     默认请求内容类型
    /// </summary>
    public string? DefaultContentType { get; set; }

    /// <summary>
    ///     默认文件下载保存目录
    /// </summary>
    public string? DefaultFileDownloadDirectory { get; set; }

    /// <summary>
    ///     设置默认请求内容类型
    /// </summary>
    /// <param name="contentType">内容类型</param>
    /// <returns>
    ///     <see cref="HttpRemoteBuilder" />
    /// </returns>
    public HttpRemoteBuilder SetDefaultContentType(string contentType)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        // 解析内容类型字符串
        var mediaTypeHeaderValue = MediaTypeHeaderValue.Parse(contentType);

        DefaultContentType = mediaTypeHeaderValue.MediaType;

        return this;
    }

    /// <summary>
    ///     设置默认文件下载保存目录
    /// </summary>
    /// <param name="fileDownloadDirectory">文件下载保存目录</param>
    /// <returns>
    ///     <see cref="HttpRemoteBuilder" />
    /// </returns>
    public HttpRemoteBuilder SetDefaultFileDownloadDirectory(string fileDownloadDirectory)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(fileDownloadDirectory);

        DefaultFileDownloadDirectory = fileDownloadDirectory;

        return this;
    }

    /// <summary>
    ///     添加 <see cref="IHttpContentProcessor" /> 集合提供器
    /// </summary>
    /// <remarks>支持多次调用。</remarks>
    /// <param name="configure"><see cref="IHttpContentProcessor" /> 实例提供器</param>
    /// <returns>
    ///     <see cref="HttpRemoteBuilder" />
    /// </returns>
    public HttpRemoteBuilder AddHttpContentProcessors(Func<IEnumerable<IHttpContentProcessor>> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        _httpContentProcessorProviders ??= new List<Func<IEnumerable<IHttpContentProcessor>>>();

        _httpContentProcessorProviders.Add(configure);

        return this;
    }

    /// <summary>
    ///     添加 <see cref="IHttpContentProcessor" /> 集合提供器
    /// </summary>
    /// <remarks>支持多次调用。</remarks>
    /// <param name="configure"><see cref="IHttpContentProcessor" /> 实例提供器</param>
    /// <returns>
    ///     <see cref="HttpRemoteBuilder" />
    /// </returns>
    public HttpRemoteBuilder AddHttpContentConverters(Func<IEnumerable<IHttpContentConverter>> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        _httpContentConverterProviders ??= new List<Func<IEnumerable<IHttpContentConverter>>>();

        _httpContentConverterProviders.Add(configure);

        return this;
    }

    /// <summary>
    ///     添加自定义 <see cref="ObjectContentConverter{TResult}" /> 工厂
    /// </summary>
    /// <typeparam name="TFactory">
    ///     <see cref="IObjectContentConverterFactory" />
    /// </typeparam>
    /// <returns>
    ///     <see cref="HttpRemoteBuilder" />
    /// </returns>
    public HttpRemoteBuilder UseObjectContentConverterFactory<TFactory>()
        where TFactory : IObjectContentConverterFactory =>
        UseObjectContentConverterFactory(typeof(TFactory));

    /// <summary>
    ///     添加自定义 <see cref="ObjectContentConverter{TResult}" /> 工厂
    /// </summary>
    /// <param name="factoryType">
    ///     <see cref="IObjectContentConverterFactory" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpRemoteBuilder" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public HttpRemoteBuilder UseObjectContentConverterFactory(Type factoryType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(factoryType);

        // 检查类型是否实现了 IObjectContentConverterFactory 接口
        if (!typeof(IObjectContentConverterFactory).IsAssignableFrom(factoryType))
        {
            throw new InvalidOperationException(
                $"`{factoryType}` type is not assignable from `{typeof(IObjectContentConverterFactory)}`.");
        }

        _objectContentConverterFactoryType = factoryType;

        return this;
    }

    /// <summary>
    ///     构建模块服务
    /// </summary>
    /// <param name="services">
    ///     <see cref="IServiceCollection" />
    /// </param>
    internal void Build(IServiceCollection services)
    {
        // 检查默认请求内容类型合法性
        EnsureLegalData(DefaultContentType);

        // 注册 CodePagesEncodingProvider，使得程序能够识别并使用 Windows 代码页中的各种编码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // 注册日志服务
        services.AddLogging();

        // 注册分析中间件服务
        services.TryAddTransient<ProfilerDelegatingHandler>();

        // 注册默认 HttpClient 客户端
        if (services.All(u => u.ServiceType != typeof(IHttpClientFactory)))
        {
            services.AddHttpClient();
        }

        // 注册 HttpContent 内容处理器工厂
        services.TryAddSingleton<IHttpContentProcessorFactory>(_ =>
            new HttpContentProcessorFactory(_httpContentProcessorProviders?.SelectMany(u => u.Invoke()).ToArray()));

        // 注册 HttpContent 内容转换器工厂
        services.TryAddSingleton<IHttpContentConverterFactory>(provider =>
            new HttpContentConverterFactory(provider,
                _httpContentConverterProviders?.SelectMany(u => u.Invoke()).ToArray()));

        // 注册 HTTP 远程请求服务
        services.TryAddSingleton<IHttpRemoteService>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteService>(provider,
                new HttpRemoteOptions
                {
                    DefaultContentType = DefaultContentType ?? Constants.DEFAULT_CONTENT_TYPE,
                    DefaultFileDownloadDirectory = DefaultFileDownloadDirectory
                }));

        // 注册或替换 IObjectContentConverterFactory 工厂
        if (_objectContentConverterFactoryType is not null)
        {
            services.Replace(ServiceDescriptor.Singleton(typeof(IObjectContentConverterFactory),
                _objectContentConverterFactoryType));
        }
    }

    /// <summary>
    ///     检查默认请求内容类型合法性
    /// </summary>
    /// <param name="defaultContentType">内容类型</param>
    /// <exception cref="ArgumentException"></exception>
    internal static void EnsureLegalData(string? defaultContentType)
    {
        // 跳过空检查
        if (defaultContentType is null)
        {
            return;
        }

        // 尝试解析内容类型字符串
        if (!MediaTypeHeaderValue.TryParse(defaultContentType, out _))
        {
            throw new ArgumentException("The provided default content type is not valid.", nameof(defaultContentType));
        }
    }
}