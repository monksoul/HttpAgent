// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     提供静态访问 <see cref="IHttpRemoteService" /> 服务的方式
/// </summary>
/// <remarks>支持服务的延迟初始化、配置更新以及资源释放。</remarks>
public static class HttpRemoteClient
{
    /// <inheritdoc cref="IServiceProvider" />
    internal static IServiceProvider? _serviceProvider;

    /// <summary>
    ///     延迟加载的 <see cref="IHttpRemoteService" /> 实例
    /// </summary>
    internal static Lazy<IHttpRemoteService> _httpRemoteService;

    /// <summary>
    ///     并发锁对象
    /// </summary>
    internal static readonly SemaphoreSlim _initializationLock = new(1, 1);

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
    static HttpRemoteClient() => _httpRemoteService = new Lazy<IHttpRemoteService>(CreateService);

    /// <summary>
    ///     获取当前配置下的 <see cref="IHttpRemoteService" /> 实例
    /// </summary>
    public static IHttpRemoteService Service
    {
        get
        {
            // 释放检查
            ObjectDisposedException.ThrowIf(_isDisposed, typeof(HttpRemoteClient));

            return _httpRemoteService.Value;
        }
    }

    /// <summary>
    ///     自定义服务注册逻辑
    /// </summary>
    public static void Configure(Action<IServiceCollection> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        // 初始化锁
        _initializationLock.Wait();

        try
        {
            // 释放检查
            ObjectDisposedException.ThrowIf(_isDisposed, typeof(HttpRemoteClient));

            // 更新配置委托
            _configure = services =>
            {
                // 调用自定义配置委托
                configure(services);

                // 检查 HTTP 远程请求服务是否已注册，若未注册则自动完成注册
                if (services.All(u => u.ServiceType != typeof(IHttpRemoteService)))
                {
                    services.AddHttpRemote();
                }
            };

            // 重新初始化服务
            Reinitialize();
        }
        finally
        {
            // 释放锁
            _initializationLock.Release();
        }
    }

    /// <summary>
    ///     释放服务提供器及相关资源
    /// </summary>
    /// <remarks>通常在应用程序关闭或不再需要 HTTP 远程请求服务时调用。</remarks>
    public static void Dispose()
    {
        // 初始化锁
        _initializationLock.Wait();

        try
        {
            // 幂等性处理
            if (_isDisposed)
            {
                return;
            }

            // 重新初始化服务
            Reinitialize();

            // 标记为已释放状态
            _isDisposed = true;
        }
        finally
        {
            // 释放锁
            _initializationLock.Release();
        }
    }

    /// <summary>
    ///     创建 <see cref="IHttpRemoteService" /> 实例
    /// </summary>
    /// <returns>
    ///     <see cref="IHttpRemoteService" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal static IHttpRemoteService CreateService()
    {
        // 初始化锁
        _initializationLock.Wait();

        try
        {
            // 释放检查
            ObjectDisposedException.ThrowIf(_isDisposed, typeof(HttpRemoteClient));

            // 如果值已创建，直接返回
            if (_httpRemoteService.IsValueCreated)
            {
                return _httpRemoteService.Value;
            }

            try
            {
                // 初始化 ServiceCollection 实例
                var services = new ServiceCollection();

                // 调用自定义服务注册逻辑的委托
                _configure(services);

                // 构建服务提供器
                _serviceProvider = services.BuildServiceProvider();

                // 解析 IHttpRemoteService 实例并返回
                return _serviceProvider.GetRequiredService<IHttpRemoteService>();
            }
            catch (Exception ex)
            {
                // 重新初始化服务
                Reinitialize();

                throw new InvalidOperationException("Failed to initialize IHttpRemoteService.", ex);
            }
        }
        finally
        {
            // 释放锁
            _initializationLock.Release();
        }
    }

    /// <summary>
    ///     使用最新的 <see cref="Configure" /> 配置重新初始化服务
    /// </summary>
    internal static void Reinitialize()
    {
        // 释放检查
        ObjectDisposedException.ThrowIf(_isDisposed, typeof(HttpRemoteClient));

        // 释放旧资源
        ReleaseServiceProvider();

        // 重置延迟加载实例
        _httpRemoteService = new Lazy<IHttpRemoteService>(CreateService);
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