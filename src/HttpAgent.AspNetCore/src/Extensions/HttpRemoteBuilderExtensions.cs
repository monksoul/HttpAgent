// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="IHttpRemoteBuilder" /> 扩展类
/// </summary>
public static class HttpRemoteBuilderExtensions
{
    /// <summary>
    ///     配置 <see cref="HttpContextForwardOptions" /> 实例
    /// </summary>
    /// <param name="remoteBuilder">
    ///     <see cref="IHttpRemoteBuilder" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="IHttpRemoteBuilder" />
    /// </returns>
    public static IHttpRemoteBuilder ConfigureForwardOptions(this IHttpRemoteBuilder remoteBuilder,
        Action<HttpContextForwardOptions> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        remoteBuilder.Services.Configure(configure);

        return remoteBuilder;
    }

    /// <summary>
    ///     配置 <see cref="HttpContextForwardOptions" /> 实例
    /// </summary>
    /// <param name="remoteBuilder">
    ///     <see cref="IHttpRemoteBuilder" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="IHttpRemoteBuilder" />
    /// </returns>
    public static IHttpRemoteBuilder ConfigureForwardOptions(this IHttpRemoteBuilder remoteBuilder,
        Action<HttpContextForwardOptions, IServiceProvider> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        remoteBuilder.Services.AddOptions<HttpContextForwardOptions>().Configure(configure);

        return remoteBuilder;
    }
}