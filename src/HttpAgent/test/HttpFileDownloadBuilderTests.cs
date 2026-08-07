// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpFileDownloadBuilderTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new HttpFileDownloadBuilder(null!, null));

    [Fact]
    public void New_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        Assert.Equal(HttpMethod.Get, builder.HttpMethod);
        Assert.Null(builder.RequestUri);

        var builder2 = new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, builder2.HttpMethod);
        Assert.NotNull(builder2.RequestUri);
        Assert.Equal("http://localhost/", builder2.RequestUri.ToString());
        Assert.Equal(80 * 1024, builder2.BufferSize);
        Assert.Equal(1, builder2.MaxThreads);
        Assert.Null(builder2.OnProgressChanged);
        Assert.Null(builder2.DestinationPath);
        Assert.Equal(FileExistsBehavior.CreateNew, builder2.FileExistsBehavior);
        Assert.Equal(TimeSpan.FromMilliseconds(250), builder2.ProgressInterval);
        Assert.Null(builder2.OnTransferStarted);
        Assert.Null(builder2.OnTransferCompleted);
        Assert.Null(builder2.OnTransferFailed);
        Assert.Null(builder2.OnFileExistAndSkip);
        Assert.Null(builder2.FileTransferEventHandlerType);
        Assert.Equal(TimeSpan.FromSeconds(30), builder2.ChunkTimeout);
        Assert.Equal(3, builder2.ChunkMaxRetries);
        Assert.Null(builder2._configureRequest);
        Assert.Null(builder2.Configure);
        Assert.Same(builder2, builder2.This);
    }

    [Fact]
    public void SetBufferSize_Invalid_Parameters()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);

        var exception = Assert.Throws<ArgumentException>(() => builder.SetBufferSize(0));
        Assert.Equal("Buffer size must be greater than 0. (Parameter 'bufferSize')", exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => builder.SetBufferSize(-1));
        Assert.Equal("Buffer size must be greater than 0. (Parameter 'bufferSize')", exception2.Message);
    }

    [Fact]
    public void SetBufferSize_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.SetBufferSize(100 * 1024);

        Assert.Equal(100 * 1024, builder.BufferSize);
    }

    [Fact]
    public void SetDestinationPath_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.SetDestinationPath(@"C:\Workspaces");
        Assert.Equal(@"C:\Workspaces", builder.DestinationPath);

        var builder2 = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder2.SetDestinationPath(null);
        Assert.Null(builder2.DestinationPath);
    }

    [Fact]
    public void SetOnProgressChanged_Invalid_Parameters()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);

        Assert.Throws<ArgumentNullException>(() => builder.SetOnProgressChanged(null!));
    }

    [Fact]
    public void SetOnProgressChanged_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        Assert.Null(builder.OnProgressChanged);

        builder.SetOnProgressChanged(async _ =>
        {
            await Task.Delay(100);
        });

        Assert.NotNull(builder.OnProgressChanged);
    }

    [Fact]
    public void SetProgressInterval_Invalid_Parameters()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);

        var exception = Assert.Throws<ArgumentException>(() => builder.SetProgressInterval(TimeSpan.Zero));
        Assert.Equal("Progress interval must be greater than 0. (Parameter 'progressInterval')", exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => builder.SetProgressInterval(TimeSpan.FromSeconds(-1)));
        Assert.Equal("Progress interval must be greater than 0. (Parameter 'progressInterval')", exception2.Message);
    }

    [Fact]
    public void SetProgressInterval_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.SetProgressInterval(TimeSpan.FromSeconds(1));

        Assert.Equal(TimeSpan.FromSeconds(1), builder.ProgressInterval);
    }

    [Fact]
    public void SetMaxThreads_Invalid_Parameters()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);

        var exception = Assert.Throws<ArgumentException>(() => builder.SetMaxThreads(0));
        Assert.Equal("Max Threads must be greater than 0. (Parameter 'maxThreads')", exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => builder.SetMaxThreads(-1));
        Assert.Equal("Max Threads must be greater than 0. (Parameter 'maxThreads')", exception2.Message);
    }

    [Fact]
    public void SetMaxThreads_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.SetMaxThreads(2);

        Assert.Equal(2, builder.MaxThreads);
    }

    [Fact]
    public void SetChunkTimeout_Invalid_Parameters()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);

        var exception = Assert.Throws<ArgumentException>(() => builder.SetChunkTimeout(TimeSpan.Zero));
        Assert.Equal("Chunk timeout must be greater than 0 or Timeout.InfiniteTimeSpan. (Parameter 'chunkTimeout')",
            exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => builder.SetChunkTimeout(TimeSpan.FromSeconds(-1)));
        Assert.Equal("Chunk timeout must be greater than 0 or Timeout.InfiniteTimeSpan. (Parameter 'chunkTimeout')",
            exception2.Message);
    }

    [Fact]
    public void SetChunkTimeout_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.SetChunkTimeout(TimeSpan.FromSeconds(60));
        Assert.Equal(TimeSpan.FromSeconds(60), builder.ChunkTimeout);

        var builder2 = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder2.SetChunkTimeout(Timeout.InfiniteTimeSpan);
        Assert.Equal(Timeout.InfiniteTimeSpan, builder2.ChunkTimeout);
    }

    [Fact]
    public void SetChunkMaxRetries_Invalid_Parameters()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);

        var exception = Assert.Throws<ArgumentException>(() => builder.SetChunkMaxRetries(-1));
        Assert.Equal("Chunk max retries must be greater than or equal to 0. (Parameter 'chunkMaxRetries')",
            exception.Message);
    }

    [Fact]
    public void SetChunkMaxRetries_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.SetChunkMaxRetries(0);
        Assert.Equal(0, builder.ChunkMaxRetries);

        var builder2 = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder2.SetChunkMaxRetries(5);
        Assert.Equal(5, builder2.ChunkMaxRetries);
    }

    [Fact]
    public void SetOnTransferStarted_Invalid_Parameters()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        Assert.Throws<ArgumentNullException>(() => builder.SetOnTransferStarted(null!));
    }

    [Fact]
    public void SetOnTransferStarted_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.SetOnTransferStarted(() => { });

        Assert.NotNull(builder.OnTransferStarted);
    }

    [Fact]
    public void SetOnTransferCompleted_Invalid_Parameters()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        Assert.Throws<ArgumentNullException>(() => builder.SetOnTransferCompleted(null!));
    }

    [Fact]
    public void SetOnTransferCompleted_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.SetOnTransferCompleted(_ => { });

        Assert.NotNull(builder.OnTransferCompleted);
    }

    [Fact]
    public void SetOnTransferFailed_Invalid_Parameters()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        Assert.Throws<ArgumentNullException>(() => builder.SetOnTransferFailed(null!));
    }

    [Fact]
    public void SetOnTransferFailed_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.SetOnTransferFailed(_ => { });

        Assert.NotNull(builder.OnTransferFailed);
    }

    [Fact]
    public void SetOnFileExistAndSkip_Invalid_Parameters()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        Assert.Throws<ArgumentNullException>(() => builder.SetOnFileExistAndSkip(null!));
    }

    [Fact]
    public void SetOnFileExistAndSkip_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.SetOnFileExistAndSkip(() => { });

        Assert.NotNull(builder.OnFileExistAndSkip);
    }

    [Fact]
    public void SetEventHandler_Invalid_Parameters()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);

        Assert.Throws<ArgumentNullException>(() => builder.SetEventHandler(null!));
        var exception = Assert.Throws<ArgumentException>(() =>
            builder.SetEventHandler(typeof(NotImplementFileTransferEventHandler)));
        Assert.Equal(
            $"`{typeof(NotImplementFileTransferEventHandler)}` type is not assignable from `{typeof(IHttpFileTransferEventHandler)}`. (Parameter 'fileTransferEventHandlerType')",
            exception.Message);
    }

    [Fact]
    public void SetEventHandler_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.SetEventHandler(typeof(CustomFileTransferEventHandler));

        Assert.Equal(typeof(CustomFileTransferEventHandler), builder.FileTransferEventHandlerType);

        var builder2 = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder2.SetEventHandler<CustomFileTransferEventHandler>();

        Assert.Equal(typeof(CustomFileTransferEventHandler), builder2.FileTransferEventHandlerType);
    }

    [Fact]
    public void With_Invalid_Parameters()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        Assert.Throws<ArgumentNullException>(() => builder.With(null!));
    }

    [Fact]
    public void With_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        Assert.Null(builder._configureRequest);
        builder.With(requestBuilder => requestBuilder.WithHeader("framework", "Furion"));
        Assert.NotNull(builder._configureRequest);
        Assert.NotNull(builder.Configure);
    }

    [Fact]
    public void Profiler_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.Profiler();

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null);
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.True(httpRequestBuilder.ProfilerEnabled);

        builder.Profiler(false);
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.False(httpRequestBuilder.ProfilerEnabled);

        builder.Profiler(_ => { });
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.True(httpRequestBuilder.ProfilerEnabled);
    }

    [Fact]
    public void DisableCache_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.DisableCache();

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null);
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.True(httpRequestBuilder.DisableCacheEnabled);

        builder.DisableCache(false);
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.False(httpRequestBuilder.DisableCacheEnabled);
    }

    [Fact]
    public void AddBearerAuthentication_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.AddBearerAuthentication("token");

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null);
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.NotNull(httpRequestBuilder.AuthenticationHeader);
        Assert.Equal("Bearer", httpRequestBuilder.AuthenticationHeader.Scheme);

        builder.AddBearerAuthentication("x-header", "token");
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.NotNull(httpRequestBuilder.Headers);
        Assert.StartsWith("Bearer", httpRequestBuilder.Headers["x-header"].First());
    }

    [Fact]
    public void SetJsonContent_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.SetJsonContent(new { });

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null);
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.NotNull(httpRequestBuilder.RawContent);

        builder.SetJsonContentWithoutValidation("[]");
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.NotNull(httpRequestBuilder.RawContent);
    }

    [Fact]
    public void SetContent_ReturnOK()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);
        builder.SetContent(new { });

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null);
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.NotNull(httpRequestBuilder.RawContent);
    }

    [Fact]
    public void Build_Invalid_Parameters()
    {
        var builder = new HttpFileDownloadBuilder(HttpMethod.Get, null);

        var httpRemoteOptions = new HttpRemoteOptions();

        Assert.Throws<ArgumentNullException>(() => builder.Build(null!));
        Assert.Throws<ArgumentException>(() => builder.SetDestinationPath(string.Empty).Build(httpRemoteOptions));
        Assert.Throws<ArgumentException>(() => builder.SetDestinationPath(" ").Build(httpRemoteOptions));
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var httpFileDownloadBuilder = new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpFileDownloadBuilder.SetDestinationPath(@"C:\Workspaces").Profiler();

        var httpRemoteOptions = new HttpRemoteOptions();

        var httpRequestBuilder = httpFileDownloadBuilder.Build(httpRemoteOptions);
        Assert.NotNull(httpRequestBuilder);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder.HttpMethod);
        Assert.NotNull(httpRequestBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder.RequestUri.ToString());
        Assert.True(httpRequestBuilder.EnsureSuccessStatusCodeEnabled);
        Assert.Null(httpRequestBuilder.RequestEventHandlerType);
        Assert.True(httpRequestBuilder.PerformanceOptimizationEnabled);
        Assert.True(httpRequestBuilder.ProfilerEnabled);

        var httpRequestBuilder2 = httpFileDownloadBuilder.SetEventHandler<CustomFileTransferEventHandler2>()
            .With(builder =>
            {
                builder.SetTimeout(100);
            }).Build(httpRemoteOptions);

        Assert.Equal(TimeSpan.FromMilliseconds(100), httpRequestBuilder2.TimeoutOptions?.Timeout);
        Assert.NotNull(httpRequestBuilder2.RequestEventHandlerType);

        var httpFileDownloadBuilder3 = new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpFileDownloadBuilder3.Build(new HttpRemoteOptions { DefaultFileDownloadDirectory = @"C:\Workspaces" });
        Assert.Equal(@"C:\Workspaces", httpFileDownloadBuilder3.DestinationPath);

        var httpFileDownloadBuilder2 = new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("http://localhost"));
        _ = httpFileDownloadBuilder2.Build(new HttpRemoteOptions());
        Assert.Equal(AppContext.BaseDirectory, httpFileDownloadBuilder2.DestinationPath);
    }
}