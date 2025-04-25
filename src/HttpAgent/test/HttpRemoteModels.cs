// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public enum EnumModel
{
    None, Default
}

public class CustomStringContentProcessor : StringContentProcessor;

public class CustomStringContentProcessor2 : StringContentProcessor;

public class CustomStringContentConverter : StringContentConverter;

public class CustomByteArrayContentConverter : ByteArrayContentConverter;

public class ObjectModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class ObjectModelSnakeCase
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
}

public class CustomObjectContentConverter<TResult> : ObjectContentConverter<TResult>;

public class CustomObjectContentConverter : ObjectContentConverter;

public class NotImplementObjectContentConverterFactory;

public class CustomObjectContentConverterFactory : IObjectContentConverterFactory
{
    /// <inheritdoc />
    public ObjectContentConverter<TResult> GetConverter<TResult>() => new CustomObjectContentConverter<TResult>();

    /// <inheritdoc />
    public ObjectContentConverter GetConverter(Type resultType) => new CustomObjectContentConverter();
}

public class MultipartModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class NotImplementRequestEventHandler;

public class CustomRequestEventHandler : IHttpRequestEventHandler
{
    public int counter;

    /// <inheritdoc />
    public void OnPreSendRequest(HttpRequestMessage httpRequestMessage) => counter++;

    /// <inheritdoc />
    public void OnPostReceiveResponse(HttpResponseMessage httpResponseMessage) => counter++;

    /// <inheritdoc />
    public void OnRequestFailed(Exception exception, HttpResponseMessage? httpResponseMessage) => counter++;
}

public class NotImplementFileDownloadEventHandler;

public class CustomFileTransferEventHandler : IHttpFileTransferEventHandler
{
    public int counter;
    public int counter2;

    /// <inheritdoc />
    public void OnTransferStarted() => counter++;

    /// <inheritdoc />
    public void OnTransferCompleted(long duration) => counter++;

    /// <inheritdoc />
    public void OnTransferFailed(Exception exception) => counter++;

    /// <inheritdoc />
    public Task OnProgressChangedAsync(FileTransferProgress fileTransferProgress)
    {
        counter2++;
        return Task.CompletedTask;
    }
}

public class CustomFileTransferEventHandler2 : IHttpFileTransferEventHandler, IHttpRequestEventHandler
{
    /// <inheritdoc />
    public void OnTransferStarted() => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnTransferCompleted(long duration) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnTransferFailed(Exception exception) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task OnProgressChangedAsync(FileTransferProgress fileTransferProgress) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public void OnPreSendRequest(HttpRequestMessage httpRequestMessage) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnPostReceiveResponse(HttpResponseMessage httpResponseMessage) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnRequestFailed(Exception exception, HttpResponseMessage? httpResponseMessage) =>
        throw new NotImplementedException();
}

public class NotImplementServerSentEventsEventHandler;

public class CustomServerSentEventsEventHandler : IHttpServerSentEventsEventHandler
{
    public int counter;

    /// <inheritdoc />
    public void OnOpen() => counter++;

    /// <inheritdoc />
    public Task OnMessageAsync(ServerSentEventsData serverSentEventsData, CancellationToken cancellationToken)
    {
        counter++;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void OnError(Exception exception) => counter++;
}

public class CustomServerSentEventsEventHandler2 : IHttpServerSentEventsEventHandler, IHttpRequestEventHandler
{
    /// <inheritdoc />
    public void OnPreSendRequest(HttpRequestMessage httpRequestMessage) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnPostReceiveResponse(HttpResponseMessage httpResponseMessage) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnRequestFailed(Exception exception, HttpResponseMessage? httpResponseMessage) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public void OnOpen() => throw new NotImplementedException();

    /// <inheritdoc />
    public Task OnMessageAsync(ServerSentEventsData serverSentEventsData, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public void OnError(Exception exception) => throw new NotImplementedException();
}

public class NotLongPollingEventHandler;

public class CustomLongPollingEventHandler : IHttpLongPollingEventHandler
{
    public int counter;

    /// <inheritdoc />
    public Task OnDataReceivedAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
    {
        counter++;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task OnErrorAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
    {
        counter++;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task OnEndOfStreamAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

public class CustomLongPollingEventHandler2 : IHttpLongPollingEventHandler, IHttpRequestEventHandler
{
    /// <inheritdoc />
    public Task OnDataReceivedAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task OnErrorAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task OnEndOfStreamAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public void OnPreSendRequest(HttpRequestMessage httpRequestMessage) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnPostReceiveResponse(HttpResponseMessage httpResponseMessage) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnRequestFailed(Exception exception, HttpResponseMessage? httpResponseMessage) =>
        throw new NotImplementedException();
}

public class MessagePackModel0
{
    public int Id { get; set; }

    public string? Name { get; set; }
}

[MessagePackObject]
public class MessagePackModel1
{
    [MessagePack.Key(0)] public int Id { get; set; }

    [MessagePack.Key(1)] public string? Name { get; set; }
}

public interface INonHttpTest;

public interface IHttpTest : IHttpDeclarative
{
    [Get("https://furion.net/")]
    Task<string> GetContent();
}

public interface IHttpTest2 : IHttpDeclarative;

public class HttpTest : IHttpDeclarative;

public interface IHttpDeclarativeTest : IHttpDeclarative
{
    Task InvalidMethod();

    [Get("https://furion.net/")]
    Task<string> Method1();

    [Get("https://furion.net/?name={name}")]
    Task<string> Method2([Query] int id, string name);

    [Get("https://furion.net/")]
    Task<string> Frozen(int id, string name, Action<HttpRequestBuilder> builder,
        Action<HttpMultipartFormDataBuilder> formBuilder, HttpCompletionOption completionOption,
        CancellationToken cancellationToken);

    [Get("{url}")]
    string GetUrl(string url, CancellationToken cancellationToken);

    [Get("{url}")]
    Task<string> GetUrlAsync(string url, CancellationToken cancellationToken);

    [Get("https://furion.net/")]
    [Timeout(1000)]
    Task<string> GetAttribute();

    [Get("https://furion.net/")]
    [Path("key1", "value1")]
    [Path("key2", "value2")]
    Task<string> GetMethodAttributes();
}

public class CustomHttpDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
    }
}