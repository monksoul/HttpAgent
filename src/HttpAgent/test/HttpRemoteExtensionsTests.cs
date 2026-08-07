// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using SameSiteMode = Microsoft.Net.Http.Headers.SameSiteMode;

namespace HttpAgent.Tests;

public class HttpRemoteExtensionsTests
{
    [Fact]
    public void AddProfilerDelegatingHandler_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpClient(string.Empty);

        using var serviceProvider = services.BuildServiceProvider();
        var httpClientFactoryOptions = serviceProvider.GetService<IOptions<HttpClientFactoryOptions>>()?.Value;
        Assert.NotNull(httpClientFactoryOptions);
        Assert.NotNull(httpClientFactoryOptions.HttpMessageHandlerBuilderActions);
        Assert.Empty(httpClientFactoryOptions.HttpMessageHandlerBuilderActions);

        var services2 = new ServiceCollection();
        services2.AddHttpClient(string.Empty).AddProfilerDelegatingHandler();
        Assert.Contains(services2, u => u.ServiceType == typeof(ProfilerDelegatingHandler));

        using var serviceProvider2 = services2.BuildServiceProvider();
        var httpClientFactoryOptions2 = serviceProvider2.GetService<IOptions<HttpClientFactoryOptions>>()?.Value;
        Assert.NotNull(httpClientFactoryOptions2);
        Assert.Single(httpClientFactoryOptions2.HttpMessageHandlerBuilderActions);

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Production" });
        builder.Services.AddHttpClient(string.Empty)
            .AddProfilerDelegatingHandler(() => builder.Environment.EnvironmentName == "Production");
        Assert.NotNull(httpClientFactoryOptions.HttpMessageHandlerBuilderActions);
        Assert.Empty(httpClientFactoryOptions.HttpMessageHandlerBuilderActions);

        var builder2 = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Production" });
        builder2.Services.AddHttpClient(string.Empty).AddProfilerDelegatingHandler(true);
        Assert.NotNull(httpClientFactoryOptions.HttpMessageHandlerBuilderActions);
        Assert.Empty(httpClientFactoryOptions.HttpMessageHandlerBuilderActions);
    }

    [Fact]
    public void PerformanceOptimization_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpRemoteExtensions.PerformanceOptimization(null!));

    [Fact]
    public void PerformanceOptimization_ReturnOK()
    {
        using var httpClient = new HttpClient();
        httpClient.PerformanceOptimization();

        Assert.NotEmpty(httpClient.DefaultRequestHeaders);
        Assert.Equal("*/*", httpClient.DefaultRequestHeaders.Accept.ToString());
#if NET11_0_OR_GREATER
        Assert.Equal("gzip, deflate, br, zstd", httpClient.DefaultRequestHeaders.AcceptEncoding.ToString());
#else
        Assert.Equal("gzip, deflate, br", httpClient.DefaultRequestHeaders.AcceptEncoding.ToString());
#endif
        Assert.False(httpClient.DefaultRequestHeaders.ConnectionClose);
    }

    [Fact]
    public void ProfilerHeaders_HttpRequestMessage_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        Assert.Equal(
            "\e[36m\e[1mRequest Headers:\e[0m \r\n  Accept:              application/json\r\n  Accept-Encoding:     gzip, deflate",
            httpRequestMessage.ProfilerHeaders());
        Assert.Equal("Accept:              application/json\r\nAccept-Encoding:     gzip, deflate",
            httpRequestMessage.ProfilerHeaders(summary: null));
    }

    [Fact]
    public void ProfilerHeaders_HttpRequestMessage_WithContent_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Content = new StringContent("Furion", Encoding.UTF8, "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        Assert.Equal(
            "\e[36m\e[1mRequest Headers:\e[0m \r\n  Accept:              application/json\r\n  Accept-Encoding:     gzip, deflate\r\n  Content-Type:        application/json; charset=utf-8",
            httpRequestMessage.ProfilerHeaders());
        Assert.Equal(
            "Accept:              application/json\r\nAccept-Encoding:     gzip, deflate\r\nContent-Type:        application/json; charset=utf-8",
            httpRequestMessage.ProfilerHeaders(summary: null));
    }

    [Fact]
    public void ProfilerHeaders_HttpRequestMessage_WithHttpClient_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAgents.Edge.PC);

        Assert.Equal(
            "\e[36m\e[1mRequest Headers:\e[0m \r\n  User-Agent:          Mozilla/5.0, (Windows NT 10.0; Win64; x64), AppleWebKit/537.36, (KHTML, like Gecko), Chrome/150.0.0.0, Safari/537.36, Edg/150.0.0.0\r\n  Accept:              application/json\r\n  Accept-Encoding:     gzip, deflate",
            httpRequestMessage.ProfilerHeaders(httpClient));
        Assert.Equal(
            "User-Agent:          Mozilla/5.0, (Windows NT 10.0; Win64; x64), AppleWebKit/537.36, (KHTML, like Gecko), Chrome/150.0.0.0, Safari/537.36, Edg/150.0.0.0\r\nAccept:              application/json\r\nAccept-Encoding:     gzip, deflate",
            httpRequestMessage.ProfilerHeaders(httpClient, null));
    }

    [Fact]
    public void ProfilerHeaders_HttpResponseMessage_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
        httpResponseMessage.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json");

        Assert.Equal(
            "\e[36m\e[1mResponse Headers:\e[0m \r\n  Accept:              application/json\r\n  Accept-Encoding:     gzip, deflate\r\n  Content-Type:        application/json",
            httpResponseMessage.ProfilerHeaders());
        Assert.Equal(
            "Accept:              application/json\r\nAccept-Encoding:     gzip, deflate\r\nContent-Type:        application/json",
            httpResponseMessage.ProfilerHeaders(null));
    }

    [Fact]
    public void ProfilerGeneralAndHeaders_Invalid_Parameters()
    {
        var httpResponseMessage = new HttpResponseMessage();
        Assert.Throws<ArgumentNullException>(() => HttpRemoteExtensions.ProfilerGeneralAndHeaders(null!));
        Assert.Throws<ArgumentNullException>(() => httpResponseMessage.ProfilerGeneralAndHeaders());
    }

    [Fact]
    public void ProfilerGeneralAndHeaders_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        var httpResponseMessage =
            new HttpResponseMessage { RequestMessage = httpRequestMessage, StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
        httpResponseMessage.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json");

        Assert.Equal(
            "\e[36m\e[1mGeneral:\e[0m \r\n  Request URL:        http://localhost\r\n  Request Method:     GET\r\n  Status Code:        \e[32m\e[1m200 OK\e[0m\r\n  HTTP Version:       1.1\r\n\e[36m\e[1mResponse Headers:\e[0m \r\n  Accept:              application/json\r\n  Accept-Encoding:     gzip, deflate\r\n  Content-Type:        application/json",
            httpResponseMessage.ProfilerGeneralAndHeaders());

        Assert.Equal(
            "\e[36m\e[1mGeneral:\e[0m \r\n  Request URL:          http://localhost\r\n  Request Method:       GET\r\n  Status Code:          \e[32m\e[1m200 OK\e[0m\r\n  HTTP Version:         1.1\r\n  Request Duration:     200ms\r\n\e[36m\e[1mResponse Headers:\e[0m \r\n  Accept:              application/json\r\n  Accept-Encoding:     gzip, deflate\r\n  Content-Type:        application/json",
            httpResponseMessage.ProfilerGeneralAndHeaders(generalCustomKeyValues:
                [new KeyValuePair<string, IEnumerable<string>>("Request Duration", ["200ms"])]));
    }

    [Fact]
    public void GetColoredText_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpRemoteExtensions.GetColoredText(null!, null));

    [Fact]
    public void GetColoredText_ReturnOK()
    {
        Assert.Equal("\e[32m\e[1m200 OK\e[0m",
            new HttpResponseMessage(HttpStatusCode.OK).GetColoredText("200 OK"));

        Assert.Equal("\e[32m\e[1m304 NotModified\e[0m",
            new HttpResponseMessage(HttpStatusCode.NotModified).GetColoredText("304 NotModified"));

        Assert.Equal("\e[33m\e[1m302 Found\e[0m",
            new HttpResponseMessage(HttpStatusCode.Redirect).GetColoredText("302 Found"));
        Assert.Equal("\e[33m\e[1m307 TemporaryRedirect\e[0m",
            new HttpResponseMessage(HttpStatusCode.TemporaryRedirect).GetColoredText("307 TemporaryRedirect"));

        Assert.Equal("\e[31m\e[1m400 BadRequest\e[0m",
            new HttpResponseMessage(HttpStatusCode.BadRequest).GetColoredText("400 BadRequest"));

        Assert.Equal("\e[31m\e[1m500 InternalServerError\e[0m",
            new HttpResponseMessage(HttpStatusCode.InternalServerError).GetColoredText("500 InternalServerError"));

        Assert.Equal("\e[34m\e[1m100 Continue\e[0m",
            new HttpResponseMessage(HttpStatusCode.Continue).GetColoredText("100 Continue"));

        Assert.Equal("\e[90m\e[1m600 Custom\e[0m",
            new HttpResponseMessage((HttpStatusCode)600).GetColoredText("600 Custom"));

        Assert.Equal("\e[32m200 OK\e[0m",
            new HttpResponseMessage(HttpStatusCode.OK).GetColoredText("200 OK", false));
    }

    [Fact]
    public void StreamContentInternalFields_ReturnOK()
    {
        Assert.NotNull(HttpRemoteExtensions.StreamContentInternalFields);
        Assert.NotEmpty(HttpRemoteExtensions.StreamContentInternalFields);
        Assert.Contains(HttpRemoteExtensions.StreamContentInternalFields,
            field => typeof(Stream).IsAssignableFrom(field.FieldType));
    }

    [Fact]
    public void GetHexDump_ReturnOK()
    {
        var buffer = new byte[32];
        for (var i = 0; i < 32; i++)
        {
            buffer[i] = (byte)i;
        }

        var result = HttpRemoteExtensions.GetHexDump(buffer, 32);
        Assert.Contains("00000000  00 01 02 03 04 05 06 07  08 09 0A 0B 0C 0D 0E 0F", result);
        Assert.Contains("00000010  10 11 12 13 14 15 16 17  18 19 1A 1B 1C 1D 1E 1F", result);

        var partialBuffer = "ABC\0"u8.ToArray();
        var partialResult = HttpRemoteExtensions.GetHexDump(partialBuffer, 4);
        Assert.Contains("00000000  41 42 43 00", partialResult);
        Assert.Contains("|ABC.", partialResult);
    }

    [Fact]
    public void FormatBytes_ReturnOK()
    {
        Assert.Equal(string.Empty, HttpRemoteExtensions.FormatBytes([], 0, 5120, false, 0, false, null));

        var binaryBuffer = new byte[] { 0x00, 0x01, 0x02, 0xFF };
        var binaryResult = HttpRemoteExtensions.FormatBytes(binaryBuffer, 4, 5120, false, 4, false, null);
        Assert.Contains("00000000", binaryResult);
        Assert.Contains("00 01 02 FF", binaryResult);

        var largeBinary = new byte[2048];
        var largeBinaryResult = HttpRemoteExtensions.FormatBytes(largeBinary, 2048, 5120, true, 2048, false, null);
        Assert.Contains("\e[36m\e[1m... [Binary content, showing first 512 bytes of 2048 total bytes]\e[0m",
            largeBinaryResult);

        var textBuffer = "Hello World"u8.ToArray();
        var textResult = HttpRemoteExtensions.FormatBytes(textBuffer, 11, 5120, false, 11, false, null);
        Assert.Equal("Hello World", textResult);

        var longTextBuffer = Encoding.UTF8.GetBytes(new string('A', 6000));
        var longTextResult = HttpRemoteExtensions.FormatBytes(longTextBuffer, 5120, 5120, true, 6000, false, null);
        Assert.Contains("\e[36m\e[1m ... [truncated, > 5120 bytes]\e[0m", longTextResult);

        var utf8Buffer = "Test"u8.ToArray();
        var fallbackResult =
            HttpRemoteExtensions.FormatBytes(utf8Buffer, 4, 5120, false, 4, false, null, "invalid-charset-xxx");
        Assert.Equal("Test", fallbackResult);

        var successResp = new HttpResponseMessage(HttpStatusCode.OK);
        var coloredResult = HttpRemoteExtensions.FormatBytes(utf8Buffer, 4, 5120, false, 4, true, successResp);
        Assert.Contains("\e[32m", coloredResult);
    }

    [Fact]
    public async Task FormatContentBodyAsync_ReturnOK()
    {
        var stringContent = new StringContent("Hello World", Encoding.UTF8, "text/plain");
        var (body, totalRead, isTruncated) = await HttpRemoteExtensions.FormatContentBodyAsync(
            stringContent, 5120, false, null, null, TestContext.Current.CancellationToken);
        Assert.Equal("Hello World", body);
        Assert.Equal(11, totalRead);
        Assert.False(isTruncated);

        const string rawText = "Compressed Text";
        var compressedBytes = GzipCompress(Encoding.UTF8.GetBytes(rawText));
        var compressedContent = new ByteArrayContent(compressedBytes);
        compressedContent.Headers.ContentEncoding.Add("gzip");
        compressedContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain") { CharSet = "utf-8" };

        var (gzipBody, gzipRead, gzipTruncated) = await HttpRemoteExtensions.FormatContentBodyAsync(
            compressedContent, 5120, false, "gzip", null, TestContext.Current.CancellationToken);
        Assert.Equal(rawText, gzipBody);
        Assert.Equal(rawText.Length, gzipRead);
        Assert.False(gzipTruncated);

        var longContent = new StringContent(new string('X', 6000));
        var (_, longRead, longTruncated) = await HttpRemoteExtensions.FormatContentBodyAsync(
            longContent, 5120, false, null, null, TestContext.Current.CancellationToken);
        Assert.True(longTruncated);
        Assert.Equal(5121, longRead);
    }

    [Fact]
    public async Task ProfilerAsync_ReturnOK()
    {
        Assert.Null(await HttpRemoteExtensions.ProfilerAsync(null,
            cancellationToken: TestContext.Current.CancellationToken));

        var stringContent = new StringContent("Hello World");
        Assert.Equal("\e[36m\e[1mRequest Body (StringContent, total: 11 bytes):\e[0m \r\n  Hello World",
            await stringContent.ProfilerAsync(cancellationToken: TestContext.Current.CancellationToken));

        var jsonContent = JsonContent.Create(new { id = 1, name = "furion" });
        Assert.Equal(
            "\e[36m\e[1mRequest Body (JsonContent, total: 24 bytes):\e[0m \r\n  {\"id\":1,\"name\":\"furion\"}",
            await jsonContent.ProfilerAsync(cancellationToken: TestContext.Current.CancellationToken));

        var byteArrayContent = new ByteArrayContent("Hello World"u8.ToArray());
        Assert.Equal("\e[36m\e[1mRequest Body (ByteArrayContent, total: 11 bytes):\e[0m \r\n  Hello World",
            await byteArrayContent.ProfilerAsync(cancellationToken: TestContext.Current.CancellationToken));

        var formUrlEncodedContent = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("id", "1"), new KeyValuePair<string, string>("name", "Furion")
        ]);
        Assert.Equal("\e[36m\e[1mRequest Body (FormUrlEncodedContent, total: 16 bytes):\e[0m \r\n  id=1&name=Furion",
            await formUrlEncodedContent.ProfilerAsync(cancellationToken: TestContext.Current.CancellationToken));

        var streamStream = new StreamContent(File.OpenRead(Path.Combine(AppContext.BaseDirectory, "test.txt")));
        Assert.Equal("\e[36m\e[1mRequest Body (StreamContent, total: 21 bytes):\e[0m \r\n  \ufeff测试文件内容",
            await streamStream.ProfilerAsync(cancellationToken: TestContext.Current.CancellationToken));

        var readOnlyMemoryContent = new ReadOnlyMemoryContent(new ReadOnlyMemory<byte>("Hello World"u8.ToArray()));
        Assert.Equal("\e[36m\e[1mRequest Body (ReadOnlyMemoryContent, total: 11 bytes):\e[0m \r\n  Hello World",
            await readOnlyMemoryContent.ProfilerAsync(cancellationToken: TestContext.Current.CancellationToken));

        var multipartFormDataContent = new MultipartFormDataContent("--------------------------");
        multipartFormDataContent.Add(new StringContent("Hello World"), "text");
        multipartFormDataContent.Add(
            new StreamContent(File.OpenRead(Path.Combine(AppContext.BaseDirectory, "test.txt"))), "file");
        Assert.Equal(
            "\e[36m\e[1mRequest Body (MultipartFormDataContent, total: 32 bytes):\e[0m \r\n  \e[90m--------------------------\e[0m\r\n  Content-Type: text/plain; charset=utf-8\r\n  Content-Disposition: form-data; name=text\r\n  \r\n  Hello World\r\n  \e[90m--------------------------\e[0m\r\n  Content-Disposition: form-data; name=file\r\n  \r\n  \ufeff测试文件内容",
            await multipartFormDataContent.ProfilerAsync(cancellationToken: TestContext.Current.CancellationToken));

        var stringContent2 = new StringContent("Hello World");
        Assert.Equal("\e[36m\e[1mResponse Body (StringContent, total: 11 bytes):\e[0m \r\n  Hello World",
            await stringContent2.ProfilerAsync("Response Body",
                cancellationToken: TestContext.Current.CancellationToken));

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        var stringContent3 = new StringContent("Hello World");
        httpResponseMessage.Content = stringContent3;
        Assert.Equal("\e[36m\e[1mRequest Body (StringContent, total: 11 bytes):\e[0m \r\n  \e[31mHello World\e[0m",
            await stringContent3.ProfilerAsync(httpResponseMessage: httpResponseMessage,
                cancellationToken: TestContext.Current.CancellationToken));

        var httpResponseMessage2 = new HttpResponseMessage(HttpStatusCode.Redirect);
        var stringContent4 = new StringContent("Hello World");
        httpResponseMessage2.Content = stringContent4;
        Assert.Equal("\e[36m\e[1mRequest Body (StringContent, total: 11 bytes):\e[0m \r\n  \e[33mHello World\e[0m",
            await stringContent4.ProfilerAsync(httpResponseMessage: httpResponseMessage2,
                cancellationToken: TestContext.Current.CancellationToken));

        var largeNoLengthContent = new LargeContentWithoutLength(11 * 1024 * 1024);
        var exReq = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            largeNoLengthContent.ProfilerAsync(cancellationToken: TestContext.Current.CancellationToken));
        Assert.Contains("The request body for request", exReq.Message);
        Assert.Contains("5 MB", exReq.Message);

        using var reqMsg = new HttpRequestMessage(HttpMethod.Post, "https://furion.net/api/data");
        var largeNoLengthContent2 = new LargeContentWithoutLength(11 * 1024 * 1024);
        var exReqWithUrl = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            largeNoLengthContent2.ProfilerAsync(httpRequestMessage: reqMsg,
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Contains("for request 'https://furion.net/api/data'", exReqWithUrl.Message);

        using var respMsg = new HttpResponseMessage();
        respMsg.RequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://furion.net/api/response");
        var largeNoLengthContent3 = new LargeContentWithoutLength(11 * 1024 * 1024);
        var exResp = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            largeNoLengthContent3.ProfilerAsync(httpResponseMessage: respMsg,
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Contains("The response body for request 'https://furion.net/api/response'", exResp.Message);

        var streamingResp = new HttpResponseMessage();
        streamingResp.RequestMessage = new HttpRequestMessage();
        streamingResp.RequestMessage.Options.Set(
            new HttpRequestOptionsKey<HttpCompletionOption>(Constants.HTTP_COMPLETION_OPTION_KEY),
            HttpCompletionOption.ResponseHeadersRead);
        streamingResp.Content = new NoLengthStreamContent("ignored"u8.ToArray());
        var streamSkipResult = await streamingResp.Content.ProfilerAsync(httpResponseMessage: streamingResp,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Contains("Skipped: ResponseHeadersRead", streamSkipResult);

        var longText = new string('A', 11 * 1024);
        var truncContent = new StringContent(longText);
        var truncResult = await truncContent.ProfilerAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(truncResult);
        Assert.Contains("[truncated, > 5120 bytes]", truncResult);
        Assert.True(truncResult.Length < longText.Length + 100);

        const string rawText = "Hello, this is a test content!";
        var compressedBytes = GzipCompress(Encoding.UTF8.GetBytes(rawText));
        var compressedContent = new ByteArrayContent(compressedBytes);
        compressedContent.Headers.ContentEncoding.Add("gzip");
        compressedContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain") { CharSet = "utf-8" };
        var gzipResp = new HttpResponseMessage();
        gzipResp.Content = compressedContent;
        var gzipResult = await compressedContent.ProfilerAsync(httpResponseMessage: gzipResp,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Contains(rawText, gzipResult);

        var largeWithLengthContent = new StringContent(new string('B', 11 * 1024 * 1024));
        var skipResult =
            await largeWithLengthContent.ProfilerAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Contains("[Skipped: content too large", skipResult);
        Assert.DoesNotContain("BBBB", skipResult);

        var successResp = new HttpResponseMessage(HttpStatusCode.OK);
        var successContent = new StringContent("Success");
        successResp.Content = successContent;
        var successResult = await successContent.ProfilerAsync(httpResponseMessage: successResp,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Contains("\e[32m", successResult);
        Assert.Contains("Success", successResult);

        var redirectResp = new HttpResponseMessage(HttpStatusCode.Redirect);
        var redirectContent = new StringContent("Redirect");
        redirectResp.Content = redirectContent;
        var redirectResult = await redirectContent.ProfilerAsync(httpResponseMessage: redirectResp,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Contains("\e[33m", redirectResult);
        Assert.Contains("Redirect", redirectResult);

        var emptyBoundaryContent = new MultipartContent();
        emptyBoundaryContent.Add(new StringContent("test data"));
        emptyBoundaryContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
        var emptyBoundaryResult =
            await emptyBoundaryContent.ProfilerAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Contains("\e[36m\e[1m[Warning: Missing boundary in Content-Type]\e[0m", emptyBoundaryResult);
        Assert.Contains("test data", emptyBoundaryResult);
    }

    [Fact]
    public async Task ProfilerAsync_304NotModified_EmptyContent_ReturnOK()
    {
        var resp304 = new HttpResponseMessage(HttpStatusCode.NotModified);
        resp304.Content = new StringContent("");
        var result = await resp304.Content.ProfilerAsync(httpResponseMessage: resp304,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Contains("\e[36m\e[1m[Empty: 304 NotModified, no content returned by server]\e[0m", result);

        var multipart304 = new HttpResponseMessage(HttpStatusCode.NotModified);
        var multipartContent = new MultipartFormDataContent("myboundary");
        multipart304.Content = multipartContent;
        var multipartResult = await multipartContent.ProfilerAsync(httpResponseMessage: multipart304,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Contains("\e[36m\e[1m[Empty: 304 NotModified, no content returned by server]\e[0m", multipartResult);
    }

    [Fact]
    public async Task ProfilerAsync_LargeContent_Skipped_ReturnOK()
    {
        var largeContent = new MockLargeContent(6 * 1024 * 1024);
        var result = await largeContent.ProfilerAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Contains("\e[36m\e[1m[Skipped: content too large (6291456 bytes) > 5242880]\e[0m", result);
    }

    [Fact]
    public async Task ProfilerAsync_NonSeekableStreamRequest_Skipped_ReturnOK()
    {
        await using var nonSeekableStream = new NonSeekableStream();
        await nonSeekableStream.WriteAsync("data"u8.ToArray(), TestContext.Current.CancellationToken);
        nonSeekableStream.Position = 0;

        var streamContent = new StreamContent(nonSeekableStream);
        var result = await streamContent.ProfilerAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Contains("underlying stream is not seekable and buffering is disabled to protect it", result);
    }

    [Fact]
    public async Task ProfilerAsync_Multipart_NonSeekableStream_Skipped_ReturnOK()
    {
        await using var nonSeekableStream = new NonSeekableStream();
        await nonSeekableStream.WriteAsync("network data"u8.ToArray(), TestContext.Current.CancellationToken);
        nonSeekableStream.Position = 0;

        var multipart = new MultipartFormDataContent("boundary123");
        multipart.Add(new StreamContent(nonSeekableStream), "file", "test.txt");

        var result = await multipart.ProfilerAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Contains("Forward-only stream", result);
    }

    [Fact]
    public async Task ProfilerAsync_Multipart_SeekableStream_ReadAndRestore_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        await using var fileStream = File.OpenRead(filePath);
        var initialPosition = fileStream.Position;

        var multipart = new MultipartFormDataContent("my_boundary");
        var streamContent = new StreamContent(fileStream);
        multipart.Add(streamContent, "file", "test.txt");

        var result = await multipart.ProfilerAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Contains("my_boundary", result);
        Assert.Contains("\ufeff测试文件内容", result);
        Assert.Equal(initialPosition, fileStream.Position);
    }

    [Fact]
    public async Task CloneAsync_Invalid_Parameters() =>
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            HttpRemoteExtensions.CloneAsync(null!, TestContext.Current.CancellationToken));

    [Fact]
    public async Task CloneAsync_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");
        httpRequestMessage.Headers.TryAddWithoutValidation("User-Agent", "furion");
        var stringContent = new StringContent("Hello World", Encoding.UTF8, "application/json");
        httpRequestMessage.Content = stringContent;

        httpRequestMessage.Options.TryAdd("name", "Furion");

        var clonedHttpRequestMessage =
            await httpRequestMessage.CloneAsync(TestContext.Current.CancellationToken);
        Assert.Equal("furion", clonedHttpRequestMessage.Headers.UserAgent.ToString());
        Assert.Single(clonedHttpRequestMessage.Options);
        Assert.True(
            clonedHttpRequestMessage.Options.TryGetValue(new HttpRequestOptionsKey<string>("name"), out var name));
        Assert.Equal("Furion", name);

        var streamContent = clonedHttpRequestMessage.Content as StreamContent;
        Assert.NotNull(streamContent);
        var str = await streamContent.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Equal("Hello World", str);

        Assert.Equal("application/json", clonedHttpRequestMessage.Content?.Headers.ContentType?.MediaType);
    }

    [Fact]
    public void Clone_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            HttpRemoteExtensions.Clone(null!, TestContext.Current.CancellationToken));

    [Fact]
    public void Clone_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");
        httpRequestMessage.Headers.TryAddWithoutValidation("User-Agent", "furion");
        var stringContent = new StringContent("Hello World", Encoding.UTF8, "application/json");
        httpRequestMessage.Content = stringContent;

        var clonedHttpRequestMessage =
            httpRequestMessage.Clone(TestContext.Current.CancellationToken);
        Assert.Equal("furion", clonedHttpRequestMessage.Headers.UserAgent.ToString());

        var streamContent = clonedHttpRequestMessage.Content as StreamContent;
        Assert.NotNull(streamContent);
#pragma warning disable xUnit1031
        var str = AsyncUtility.RunSync(() => streamContent.ReadAsStringAsync(TestContext.Current.CancellationToken));
#pragma warning restore xUnit1031
        Assert.Equal("Hello World", str);

        Assert.Equal("application/json", clonedHttpRequestMessage.Content?.Headers.ContentType?.MediaType);
    }

    [Fact]
    public void TryGetSetCookies_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ((HttpResponseMessage)null!).TryGetSetCookies(out _, out _));
        Assert.Throws<ArgumentNullException>(() =>
            ((HttpResponseHeaders)null!).TryGetSetCookies(out _, out _));
    }

    [Fact]
    public void TryGetSetCookies_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Headers.TryGetSetCookies(out var setCookies, out var rawSetCookies);

        Assert.Null(rawSetCookies);
        Assert.Null(setCookies);

        var httpResponseMessage2 = new HttpResponseMessage();
        const string setCookieHeader =
            "BDUSS_BFESS=hBSH5yRDI1a0Fzb2lMWllDYk0tRkZ0UEc2OW1URjBvLUtVckNMeFUyaUNxdWxtRVFBQUFBJCQAAAAAAAAAAAEAAADeGZbRsNnHqc34xcwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIIdwmaCHcJmUm; Path=/; Domain=baidu.com; Expires=Fri, 01 Sep 2034 02:22:19 GMT; Max-Age=315360000; HttpOnly; Secure; SameSite=None";

        httpResponseMessage2.Headers.Add("Set-Cookie", setCookieHeader);

        httpResponseMessage2.Headers.TryGetSetCookies(out var setCookies2, out var rawSetCookies2);

        Assert.NotNull(rawSetCookies2);
        Assert.NotNull(setCookies2);
        Assert.Equal(setCookieHeader, rawSetCookies2.First());
        Assert.Single(setCookies2);

        var cookies = setCookies2.First();
        Assert.Equal("baidu.com", cookies.Domain.ToString());
        Assert.Equal("/", cookies.Path.ToString());
        Assert.Equal("2034/9/1 2:22:19 +00:00", cookies.Expires.ToString());
        Assert.Equal(TimeSpan.FromSeconds(315360000), cookies.MaxAge);
        Assert.True(cookies.HttpOnly);
        Assert.True(cookies.Secure);
        Assert.Equal(SameSiteMode.None, cookies.SameSite);
        Assert.Equal("BDUSS_BFESS", cookies.Name.ToString());
        Assert.Equal(
            "hBSH5yRDI1a0Fzb2lMWllDYk0tRkZ0UEc2OW1URjBvLUtVckNMeFUyaUNxdWxtRVFBQUFBJCQAAAAAAAAAAAEAAADeGZbRsNnHqc34xcwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIIdwmaCHcJmUm",
            cookies.Value.ToString());

        httpResponseMessage.TryGetSetCookies(out var setCookies3, out var rawSetCookies3);
        Assert.Null(setCookies3);
        Assert.Null(rawSetCookies3);
        httpResponseMessage2.TryGetSetCookies(out var setCookies4, out var rawSetCookies4);

        Assert.NotNull(rawSetCookies4);
        Assert.NotNull(setCookies4);
        Assert.Equal(setCookieHeader, rawSetCookies4.First());
        Assert.Single(setCookies4);

        var cookies2 = setCookies4.First();
        Assert.Equal("baidu.com", cookies2.Domain.ToString());
        Assert.Equal("/", cookies2.Path.ToString());
        Assert.Equal("2034/9/1 2:22:19 +00:00", cookies2.Expires.ToString());
        Assert.Equal(TimeSpan.FromSeconds(315360000), cookies2.MaxAge);
        Assert.True(cookies2.HttpOnly);
        Assert.True(cookies2.Secure);
        Assert.Equal(SameSiteMode.None, cookies2.SameSite);
        Assert.Equal("BDUSS_BFESS", cookies2.Name.ToString());
        Assert.Equal(
            "hBSH5yRDI1a0Fzb2lMWllDYk0tRkZ0UEc2OW1URjBvLUtVckNMeFUyaUNxdWxtRVFBQUFBJCQAAAAAAAAAAAEAAADeGZbRsNnHqc34xcwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIIdwmaCHcJmUm",
            cookies2.Value.ToString());
    }

    [Fact]
    public void GetHostEnvironmentName_ReturnOK()
    {
        var services = new ServiceCollection();
        Assert.Null(HttpRemoteExtensions.GetHostEnvironmentName(services));

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Development" });
        Assert.Equal("Development", HttpRemoteExtensions.GetHostEnvironmentName(builder.Services));

        var builder2 = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Production" });
        Assert.Equal("Production", HttpRemoteExtensions.GetHostEnvironmentName(builder2.Services));
    }

    [Fact]
    public void ConfigureOptions_Invalid_Parameters()
    {
        var services = new ServiceCollection();

        Assert.Throws<ArgumentNullException>(() =>
            services.AddHttpClient(string.Empty).ConfigureOptions((Action<HttpClientOptions>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            services.AddHttpClient(string.Empty).ConfigureOptions((Action<HttpClientOptions, IServiceProvider>)null!));
    }

    [Fact]
    public void ConfigureOptions_ReturnOK()
    {
        var services = new ServiceCollection();

        services.AddHttpClient(string.Empty)
            .ConfigureOptions(options => options.JsonSerializerOptions.IncludeFields = true);
        services.AddHttpClient("github").ConfigureOptions(options =>
        {
            options.JsonSerializerOptions.IncludeFields = true;
        });

        var serviceProvider = services.BuildServiceProvider();

        var httpClientOptionsAccessor = serviceProvider.GetRequiredService<IOptionsMonitor<HttpClientOptions>>();

        var httpClientOptions = httpClientOptionsAccessor.Get(string.Empty);
        Assert.True(httpClientOptions.JsonSerializerOptions.IncludeFields);

        var httpClientOptions2 = httpClientOptionsAccessor.Get("github");
        Assert.True(httpClientOptions2.JsonSerializerOptions.IncludeFields);

        var httpClientOptions3 = httpClientOptionsAccessor.Get("notfound");
        Assert.Null(httpClientOptions3.JsonSerializerOptions);

        serviceProvider.Dispose();
    }

    [Fact]
    public void IsXmlContent_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = new StringContent("""{"id":10, "name":"furion"}""", Encoding.UTF8,
            new MediaTypeHeaderValue("application/json"));

        httpResponseMessage.Content = new StringContent("""
                                                        <XmlModel>
                                                           <Name>Furion</Name>
                                                           <Age>30</Age>
                                                        </XmlModel>
                                                        """, Encoding.UTF8,
            new MediaTypeHeaderValue("application/xml"));
        Assert.True(httpResponseMessage.IsXmlContent());

        httpResponseMessage.Content = new StringContent("""
                                                        <XmlModel>
                                                           <Name>Furion</Name>
                                                           <Age>30</Age>
                                                        </XmlModel>
                                                        """, Encoding.UTF8,
            new MediaTypeHeaderValue("text/xml"));
        Assert.True(httpResponseMessage.IsXmlContent());

        httpResponseMessage.Content = new StringContent("""
                                                        <XmlModel>
                                                           <Name>Furion</Name>
                                                           <Age>30</Age>
                                                        </XmlModel>
                                                        """, Encoding.UTF8,
            new MediaTypeHeaderValue("application/xml-patch+xml"));
        Assert.True(httpResponseMessage.IsXmlContent());

        httpResponseMessage.Content = new StringContent("""
                                                        {"id":1,"name":"Furion"}
                                                        """, Encoding.UTF8,
            new MediaTypeHeaderValue("application/json"));
        Assert.False(httpResponseMessage.IsXmlContent());
    }

    [Fact]
    public void ResolveHttpClientName_ReturnOK()
    {
        Assert.Null(((HttpResponseMessage?)null).ResolveHttpClientName());
        Assert.Null(new HttpResponseMessage().ResolveHttpClientName());

        var httpResponseMessage = new HttpResponseMessage();
        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Options.AddOrUpdate(Constants.HTTP_CLIENT_NAME, "Github");
        httpResponseMessage.RequestMessage = httpRequestMessage;

        Assert.Equal("Github", httpResponseMessage.ResolveHttpClientName());
    }

    [Fact]
    public void ToJsonString_ReturnOK()
    {
        Assert.Equal("null", HttpRemoteExtensions.ToJsonString(null));
        Assert.Equal("{\"id\":1,\"name\":\"百小僧\",\"age\":30}",
            new { Id = 1, Name = "百小僧", Age = 30 }.ToJsonString());
        Assert.Equal("{\"Id\":1,\"Name\":\"\\u767E\\u5C0F\\u50E7\",\"Age\":30}",
            new { Id = 1, Name = "百小僧", Age = 30 }.ToJsonString(JsonSerializerOptions.Default));
    }

    [Fact]
    public void FixInvalidCharset_ReturnOK()
    {
        ((HttpResponseMessage?)null).FixInvalidCharset();
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content.FixInvalidCharset();

        httpResponseMessage.Content.Headers.ContentType =
            new MediaTypeHeaderValue("application/json") { CharSet = "utf8" };
        httpResponseMessage.FixInvalidCharset();

        Assert.Equal("utf-8", httpResponseMessage.Content.Headers.ContentType?.CharSet);

        httpResponseMessage.Content.Headers.ContentType =
            new MediaTypeHeaderValue("application/json") { CharSet = "utf 8" };
        httpResponseMessage.FixInvalidCharset();

        Assert.Equal("utf-8", httpResponseMessage.Content.Headers.ContentType?.CharSet);

        httpResponseMessage.Content.Headers.ContentType =
            new MediaTypeHeaderValue("application/json") { CharSet = "utf-8;" };
        httpResponseMessage.FixInvalidCharset();

        Assert.Equal("utf-8", httpResponseMessage.Content.Headers.ContentType?.CharSet);

        httpResponseMessage.Content.Headers.ContentType =
            new MediaTypeHeaderValue("application/json") { CharSet = "UTF8" };
        httpResponseMessage.FixInvalidCharset();

        Assert.Equal("utf-8", httpResponseMessage.Content.Headers.ContentType?.CharSet);

        httpResponseMessage.Content.Headers.ContentType =
            new MediaTypeHeaderValue("application/json") { CharSet = "utf" };
        httpResponseMessage.FixInvalidCharset();

        Assert.Equal("utf-8", httpResponseMessage.Content.Headers.ContentType?.CharSet);
    }

    [Fact]
    public void ShouldUseJsonResponseWrapper_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();
        Assert.False(httpResponseMessage.ShouldUseJsonResponseWrapper(null));

        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Options.AddOrUpdate(Constants.ENABLE_JSON_RESPONSE_WRAPPER_KEY, "TRUE");
        httpResponseMessage.RequestMessage = httpRequestMessage;
        Assert.True(httpResponseMessage.ShouldUseJsonResponseWrapper(null));

        httpRequestMessage.Options.AddOrUpdate(Constants.ENABLE_JSON_RESPONSE_WRAPPER_KEY, "FALSE");
        httpResponseMessage.RequestMessage = httpRequestMessage;
        Assert.False(httpResponseMessage.ShouldUseJsonResponseWrapper(null));

        var services = new ServiceCollection();
        services.AddHttpClient(string.Empty).ConfigureOptions(u => u.UseJsonResponseWrapper = true);
        using var serviceProvider = services.BuildServiceProvider();

        var httpResponseMessage2 = new HttpResponseMessage();
        Assert.True(httpResponseMessage2.ShouldUseJsonResponseWrapper(serviceProvider));

        var httpRequestMessage2 = new HttpRequestMessage();
        httpRequestMessage2.Options.AddOrUpdate(Constants.ENABLE_JSON_RESPONSE_WRAPPER_KEY, "TRUE");
        httpResponseMessage2.RequestMessage = httpRequestMessage2;
        Assert.True(httpResponseMessage2.ShouldUseJsonResponseWrapper(serviceProvider));

        httpRequestMessage2.Options.AddOrUpdate(Constants.ENABLE_JSON_RESPONSE_WRAPPER_KEY, "FALSE");
        httpResponseMessage2.RequestMessage = httpRequestMessage2;
        Assert.False(httpResponseMessage2.ShouldUseJsonResponseWrapper(serviceProvider));
    }

    [Fact]
    public void ShouldJsonResponseStringUnwrap_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();
        Assert.False(httpResponseMessage.ShouldJsonResponseStringUnwrap());

        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Options.AddOrUpdate(Constants.ENABLE_JSON_RESPONSE_STRING_UNWRAP_KEY, "TRUE");
        httpResponseMessage.RequestMessage = httpRequestMessage;
        Assert.True(httpResponseMessage.ShouldJsonResponseStringUnwrap());

        httpRequestMessage.Options.AddOrUpdate(Constants.ENABLE_JSON_RESPONSE_STRING_UNWRAP_KEY, "FALSE");
        httpResponseMessage.RequestMessage = httpRequestMessage;
        Assert.False(httpResponseMessage.ShouldJsonResponseStringUnwrap());
    }

    [Fact]
    public async Task ReadAndDecompressFirstBytesAsync_ReturnOK()
    {
        const string originalText = "测试数据 DecompressAsync @2026 #UTF8 中文/English/123";
        var originalBytes = Encoding.UTF8.GetBytes(originalText);
        var originalLength = originalBytes.Length;

        using var gzipCompressedStream = new MemoryStream();
        await using (var gzip = new GZipStream(gzipCompressedStream, CompressionMode.Compress, true))
        {
            await gzip.WriteAsync(originalBytes, TestContext.Current.CancellationToken);
        }

        gzipCompressedStream.Position = 0;

        var (gzipBuffer, gzipRead, gzipTruncated) = await HttpRemoteExtensions.ReadAndDecompressFirstBytesAsync(
            gzipCompressedStream, "gzip", originalLength + 100, CancellationToken.None);

        var gzipResult = Encoding.UTF8.GetString(gzipBuffer, 0, gzipRead);
        Assert.Equal(originalText, gzipResult);
        Assert.Equal(originalLength, gzipRead);
        Assert.False(gzipTruncated);

        using var deflateCompressedStream = new MemoryStream();
        await using (var deflate = new DeflateStream(deflateCompressedStream, CompressionMode.Compress, true))
        {
            await deflate.WriteAsync(originalBytes, TestContext.Current.CancellationToken);
        }

        deflateCompressedStream.Position = 0;

        var (deflateBuffer, deflateRead, deflateTruncated) =
            await HttpRemoteExtensions.ReadAndDecompressFirstBytesAsync(
                deflateCompressedStream, "deflate", originalLength + 100, CancellationToken.None);

        var deflateResult = Encoding.UTF8.GetString(deflateBuffer, 0, deflateRead);
        Assert.Equal(originalText, deflateResult);
        Assert.Equal(originalLength, deflateRead);
        Assert.False(deflateTruncated);

        using var brCompressedStream = new MemoryStream();
        await using (var brotli = new BrotliStream(brCompressedStream, CompressionMode.Compress, true))
        {
            await brotli.WriteAsync(originalBytes, TestContext.Current.CancellationToken);
        }

        brCompressedStream.Position = 0;

        var (brBuffer, brRead, brTruncated) = await HttpRemoteExtensions.ReadAndDecompressFirstBytesAsync(
            brCompressedStream, "br", originalLength + 100, CancellationToken.None);

        var brResult = Encoding.UTF8.GetString(brBuffer, 0, brRead);
        Assert.Equal(originalText, brResult);
        Assert.Equal(originalLength, brRead);
        Assert.False(brTruncated);

        using var unknownStream = new MemoryStream(originalBytes);
        unknownStream.Position = 0;

        var (unknownBuffer, unknownRead, unknownTruncated) =
            await HttpRemoteExtensions.ReadAndDecompressFirstBytesAsync(
                unknownStream, "unknown", originalLength + 100, CancellationToken.None);

        var unknownResult = Encoding.UTF8.GetString(unknownBuffer, 0, unknownRead);
        Assert.Equal(originalText, unknownResult);
        Assert.Equal(originalLength, unknownRead);
        Assert.False(unknownTruncated);

        using var nullEncodingStream = new MemoryStream(originalBytes);
        nullEncodingStream.Position = 0;

        var (nullEncBuffer, nullEncRead, nullEncTruncated) =
            await HttpRemoteExtensions.ReadAndDecompressFirstBytesAsync(
                nullEncodingStream, null, originalLength + 100, CancellationToken.None);

        var nullEncResult = Encoding.UTF8.GetString(nullEncBuffer, 0, nullEncRead);
        Assert.Equal(originalText, nullEncResult);
        Assert.Equal(originalLength, nullEncRead);
        Assert.False(nullEncTruncated);
    }

    [Fact]
    public async Task ReadAndDecompressFirstBytesAsync_Truncated_ReturnPartialContent()
    {
        const string originalText = "这是一个较长的测试文本，用于验证截断行为。";
        var originalBytes = Encoding.UTF8.GetBytes(originalText);

        using var compressedStream = new MemoryStream();
        await using (var gzip = new GZipStream(compressedStream, CompressionMode.Compress, true))
        {
            await gzip.WriteAsync(originalBytes, TestContext.Current.CancellationToken);
        }

        compressedStream.Position = 0;

        const int maxBytes = 10;
        var (buffer, totalRead, isTruncated) = await HttpRemoteExtensions.ReadAndDecompressFirstBytesAsync(
            compressedStream, "gzip", maxBytes, CancellationToken.None);

        Assert.Equal(maxBytes, totalRead);
        Assert.True(isTruncated);

        var expectedPrefix = Encoding.UTF8.GetBytes(originalText)[..maxBytes];
        var actualPrefix = buffer[..totalRead];
        Assert.Equal(expectedPrefix, actualPrefix);
    }

    [Fact]
    public async Task ReadAndDecompressFirstBytesAsync_EmptyContent_ReturnEmpty()
    {
        using var emptyStream = new MemoryStream();
        await using (_ = new GZipStream(emptyStream, CompressionMode.Compress, true))
        {
        }

        emptyStream.Position = 0;

        var (buffer, totalRead, isTruncated) = await HttpRemoteExtensions.ReadAndDecompressFirstBytesAsync(
            emptyStream, "gzip", 1024, CancellationToken.None);

        Assert.Equal(0, totalRead);
        Assert.False(isTruncated);
        Assert.Empty(buffer);
    }

    [Fact]
    public async Task ReadAndUnwrapFromJsonAsync_ReturnOK()
    {
        using var stringContent = new StringContent("\"{\\\"id\\\":10, \\\"name\\\":\\\"furion\\\"}\"");

        var objectModel = await stringContent.ReadAndUnwrapFromJsonAsync(typeof(JsonModel),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            TestContext.Current.CancellationToken) as JsonModel;
        Assert.NotNull(objectModel);
        Assert.Equal(10, objectModel.Id);
        Assert.Equal("furion", objectModel.Name);
    }

    [Fact]
    public void GetResponseMessage_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpRemoteExtensions.GetResponseMessage(null!));

    [Fact]
    public void GetResponseMessage_ReturnOK()
    {
        var exception = new Exception("出错了");
        Assert.Null(exception.GetResponseMessage());

        exception.Data[nameof(HttpResponseMessage)] = new HttpResponseMessage();
        Assert.NotNull(exception.GetResponseMessage());
    }

    [Fact]
    public void GetRequestDuration_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpRemoteExtensions.GetRequestDuration(null!));

    [Fact]
    public void GetRequestDuration_ReturnOK()
    {
        var exception = new Exception("出错了");
        Assert.Null(exception.GetRequestDuration());

        exception.Data[nameof(HttpRequestPipelineContext.RequestDuration)] = 100L;
        Assert.Equal(100L, exception.GetRequestDuration());
    }

    private static byte[] GzipCompress(byte[] data)
    {
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionMode.Compress, true))
        {
            gzip.Write(data, 0, data.Length);
        }

        return output.ToArray();
    }

    public class JsonModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    internal class NoLengthStreamContent : HttpContent
    {
        private readonly byte[] _data;
        public NoLengthStreamContent(byte[] data) => _data = data;

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
            => stream.WriteAsync(_data, 0, _data.Length);

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }

    internal class LargeContentWithoutLength : HttpContent
    {
        private readonly int _sizeInBytes;
        public LargeContentWithoutLength(int sizeInBytes) => _sizeInBytes = sizeInBytes;

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            var buffer = new byte[8192];
            var remaining = _sizeInBytes;
            while (remaining > 0)
            {
                var toWrite = Math.Min(remaining, buffer.Length);
                await stream.WriteAsync(buffer.AsMemory(0, toWrite));
                remaining -= toWrite;
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }

    internal class MockLargeContent : HttpContent
    {
        private readonly long _length;
        public MockLargeContent(long length) => _length = length;

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) => Task.CompletedTask;

        protected override bool TryComputeLength(out long length)
        {
            length = _length;
            return true;
        }
    }

    internal class NonSeekableStream : MemoryStream
    {
        public override bool CanSeek => false;
    }
}