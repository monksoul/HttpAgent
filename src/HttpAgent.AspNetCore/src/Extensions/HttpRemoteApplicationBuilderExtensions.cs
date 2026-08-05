// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Microsoft.AspNetCore.Builder;

/// <summary>
///     <see cref="IApplicationBuilder" /> 扩展类
/// </summary>
public static class HttpRemoteApplicationBuilderExtensions
{
    /// <summary>
    ///     启用请求正文缓存
    /// </summary>
    /// <remarks>
    ///     <para>支持 <c>HttpRequest.Body</c> 重复读取。</para>
    ///     <para>参考文献：https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/use-http-context?view=aspnetcore-8.0#enable-request-body-buffering</para>
    /// </remarks>
    /// <param name="app">
    ///     <see cref="IApplicationBuilder" />
    /// </param>
    /// <returns>
    ///     <see cref="IApplicationBuilder" />
    /// </returns>
    public static IApplicationBuilder UseEnableBuffering(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            context.Request.EnableBuffering();
            context.Request.Body.Position = 0;

            await next.Invoke();
        });

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
    /// <param name="app">
    ///     <see cref="IApplicationBuilder" />
    /// </param>
    /// <returns>
    ///     <see cref="IApplicationBuilder" />
    /// </returns>
    public static IApplicationBuilder UseHttpRemoteClient(this IApplicationBuilder app)
    {
        HttpRemoteClient.SetServiceProvider(app.ApplicationServices);

        return app;
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
    /// <param name="app">
    ///     <see cref="WebApplication" />
    /// </param>
    /// <returns>
    ///     <see cref="WebApplication" />
    /// </returns>
    public static WebApplication UseHttpRemoteClient(this WebApplication app)
    {
        HttpRemoteClient.SetServiceProvider(app.Services);

        return app;
    }
}