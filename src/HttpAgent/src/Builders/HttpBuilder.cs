// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="HttpRequestMessage" /> 构建器
/// </summary>
/// <remarks>用于简化 <see cref="HttpRequestBuilder" /> 名称过长问题。</remarks>
public sealed class HttpBuilder
{
    /// <summary>
    ///     创建 <c>GET</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <remarks>用于简化 <see cref="HttpRequestBuilder" /> 名字过长。</remarks>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Get(string? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Get(requestUri, configure);

    /// <summary>
    ///     创建 <c>GET</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Get(Uri? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Get(requestUri, configure);

    /// <summary>
    ///     创建 <c>PUT</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Put(string? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Put(requestUri, configure);

    /// <summary>
    ///     创建 <c>PUT</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Put(Uri? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Put(requestUri, configure);

    /// <summary>
    ///     创建 <c>POST</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Post(string? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Post(requestUri, configure);

    /// <summary>
    ///     创建 <c>POST</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Post(Uri? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Post(requestUri, configure);

    /// <summary>
    ///     创建 <c>DELETE</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Delete(string? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Delete(requestUri, configure);

    /// <summary>
    ///     创建 <c>DELETE</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Delete(Uri? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Delete(requestUri, configure);

    /// <summary>
    ///     创建 <c>HEAD</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Head(string? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Head(requestUri, configure);

    /// <summary>
    ///     创建 <c>HEAD</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Head(Uri? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Head(requestUri, configure);

    /// <summary>
    ///     创建 <c>OPTIONS</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Options(string? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Options(requestUri, configure);

    /// <summary>
    ///     创建 <c>OPTIONS</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Options(Uri? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Options(requestUri, configure);

    /// <summary>
    ///     创建 <c>TRACE</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Trace(string? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Trace(requestUri, configure);

    /// <summary>
    ///     创建 <c>TRACE</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Trace(Uri? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Trace(requestUri, configure);

    /// <summary>
    ///     创建 <c>PATCH</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Patch(string? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Patch(requestUri, configure);

    /// <summary>
    ///     创建 <c>PATCH</c> 请求的 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Patch(Uri? requestUri, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.Patch(requestUri, configure);

    /// <summary>
    ///     创建 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Create(HttpMethod httpMethod, Uri? requestUri) =>
        HttpRequestBuilder.Create(httpMethod, requestUri);

    /// <summary>
    ///     创建 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Create(HttpMethod httpMethod, string? requestUri) =>
        HttpRequestBuilder.Create(httpMethod, requestUri);

    /// <summary>
    ///     创建 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Create(string httpMethod, string? requestUri) =>
        HttpRequestBuilder.Create(httpMethod, requestUri);

    /// <summary>
    ///     创建 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Create(string httpMethod, Uri? requestUri) =>
        HttpRequestBuilder.Create(httpMethod, requestUri);

    /// <summary>
    ///     创建 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Create(HttpMethod httpMethod, Uri? requestUri,
        Action<HttpRequestBuilder>? configure) => HttpRequestBuilder.Create(httpMethod, requestUri, configure);

    /// <summary>
    ///     创建 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Create(HttpMethod httpMethod, string? requestUri,
        Action<HttpRequestBuilder>? configure) => HttpRequestBuilder.Create(httpMethod, requestUri, configure);

    /// <summary>
    ///     创建 <see cref="HttpRequestBuilder" /> 构建器
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    public static HttpRequestBuilder Create(string httpMethod, string? requestUri,
        Action<HttpRequestBuilder>? configure) => HttpRequestBuilder.Create(httpMethod, requestUri, configure);

    /// <summary>
    ///     创建 <see cref="HttpFileDownloadBuilder" /> 构建器
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="destinationPath">文件保存的目标路径</param>
    /// <param name="onProgressChanged">用于传输进度发生变化时执行的委托</param>
    /// <param name="fileExistsBehavior">
    ///     <see cref="FileExistsBehavior" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpFileDownloadBuilder" />
    /// </returns>
    public static HttpFileDownloadBuilder DownloadFile(HttpMethod httpMethod, Uri? requestUri, string? destinationPath,
        Func<FileTransferProgress, Task>? onProgressChanged = null,
        FileExistsBehavior fileExistsBehavior = FileExistsBehavior.CreateNew,
        Action<HttpFileDownloadBuilder>? configure = null) => HttpRequestBuilder.DownloadFile(httpMethod, requestUri,
        destinationPath, onProgressChanged, fileExistsBehavior, configure);

    /// <summary>
    ///     创建 <see cref="HttpFileDownloadBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="destinationPath">文件保存的目标路径</param>
    /// <param name="onProgressChanged">用于传输进度发生变化时执行的委托</param>
    /// <param name="fileExistsBehavior">
    ///     <see cref="FileExistsBehavior" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpFileDownloadBuilder" />
    /// </returns>
    public static HttpFileDownloadBuilder DownloadFile(Uri? requestUri, string? destinationPath,
        Func<FileTransferProgress, Task>? onProgressChanged = null,
        FileExistsBehavior fileExistsBehavior = FileExistsBehavior.CreateNew,
        Action<HttpFileDownloadBuilder>? configure = null) => HttpRequestBuilder.DownloadFile(requestUri,
        destinationPath, onProgressChanged, fileExistsBehavior, configure);

    /// <summary>
    ///     创建 <see cref="HttpFileDownloadBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="destinationPath">文件保存的目标路径</param>
    /// <param name="onProgressChanged">用于传输进度发生变化时执行的委托</param>
    /// <param name="fileExistsBehavior">
    ///     <see cref="FileExistsBehavior" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpFileDownloadBuilder" />
    /// </returns>
    public static HttpFileDownloadBuilder DownloadFile(string? requestUri, string? destinationPath,
        Func<FileTransferProgress, Task>? onProgressChanged = null,
        FileExistsBehavior fileExistsBehavior = FileExistsBehavior.CreateNew,
        Action<HttpFileDownloadBuilder>? configure = null) => HttpRequestBuilder.DownloadFile(requestUri,
        destinationPath, onProgressChanged, fileExistsBehavior, configure);

    /// <summary>
    ///     创建 <see cref="HttpFileUploadBuilder" /> 构建器
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="filePath">文件路径</param>
    /// <param name="name">表单名称；默认值为 <c>file</c>。</param>
    /// <param name="onProgressChanged">用于传输进度发生变化时执行的委托</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpFileUploadBuilder" />
    /// </returns>
    public static HttpFileUploadBuilder UploadFile(HttpMethod httpMethod, Uri? requestUri, string filePath,
        string name = "file", Func<FileTransferProgress, Task>? onProgressChanged = null, string? fileName = null,
        Action<HttpFileUploadBuilder>? configure = null) => HttpRequestBuilder.UploadFile(httpMethod, requestUri,
        filePath, name, onProgressChanged, fileName, configure);

    /// <summary>
    ///     创建 <see cref="HttpFileUploadBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="filePath">文件路径</param>
    /// <param name="name">表单名称；默认值为 <c>file</c>。</param>
    /// <param name="onProgressChanged">用于传输进度发生变化时执行的委托</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpFileUploadBuilder" />
    /// </returns>
    public static HttpFileUploadBuilder UploadFile(Uri? requestUri, string filePath, string name = "file",
        Func<FileTransferProgress, Task>? onProgressChanged = null, string? fileName = null,
        Action<HttpFileUploadBuilder>? configure = null) =>
        HttpRequestBuilder.UploadFile(requestUri, filePath, name, onProgressChanged, fileName, configure);

    /// <summary>
    ///     创建 <see cref="HttpFileUploadBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="filePath">文件路径</param>
    /// <param name="name">表单名称；默认值为 <c>file</c>。</param>
    /// <param name="onProgressChanged">用于传输进度发生变化时执行的委托</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpFileUploadBuilder" />
    /// </returns>
    public static HttpFileUploadBuilder UploadFile(string? requestUri, string filePath, string name = "file",
        Func<FileTransferProgress, Task>? onProgressChanged = null, string? fileName = null,
        Action<HttpFileUploadBuilder>? configure = null) =>
        HttpRequestBuilder.UploadFile(requestUri, filePath, name, onProgressChanged, fileName, configure);

    /// <summary>
    ///     创建 <see cref="HttpServerSentEventsBuilder" /> 构建器
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="onMessage">用于在从事件源接收到数据时的操作</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpServerSentEventsBuilder" />
    /// </returns>
    public static HttpServerSentEventsBuilder ServerSentEvents(HttpMethod httpMethod, Uri? requestUri,
        Func<ServerSentEventsData, CancellationToken, Task> onMessage,
        Action<HttpServerSentEventsBuilder>? configure = null) =>
        HttpRequestBuilder.ServerSentEvents(httpMethod, requestUri, onMessage, configure);

    /// <summary>
    ///     创建 <see cref="HttpServerSentEventsBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="onMessage">用于在从事件源接收到数据时的操作</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpServerSentEventsBuilder" />
    /// </returns>
    public static HttpServerSentEventsBuilder ServerSentEvents(Uri? requestUri,
        Func<ServerSentEventsData, CancellationToken, Task> onMessage,
        Action<HttpServerSentEventsBuilder>? configure = null) =>
        HttpRequestBuilder.ServerSentEvents(requestUri, onMessage, configure);

    /// <summary>
    ///     创建 <see cref="HttpServerSentEventsBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="onMessage">用于在从事件源接收到数据时的操作</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpServerSentEventsBuilder" />
    /// </returns>
    public static HttpServerSentEventsBuilder ServerSentEvents(string? requestUri,
        Func<ServerSentEventsData, CancellationToken, Task> onMessage,
        Action<HttpServerSentEventsBuilder>? configure = null) =>
        HttpRequestBuilder.ServerSentEvents(requestUri, onMessage, configure);

    /// <summary>
    ///     创建 <see cref="HttpStressTestHarnessBuilder" /> 构建器
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="numberOfRequests">并发请求数量，默认值为：100。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpStressTestHarnessBuilder" />
    /// </returns>
    public static HttpStressTestHarnessBuilder StressTestHarness(HttpMethod httpMethod, Uri? requestUri,
        int numberOfRequests = 100, Action<HttpStressTestHarnessBuilder>? configure = null) =>
        HttpRequestBuilder.StressTestHarness(httpMethod, requestUri, numberOfRequests, configure);

    /// <summary>
    ///     创建 <see cref="HttpStressTestHarnessBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="numberOfRequests">并发请求数量，默认值为：100。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpStressTestHarnessBuilder" />
    /// </returns>
    public static HttpStressTestHarnessBuilder StressTestHarness(Uri? requestUri, int numberOfRequests = 100,
        Action<HttpStressTestHarnessBuilder>? configure = null) =>
        HttpRequestBuilder.StressTestHarness(requestUri, numberOfRequests, configure);

    /// <summary>
    ///     创建 <see cref="HttpStressTestHarnessBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="numberOfRequests">并发请求数量，默认值为：100。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpStressTestHarnessBuilder" />
    /// </returns>
    public static HttpStressTestHarnessBuilder StressTestHarness(string? requestUri, int numberOfRequests = 100,
        Action<HttpStressTestHarnessBuilder>? configure = null) =>
        HttpRequestBuilder.StressTestHarness(requestUri, numberOfRequests, configure);

    /// <summary>
    ///     创建 <see cref="HttpLongPollingBuilder" /> 构建器
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    /// <param name="onDataReceived">用于接收服务器返回 <c>200~299</c> 状态码的数据的操作</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpLongPollingBuilder" />
    /// </returns>
    public static HttpLongPollingBuilder LongPolling(HttpMethod httpMethod, Uri? requestUri,
        Func<HttpResponseMessage, CancellationToken, Task> onDataReceived,
        Action<HttpLongPollingBuilder>? configure = null) =>
        HttpRequestBuilder.LongPolling(httpMethod, requestUri, onDataReceived, configure);

    /// <summary>
    ///     创建 <see cref="HttpLongPollingBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="onDataReceived">用于接收服务器返回 <c>200~299</c> 状态码的数据的操作</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpLongPollingBuilder" />
    /// </returns>
    public static HttpLongPollingBuilder LongPolling(Uri? requestUri,
        Func<HttpResponseMessage, CancellationToken, Task> onDataReceived,
        Action<HttpLongPollingBuilder>? configure = null) =>
        HttpRequestBuilder.LongPolling(requestUri, onDataReceived, configure);

    /// <summary>
    ///     创建 <see cref="HttpLongPollingBuilder" /> 构建器
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="onDataReceived">用于接收服务器返回 <c>200~299</c> 状态码的数据的操作</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpLongPollingBuilder" />
    /// </returns>
    public static HttpLongPollingBuilder LongPolling(string? requestUri,
        Func<HttpResponseMessage, CancellationToken, Task> onDataReceived,
        Action<HttpLongPollingBuilder>? configure = null) =>
        HttpRequestBuilder.LongPolling(requestUri, onDataReceived, configure);

    /// <summary>
    ///     创建 <see cref="HttpDeclarativeBuilder" /> 构建器
    /// </summary>
    /// <param name="method">被调用方法</param>
    /// <param name="args">被调用方法的参数值数组</param>
    /// <returns>
    ///     <see cref="HttpDeclarativeBuilder" />
    /// </returns>
    public static HttpDeclarativeBuilder Declarative(MethodInfo method, object?[] args) =>
        HttpRequestBuilder.Declarative(method, args);

    /// <summary>
    ///     从 JSON 中创建 <see cref="HttpRequestBuilder" /> 实例
    /// </summary>
    /// <param name="json">JSON 字符串</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static HttpRequestBuilder FromJson(string json, Action<HttpRequestBuilder>? configure = null) =>
        HttpRequestBuilder.FromJson(json, configure);
}