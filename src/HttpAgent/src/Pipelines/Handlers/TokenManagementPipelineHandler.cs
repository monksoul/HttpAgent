// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using Microsoft.Net.Http.Headers;

namespace HttpAgent;

/// <summary>
///     Access Token 自动管理管道处理器
/// </summary>
/// <param name="serviceProvider">
///     <see cref="IServiceProvider" />
/// </param>
/// <param name="accessTokenManager">
///     <see cref="HttpAccessTokenManager" />
/// </param>
internal sealed class TokenManagementPipelineHandler(
    IServiceProvider serviceProvider,
    HttpAccessTokenManager accessTokenManager) : IHttpRequestPipelineHandler
{
    /// <inheritdoc />
    public async Task<HttpResponseMessage?> HandleAsync(HttpRequestPipelineContext context,
        Func<Task<HttpResponseMessage?>> next)
    {
        // 获取当前 HttpRequestBuilder 实例
        var httpRequestBuilder = context.Builder;

        // 检查是否跳过框架的 Access Token 自动管理
        if (httpRequestBuilder.SuppressTokenManagement)
        {
            // 调用下一个处理器的委托
            return await next();
        }

        // 获取当前 HttpClient 实例的配置名称
        var httpClientName = httpRequestBuilder.HttpClientName;

        // 获取当前 HttpClient 实例的配置名称的配置选项
        var httpClientOptions = serviceProvider.GetService<IOptionsMonitor<HttpClientOptions>>()?.Get(httpClientName);

        // 检查是否配置了 Access Token 提供器
        if (httpClientOptions?.HttpAccessTokenProvider is not { } httpAccessTokenProvider)
        {
            // 调用下一个处理器的委托
            return await next();
        }

        // 初始化 HttpAccessTokenContext 实例
        var httpAccessTokenContext = new HttpAccessTokenContext(httpClientName, httpAccessTokenProvider);

        // 获取或刷新指定 HttpClient 实例的配置名称的 Access Token
        var httpAccessToken =
            await accessTokenManager.GetOrRefreshAsync(httpAccessTokenContext, context.CancellationToken);

        // 申请将 Access Token 添加到 HttpRequestBuilder 中
        ApplyAccessToken(httpRequestBuilder, httpAccessTokenProvider, httpAccessToken);

        // 调用下一个处理器的委托
        var httpResponseMessage = await next();

        // 检查是否需要强制刷新 Token 并重试（由提供器决定，默认 401）
        // ReSharper disable once InvertIf
        if (httpResponseMessage is not null &&
            httpAccessTokenProvider.ShouldRefreshToken(httpResponseMessage, context.CancellationToken))
        {
            // 释放前一个 HttpResponseMessage 实例
            httpResponseMessage.Dispose();

            // 强制刷新指定 HttpClient 实例的配置名称的 Access Token
            var newHttpAccessTokenToken =
                await accessTokenManager.ForceRefreshAsync(httpAccessTokenContext, context.CancellationToken);

            //  克隆新的 HttpRequestBuilder 实例
            var cloneHttpRequestBuilder = httpRequestBuilder.Clone();

            // 申请将 Access Token 添加到 HttpRequestBuilder 中
            ApplyAccessToken(cloneHttpRequestBuilder, httpAccessTokenProvider, newHttpAccessTokenToken);

            // 更新上下文
            context.Builder = cloneHttpRequestBuilder;

            // 调用下一个处理器的委托
            httpResponseMessage = await next();
        }

        return httpResponseMessage;
    }

    /// <summary>
    ///     申请将 Access Token 添加到 <see cref="HttpRequestBuilder" /> 中
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="httpAccessTokenProvider">
    ///     <see cref="IHttpAccessTokenProvider" />
    /// </param>
    /// <param name="httpAccessToken">
    ///     <see cref="HttpAccessToken" />
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    internal void ApplyAccessToken(HttpRequestBuilder httpRequestBuilder,
        IHttpAccessTokenProvider httpAccessTokenProvider, HttpAccessToken httpAccessToken)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRequestBuilder);
        ArgumentNullException.ThrowIfNull(httpAccessTokenProvider);
        ArgumentNullException.ThrowIfNull(httpAccessToken);

        // 获取或解析 IHttpAccessTokenConfigurator 实例
        // ReSharper disable once SuspiciousTypeConversion.Global
        var httpAccessTokenConfigurator = httpAccessTokenProvider as IHttpAccessTokenConfigurator ??
                                          serviceProvider.GetService<IHttpAccessTokenConfigurator>();

        // 空检查
        if (httpAccessTokenConfigurator is not null)
        {
            httpAccessTokenConfigurator.Configure(httpRequestBuilder, httpAccessToken);
        }
        // 检查是否配置了 HTTP 认证方案
        else if (httpAccessToken.Scheme is { } scheme)
        {
            httpRequestBuilder.AddAuthentication(scheme, httpAccessToken.Value);
        }
        else
        {
            // 无自定义配置时，默认将 Access Token 值作为 Authorization 请求头发送
            httpRequestBuilder.WithHeader(HeaderNames.Authorization, httpAccessToken.Value, replace: true);
        }
    }
}