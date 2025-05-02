// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     提供静态访问 <see cref="IHttpRemoteService" /> 服务的方式
/// </summary>
/// <remarks>支持服务的延迟初始化、配置更新以及资源释放。</remarks>
#pragma warning disable CA1513
public static class HttpRemoteClient
{
    /// <inheritdoc cref="IServiceProvider" />
    internal static IServiceProvider? _serviceProvider;

    /// <summary>
    ///     延迟加载的 <see cref="IHttpRemoteService" /> 实例
    /// </summary>
    internal static Lazy<IHttpRemoteService> _lazyService;

    /// <summary>
    ///     并发锁对象
    /// </summary>
    internal static readonly object _lock = new();

    /// <summary>
    ///     标记服务是否已释放
    /// </summary>
    internal static bool _isDisposed;

    /// <summary>
    ///     自定义服务注册逻辑的委托
    /// </summary>
    internal static Action<IServiceCollection> _configure = services => services.AddHttpRemote();

    /// <summary>
    ///     <inheritdoc cref="HttpRemoteClient" />
    /// </summary>
    static HttpRemoteClient() => _lazyService = new Lazy<IHttpRemoteService>(CreateService);

    /// <summary>
    ///     获取当前配置下的 <see cref="IHttpRemoteService" /> 实例
    /// </summary>
    /// <exception cref="ObjectDisposedException"></exception>
    public static IHttpRemoteService Service
    {
        get
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(HttpRemoteClient));
            }

            return _lazyService.Value;
        }
    }

    /// <summary>
    ///     自定义服务注册逻辑
    /// </summary>
    public static void Configure(Action<IServiceCollection> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        lock (_lock)
        {
            _configure = configure;

            // 重新初始化服务
            Reinitialize();
        }
    }

    /// <summary>
    ///     释放服务提供器及相关资源
    /// </summary>
    /// <remarks>通常在应用程序关闭或不再需要 HTTP 远程请求服务时调用。</remarks>
    public static void Dispose()
    {
        lock (_lock)
        {
            if (_isDisposed)
            {
                return;
            }

            // 释放服务提供器
            ReleaseServiceProvider();
            _isDisposed = true;
        }
    }

    /// <summary>
    ///     创建 <see cref="IHttpRemoteService" /> 实例
    /// </summary>
    /// <returns>
    ///     <see cref="IHttpRemoteService" />
    /// </returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    internal static IHttpRemoteService CreateService()
    {
        lock (_lock)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(HttpRemoteClient));
            }

            // 如果值已创建，直接返回
            if (_lazyService.IsValueCreated)
            {
                return _lazyService.Value;
            }

            try
            {
                // 初始化 ServiceCollection 实例
                var services = new ServiceCollection();

                // 调用自定义服务注册逻辑的委托
                _configure(services);

                // 构建服务提供器
                _serviceProvider = services.BuildServiceProvider();

                // 解析 IHttpRemoteService 实例
                var service = _serviceProvider.GetRequiredService<IHttpRemoteService>();

                return service;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialize IHttpRemoteService.", ex);
            }
        }
    }

    /// <summary>
    ///     使用最新的 <see cref="Configure" /> 配置重新初始化服务
    /// </summary>
    /// <exception cref="ObjectDisposedException"></exception>
    internal static void Reinitialize()
    {
        lock (_lock)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(HttpRemoteClient));
            }

            // 释放当前的服务提供器
            ReleaseServiceProvider();

            // 重新创建延迟加载实例
            _lazyService = new Lazy<IHttpRemoteService>(CreateService, LazyThreadSafetyMode.ExecutionAndPublication);
        }
    }

    /// <summary>
    ///     释放服务提供器
    /// </summary>
    internal static void ReleaseServiceProvider()
    {
        // 如果服务提供器支持释放资源，则执行释放操作
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _serviceProvider = null;
    }
}
#pragma warning restore CA1513