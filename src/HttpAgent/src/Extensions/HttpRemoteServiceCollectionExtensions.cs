// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     HTTP 远程请求模块 <see cref="IServiceCollection" /> 扩展类
/// </summary>
public static class HttpRemoteServiceCollectionExtensions
{
    /// <summary>
    ///     添加 HTTP 远程请求服务
    /// </summary>
    /// <param name="services">
    ///     <see cref="IServiceCollection" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="IHttpRemoteBuilder" />
    /// </returns>
    public static IHttpRemoteBuilder AddHttpRemote(this IServiceCollection services
        , Action<HttpRemoteBuilder>? configure = null)
    {
        // 初始化 HTTP 远程请求构建器
        var httpRemoteBuilder = new HttpRemoteBuilder();

        // 调用自定义配置委托
        configure?.Invoke(httpRemoteBuilder);

        return services.AddHttpRemote(httpRemoteBuilder);
    }

    /// <summary>
    ///     添加 HTTP 远程请求服务
    /// </summary>
    /// <param name="services">
    ///     <see cref="IServiceCollection" />
    /// </param>
    /// <param name="httpRemoteBuilder">
    ///     <see cref="HttpRemoteBuilder" />
    /// </param>
    /// <returns>
    ///     <see cref="IHttpRemoteBuilder" />
    /// </returns>
    public static IHttpRemoteBuilder AddHttpRemote(this IServiceCollection services,
        HttpRemoteBuilder httpRemoteBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRemoteBuilder);

        // 构建模块服务
        httpRemoteBuilder.Build(services);

        return new DefaultHttpRemoteBuilder(services);
    }

    /// <summary>
    ///     将应用程序的主 <see cref="IServiceProvider" /> 注入到 <see cref="HttpRemoteClient" />，使其优先使用该容器解析
    ///     <see cref="IHttpRemoteService" /> 服务
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         调用此方法后，<see cref="HttpRemoteClient.Service" /> 将从外部容器获取服务实例，而非自行构建独立 DI 容器。注入的容器生命周期由调用方管理，
    ///         <see cref="HttpRemoteClient.Dispose" /> 不会释放它。
    ///     </para>
    ///     <para>注意：请确保使用的是根容器（Root ServiceProvider），使用作用域容器可能导致对象生命周期异常。</para>
    /// </remarks>
    /// <param name="serviceProvider">应用程序的根服务提供器，必须已完成 <see cref="IHttpRemoteService" /> 的注册。</param>
    /// <returns>
    ///     <see cref="IServiceProvider" />
    /// </returns>
    public static IServiceProvider UseHttpRemoteClient(this IServiceProvider serviceProvider)
    {
        HttpRemoteClient.SetServiceProvider(serviceProvider);

        return serviceProvider;
    }
}