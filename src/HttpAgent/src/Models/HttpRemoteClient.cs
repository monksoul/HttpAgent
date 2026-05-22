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
    /// <summary>
    ///     标记服务是否已释放
    /// </summary>
    internal static volatile bool _isDisposed;

    /// <summary>
    ///     当前 <see cref="IHttpRemoteService" /> 实例
    /// </summary>
    internal static volatile IHttpRemoteService? _serviceInstance;

    /// <inheritdoc cref="IServiceProvider" />
    internal static IServiceProvider? _serviceProvider;

    /// <summary>
    ///     并发锁对象
    /// </summary>
    internal static readonly object _lock = new();

    /// <summary>
    ///     自定义服务注册逻辑的委托
    /// </summary>
    internal static Action<IServiceCollection> _configure = services => services.AddHttpRemote();

    /// <summary>
    ///     获取当前配置下的 <see cref="IHttpRemoteService" /> 实例
    /// </summary>
    public static IHttpRemoteService Service
    {
        get
        {
            // 释放检查
            ObjectDisposedException.ThrowIf(_isDisposed, typeof(HttpRemoteClient));

            // 双重检查锁定
            // ReSharper disable once InvertIf
            if (_serviceInstance is null)
            {
                lock (_lock)
                {
                    // 释放检查
                    ObjectDisposedException.ThrowIf(_isDisposed, typeof(HttpRemoteClient));

                    // 空检查
                    // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
                    if (_serviceInstance is null)
                    {
                        _serviceInstance = CreateService();
                    }
                }
            }

            return _serviceInstance;
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
            ObjectDisposedException.ThrowIf(_isDisposed, typeof(HttpRemoteClient));

            // 更新配置委托
            var previousConfigure = _configure;
            _configure = services =>
            {
                // 调用历史配置委托
                previousConfigure(services);

                // 调用当前自定义配置委托
                configure(services);

                // 检查 HTTP 远程请求服务是否已注册，若未注册则自动完成注册 
                // ReSharper disable once SimplifyLinqExpressionUseAll
                if (!services.Any(u => u.ServiceType == typeof(IHttpRemoteService)))
                {
                    services.AddHttpRemote();
                }
            };

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

            // 清理资源并重置实例
            ReleaseServiceProvider();
            _serviceInstance = null;

            // 标记为已释放状态
            _isDisposed = true;
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
        try
        {
            // 初始化 ServiceCollection 实例
            var services = new ServiceCollection();

            // 调用自定义服务注册逻辑的委托
            _configure(services);

            // 构建服务提供器
            var provider = services.BuildServiceProvider();
            _serviceProvider = provider;

            // 解析并返回
            return provider.GetRequiredService<IHttpRemoteService>();
        }
        catch (Exception ex)
        {
            // 清理资源
            ReleaseServiceProvider();

            throw new InvalidOperationException("Failed to initialize IHttpRemoteService.", ex);
        }
    }

    /// <summary>
    ///     使用最新的配置重新初始化服务
    /// </summary>
    internal static void Reinitialize()
    {
        // 释放旧资源
        ReleaseServiceProvider();
        _serviceInstance = null;
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