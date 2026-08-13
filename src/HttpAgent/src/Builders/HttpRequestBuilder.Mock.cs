// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="HttpRequestMessage" /> 构建器
/// </summary>
public sealed partial class HttpRequestBuilder
{
    /// <summary>
    ///     设置模拟的 <see cref="HttpResponseMessage" />
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpRequestBuilder MockResponse(HttpResponseMessage httpResponseMessage)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpResponseMessage);

        // 释放旧的 MockedResponse 实例
        MockedResponse?.Dispose();

        MockedResponse = httpResponseMessage;
        MockedException = null;

        return this;
    }

    /// <summary>
    ///     设置模拟的 <see cref="HttpResponseMessage" />
    /// </summary>
    /// <param name="content">
    ///     <typeparamref name="T" />
    /// </param>
    /// <param name="statusCode">响应状态码</param>
    /// <param name="contentType">内容类型</param>
    /// <typeparam name="T">内容对象类型</typeparam>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpRequestBuilder MockResponse<T>(T content, HttpStatusCode statusCode = HttpStatusCode.OK,
        string? contentType = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(content);

        // 初始化 HttpResponseMessage 实例
        var httpResponseMessage = new HttpResponseMessage(statusCode);

        // 序列化内容对象并设置给 Content 属性
        httpResponseMessage.Content = new StringContent(content.ToJsonString(), Encoding.UTF8,
            contentType ?? MediaTypeNames.Application.Json);

        return MockResponse(httpResponseMessage);
    }

    /// <summary>
    ///     设置模拟的异常
    /// </summary>
    /// <param name="exception">
    ///     <see cref="Exception" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpRequestBuilder MockException(Exception exception)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(exception);

        // 释放旧的 MockedResponse 实例
        MockedResponse?.Dispose();

        MockedException = exception;
        MockedResponse = null;

        return this;
    }

    /// <summary>
    ///     清除所有模拟设置
    /// </summary>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public HttpRequestBuilder ClearMock()
    {
        // 释放旧的 MockedResponse 实例
        MockedResponse?.Dispose();

        MockedResponse = null;
        MockedException = null;

        return this;
    }

    /// <summary>
    ///     检查当前构建器是否配置了模拟响应或模拟异常
    /// </summary>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public bool IsMocked() => MockedResponse is not null || MockedException is not null;
}