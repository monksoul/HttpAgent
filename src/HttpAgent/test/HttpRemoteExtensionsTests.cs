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
        Assert.Equal("gzip, deflate, br", httpClient.DefaultRequestHeaders.AcceptEncoding.ToString());
        Assert.False(httpClient.DefaultRequestHeaders.ConnectionClose);
    }

    [Fact]
    public void ProfilerHeaders_HttpRequestMessage_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        Assert.Equal(
            "Request Headers: \r\n\tAccept:              application/json\r\n\tAccept-Encoding:     gzip, deflate",
            httpRequestMessage.ProfilerHeaders());
        Assert.Equal("Accept:              application/json\r\nAccept-Encoding:     gzip, deflate",
            httpRequestMessage.ProfilerHeaders(null));
    }

    [Fact]
    public void ProfilerHeaders_HttpRequestMessage_WithContent_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Content = new StringContent("Furion", Encoding.UTF8, "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        Assert.Equal(
            "Request Headers: \r\n\tAccept:              application/json\r\n\tAccept-Encoding:     gzip, deflate\r\n\tContent-Type:        application/json; charset=utf-8",
            httpRequestMessage.ProfilerHeaders());
        Assert.Equal(
            "Accept:              application/json\r\nAccept-Encoding:     gzip, deflate\r\nContent-Type:        application/json; charset=utf-8",
            httpRequestMessage.ProfilerHeaders(null));
    }

    [Fact]
    public void ProfilerHeaders_HttpResponseMessage_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
        httpResponseMessage.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json");

        Assert.Equal(
            "Response Headers: \r\n\tAccept:              application/json\r\n\tAccept-Encoding:     gzip, deflate\r\n\tContent-Type:        application/json",
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
            "General: \r\n\tRequest URL:      http://localhost\r\n\tHTTP Method:      GET\r\n\tStatus Code:      200 OK\r\n\tHTTP Version:     1.1\r\n\tHTTP Content:     \r\nResponse Headers: \r\n\tAccept:              application/json\r\n\tAccept-Encoding:     gzip, deflate\r\n\tContent-Type:        application/json",
            httpResponseMessage.ProfilerGeneralAndHeaders());

        Assert.Equal(
            "General: \r\n\tRequest URL:          http://localhost\r\n\tHTTP Method:          GET\r\n\tStatus Code:          200 OK\r\n\tHTTP Version:         1.1\r\n\tHTTP Content:         \r\n\tRequest Duration:     200ms\r\nResponse Headers: \r\n\tAccept:              application/json\r\n\tAccept-Encoding:     gzip, deflate\r\n\tContent-Type:        application/json",
            httpResponseMessage.ProfilerGeneralAndHeaders(generalCustomKeyValues:
                [new KeyValuePair<string, IEnumerable<string>>("Request Duration", ["200ms"])]));
    }

    [Fact]
    public async Task LogHttpContentAsync_ReturnOK()
    {
        Assert.Null(await HttpRemoteExtensions.ProfilerAsync(null));

        var stringContent = new StringContent("Hello World");
        Assert.Equal("Request Body (StringContent, total: 11 bytes): \r\n\tHello World",
            await stringContent.ProfilerAsync());

        var jsonContent = JsonContent.Create(new { id = 1, name = "furion" });
        Assert.Equal("Request Body (JsonContent, total: 24 bytes): \r\n\t{\"id\":1,\"name\":\"furion\"}",
            await jsonContent.ProfilerAsync());

        var byteArrayContent = new ByteArrayContent("Hello World"u8.ToArray());
        Assert.Equal("Request Body (ByteArrayContent, total: 11 bytes): \r\n\tHello World",
            await byteArrayContent.ProfilerAsync());

        var formUrlEncodedContent = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("id", "1"), new KeyValuePair<string, string>("name", "Furion")
        ]);
        Assert.Equal("Request Body (FormUrlEncodedContent, total: 16 bytes): \r\n\tid=1&name=Furion",
            await formUrlEncodedContent.ProfilerAsync());

        var streamStream = new StreamContent(File.OpenRead(Path.Combine(AppContext.BaseDirectory, "test.txt")));
        Assert.Equal("Request Body (StreamContent, total: 21 bytes): \r\n\t﻿测试文件内容",
            await streamStream.ProfilerAsync());

        var readOnlyMemoryContent = new ReadOnlyMemoryContent(new ReadOnlyMemory<byte>("Hello World"u8.ToArray()));
        Assert.Equal("Request Body (ReadOnlyMemoryContent, total: 11 bytes): \r\n\tHello World",
            await readOnlyMemoryContent.ProfilerAsync());

        var multipartFormDataContent = new MultipartFormDataContent("--------------------------");
        multipartFormDataContent.Add(new StringContent("Hello World"), "text");
        multipartFormDataContent.Add(
            new StreamContent(File.OpenRead(Path.Combine(AppContext.BaseDirectory, "test.txt"))), "file");
        Assert.Equal(
            "Request Body (MultipartFormDataContent, total: 259 bytes): \r\n\t----------------------------\r\n  Content-Type: text/plain; charset=utf-8\r\n  Content-Disposition: form-data; name=text\r\n  \r\n  Hello World\r\n  ----------------------------\r\n  Content-Disposition: form-data; name=file\r\n  \r\n  ﻿测试文件内容\r\n  ------------------------------\r\n  ",
            await multipartFormDataContent.ProfilerAsync());

        var stringContent2 = new StringContent("Hello World");
        Assert.Equal("Response Body (StringContent, total: 11 bytes): \r\n\tHello World",
            await stringContent2.ProfilerAsync("Response Body"));
    }

    [Fact]
    public async Task CloneAsync_Invalid_Parameters() =>
        await Assert.ThrowsAsync<ArgumentNullException>(() => HttpRemoteExtensions.CloneAsync(null!));

    [Fact]
    public async Task CloneAsync_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");
        httpRequestMessage.Headers.TryAddWithoutValidation("User-Agent", "furion");
        var stringContent = new StringContent("Hello World", Encoding.UTF8, "application/json");
        httpRequestMessage.Content = stringContent;

        var clonedHttpRequestMessage = await httpRequestMessage.CloneAsync();
        Assert.Equal("furion", clonedHttpRequestMessage.Headers.UserAgent.ToString());

        var streamContent = clonedHttpRequestMessage.Content as StreamContent;
        Assert.NotNull(streamContent);
        var str = await streamContent.ReadAsStringAsync();
        Assert.Equal("Hello World", str);

        Assert.Equal("application/json", clonedHttpRequestMessage.Content?.Headers.ContentType?.MediaType);
    }

    [Fact]
    public void Clone_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpRemoteExtensions.Clone(null!));

    [Fact]
    public void Clone_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost");
        httpRequestMessage.Headers.TryAddWithoutValidation("User-Agent", "furion");
        var stringContent = new StringContent("Hello World", Encoding.UTF8, "application/json");
        httpRequestMessage.Content = stringContent;

        var clonedHttpRequestMessage = httpRequestMessage.Clone();
        Assert.Equal("furion", clonedHttpRequestMessage.Headers.UserAgent.ToString());

        var streamContent = clonedHttpRequestMessage.Content as StreamContent;
        Assert.NotNull(streamContent);
#pragma warning disable xUnit1031
        var str = streamContent.ReadAsStringAsync().GetAwaiter().GetResult();
#pragma warning restore xUnit1031
        Assert.Equal("Hello World", str);

        Assert.Equal("application/json", clonedHttpRequestMessage.Content?.Headers.ContentType?.MediaType);
    }

    [Fact]
    public void TryGetSetCookies_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            // ReSharper disable once InvokeAsExtensionMethod
            HttpRemoteExtensions.TryGetSetCookies((HttpResponseMessage)null!, out _, out _));
        Assert.Throws<ArgumentNullException>(() =>
            // ReSharper disable once InvokeAsExtensionMethod
            HttpRemoteExtensions.TryGetSetCookies((HttpResponseHeaders)null!, out _, out _));
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
        Assert.Equal<StringSegment>("baidu.com", cookies.Domain);
        Assert.Equal<StringSegment>("/", cookies.Path);
        Assert.Equal("2034/9/1 2:22:19 +00:00", cookies.Expires.ToString());
        Assert.Equal(TimeSpan.FromSeconds(315360000), cookies.MaxAge);
        Assert.True(cookies.HttpOnly);
        Assert.True(cookies.Secure);
        Assert.Equal(SameSiteMode.None, cookies.SameSite);
        Assert.Equal<StringSegment>("BDUSS_BFESS", cookies.Name);
        Assert.Equal<StringSegment>(
            "hBSH5yRDI1a0Fzb2lMWllDYk0tRkZ0UEc2OW1URjBvLUtVckNMeFUyaUNxdWxtRVFBQUFBJCQAAAAAAAAAAAEAAADeGZbRsNnHqc34xcwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIIdwmaCHcJmUm",
            cookies.Value);

        // ===============

        httpResponseMessage.TryGetSetCookies(out var setCookies3, out var rawSetCookies3);

        Assert.Null(setCookies3);
        Assert.Null(rawSetCookies3);

        httpResponseMessage2.TryGetSetCookies(out var setCookies4, out var rawSetCookies4);

        Assert.NotNull(rawSetCookies4);
        Assert.NotNull(setCookies4);
        Assert.Equal(setCookieHeader, rawSetCookies4.First());
        Assert.Single(setCookies4);

        var cookies2 = setCookies4.First();
        Assert.Equal<StringSegment>("baidu.com", cookies2.Domain);
        Assert.Equal<StringSegment>("/", cookies2.Path);
        Assert.Equal("2034/9/1 2:22:19 +00:00", cookies2.Expires.ToString());
        Assert.Equal(TimeSpan.FromSeconds(315360000), cookies2.MaxAge);
        Assert.True(cookies2.HttpOnly);
        Assert.True(cookies2.Secure);
        Assert.Equal(SameSiteMode.None, cookies2.SameSite);
        Assert.Equal<StringSegment>("BDUSS_BFESS", cookies2.Name);
        Assert.Equal<StringSegment>(
            "hBSH5yRDI1a0Fzb2lMWllDYk0tRkZ0UEc2OW1URjBvLUtVckNMeFUyaUNxdWxtRVFBQUFBJCQAAAAAAAAAAAEAAADeGZbRsNnHqc34xcwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIIdwmaCHcJmUm",
            cookies2.Value);
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
        services.AddHttpClient("github").ConfigureOptions((options, serviceProvider) =>
        {
            Assert.NotNull(serviceProvider);
            options.JsonSerializerOptions.IncludeFields = true;
        });

        var serviceProvider = services.BuildServiceProvider();

        var httpClientOptionsAccessor = serviceProvider.GetRequiredService<IOptionsSnapshot<HttpClientOptions>>();

        var httpClientOptions = httpClientOptionsAccessor.Get(string.Empty);
        Assert.True(httpClientOptions.JsonSerializerOptions.IncludeFields);
        Assert.False(httpClientOptions.IsDefault);

        var httpClientOptions2 = httpClientOptionsAccessor.Get("github");
        Assert.True(httpClientOptions2.JsonSerializerOptions.IncludeFields);
        Assert.False(httpClientOptions2.IsDefault);

        var httpClientOptions3 = httpClientOptionsAccessor.Get("notfound");
        Assert.False(httpClientOptions3.JsonSerializerOptions.IncludeFields);
        Assert.True(httpClientOptions3.IsDefault);

        serviceProvider.Dispose();
    }
}