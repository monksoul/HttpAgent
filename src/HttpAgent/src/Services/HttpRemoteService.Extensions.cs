// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <inheritdoc cref="IHttpRemoteService" />
/// </summary>
internal sealed partial class HttpRemoteService
{
    /// <inheritdoc />
    public FileTransferResult DownloadFile(string? requestUri, string? destinationPath,
        Func<FileTransferProgress, Task>? onProgressChanged = null,
        FileExistsBehavior fileExistsBehavior = FileExistsBehavior.CreateNew,
        Action<HttpFileDownloadBuilder>? configure = null, CancellationToken cancellationToken = default) =>
        Send(
            HttpRequestBuilder.DownloadFile(requestUri, destinationPath, onProgressChanged, fileExistsBehavior,
                configure), cancellationToken);

    /// <inheritdoc />
    public Task<FileTransferResult> DownloadFileAsync(string? requestUri, string? destinationPath,
        Func<FileTransferProgress, Task>? onProgressChanged = null,
        FileExistsBehavior fileExistsBehavior = FileExistsBehavior.CreateNew,
        Action<HttpFileDownloadBuilder>? configure = null, CancellationToken cancellationToken = default) =>
        SendAsync(
            HttpRequestBuilder.DownloadFile(requestUri, destinationPath, onProgressChanged, fileExistsBehavior,
                configure), cancellationToken);

    /// <inheritdoc />
    public FileTransferResult Send(HttpFileDownloadBuilder httpFileDownloadBuilder,
        CancellationToken cancellationToken = default) =>
        new FileDownloadManager(this, httpFileDownloadBuilder).Start(cancellationToken);

    /// <inheritdoc />
    public Task<FileTransferResult> SendAsync(HttpFileDownloadBuilder httpFileDownloadBuilder,
        CancellationToken cancellationToken = default) =>
        new FileDownloadManager(this, httpFileDownloadBuilder).StartAsync(cancellationToken);

    /// <inheritdoc />
    public HttpResponseMessage? UploadFile(string? requestUri, string filePath, string name = "file",
        Func<FileTransferProgress, Task>? onProgressChanged = null, string? fileName = null,
        Action<HttpFileUploadBuilder>? configure = null, CancellationToken cancellationToken = default) =>
        Send(HttpRequestBuilder.UploadFile(requestUri, filePath, name, onProgressChanged, fileName, configure),
            cancellationToken);

    /// <inheritdoc />
    public Task<HttpResponseMessage?> UploadFileAsync(string? requestUri, string filePath, string name = "file",
        Func<FileTransferProgress, Task>? onProgressChanged = null, string? fileName = null,
        Action<HttpFileUploadBuilder>? configure = null, CancellationToken cancellationToken = default) =>
        SendAsync(HttpRequestBuilder.UploadFile(requestUri, filePath, name, onProgressChanged, fileName, configure),
            cancellationToken);

    /// <inheritdoc />
    public HttpResponseMessage? Send(HttpFileUploadBuilder httpFileUploadBuilder,
        CancellationToken cancellationToken = default) =>
        new FileUploadManager(this, httpFileUploadBuilder).Start(cancellationToken);

    /// <inheritdoc />
    public Task<HttpResponseMessage?> SendAsync(HttpFileUploadBuilder httpFileUploadBuilder,
        CancellationToken cancellationToken = default) =>
        new FileUploadManager(this, httpFileUploadBuilder).StartAsync(cancellationToken);

    /// <inheritdoc />
    public void ServerSentEvents(string? requestUri, Func<ServerSentEventsData, CancellationToken, Task> onMessage,
        Action<HttpServerSentEventsBuilder>? configure = null, CancellationToken cancellationToken = default) =>
        Send(HttpRequestBuilder.ServerSentEvents(requestUri, onMessage, configure), cancellationToken);

    /// <inheritdoc />
    public Task ServerSentEventsAsync(string? requestUri, Func<ServerSentEventsData, CancellationToken, Task> onMessage,
        Action<HttpServerSentEventsBuilder>? configure = null, CancellationToken cancellationToken = default) =>
        SendAsync(HttpRequestBuilder.ServerSentEvents(requestUri, onMessage, configure), cancellationToken);

    /// <inheritdoc />
    public void Send(HttpServerSentEventsBuilder httpServerSentEventsBuilder,
        CancellationToken cancellationToken = default) =>
        new ServerSentEventsManager(this, httpServerSentEventsBuilder).Start(cancellationToken);

    /// <inheritdoc />
    public Task SendAsync(HttpServerSentEventsBuilder httpServerSentEventsBuilder,
        CancellationToken cancellationToken = default) =>
        new ServerSentEventsManager(this, httpServerSentEventsBuilder).StartAsync(cancellationToken);

    /// <inheritdoc />
    public StressTestHarnessResult StressTestHarness(string? requestUri, int numberOfRequests = 100,
        Action<HttpStressTestHarnessBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default) =>
        Send(HttpRequestBuilder.StressTestHarness(requestUri, numberOfRequests, configure), completionOption,
            cancellationToken);

    /// <inheritdoc />
    public Task<StressTestHarnessResult> StressTestHarnessAsync(string? requestUri, int numberOfRequests = 100,
        Action<HttpStressTestHarnessBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default) =>
        SendAsync(HttpRequestBuilder.StressTestHarness(requestUri, numberOfRequests, configure), completionOption,
            cancellationToken);

    /// <inheritdoc />
    public StressTestHarnessResult Send(HttpStressTestHarnessBuilder httpStressTestHarnessBuilder,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default) =>
        new StressTestHarnessManager(this, httpStressTestHarnessBuilder).Start(completionOption,
            cancellationToken);

    /// <inheritdoc />
    public Task<StressTestHarnessResult> SendAsync(HttpStressTestHarnessBuilder httpStressTestHarnessBuilder,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default) =>
        new StressTestHarnessManager(this, httpStressTestHarnessBuilder).StartAsync(completionOption,
            cancellationToken);

    /// <inheritdoc />
    public void LongPolling(string? requestUri, Func<HttpResponseMessage, CancellationToken, Task> onDataReceived,
        Action<HttpLongPollingBuilder>? configure = null, CancellationToken cancellationToken = default) =>
        Send(HttpRequestBuilder.LongPolling(requestUri, onDataReceived, configure), cancellationToken);

    /// <inheritdoc />
    public Task LongPollingAsync(string? requestUri, Func<HttpResponseMessage, CancellationToken, Task> onDataReceived,
        Action<HttpLongPollingBuilder>? configure = null, CancellationToken cancellationToken = default) =>
        SendAsync(HttpRequestBuilder.LongPolling(requestUri, onDataReceived, configure), cancellationToken);

    /// <inheritdoc />
    public void Send(HttpLongPollingBuilder httpLongPollingBuilder, CancellationToken cancellationToken = default) =>
        new LongPollingManager(this, httpLongPollingBuilder).Start(cancellationToken);

    /// <inheritdoc />
    public Task SendAsync(HttpLongPollingBuilder httpLongPollingBuilder,
        CancellationToken cancellationToken = default) =>
        new LongPollingManager(this, httpLongPollingBuilder).StartAsync(cancellationToken);

    /// <inheritdoc />
    public object? Declarative(MethodInfo method, object[] args) =>
        SendAs(HttpRequestBuilder.Declarative(method, args));

    /// <inheritdoc />
    public Task<T?> DeclarativeAsync<T>(MethodInfo method, object[] args) =>
        SendAsAsync<T>(HttpRequestBuilder.Declarative(method, args));

    /// <inheritdoc />
    public object? SendAs(HttpDeclarativeBuilder httpDeclarativeBuilder) =>
        new DeclarativeManager(this, httpDeclarativeBuilder).Start();

    /// <inheritdoc />
    public Task<T?> SendAsAsync<T>(HttpDeclarativeBuilder httpDeclarativeBuilder) =>
        new DeclarativeManager(this, httpDeclarativeBuilder).StartAsync<T>();
}