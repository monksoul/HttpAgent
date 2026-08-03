// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ETagPipelineHandlerTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var services = new ServiceCollection();
        services.TryAddSingleton<IHttpETagCache, MemoryETagCache>();
        using var serviceProvider = services.BuildServiceProvider();

        var handler = new ETagPipelineHandler(serviceProvider.GetRequiredService<IHttpETagCache>());

        Assert.NotNull(handler);
    }

    [Fact]
    public void GenerateCacheKey_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => ETagPipelineHandler.GenerateCacheKey(null!));

    [Fact]
    public void GenerateCacheKey_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://furion.net");
        Assert.Equal("GET:https://furion.net/", ETagPipelineHandler.GenerateCacheKey(httpRequestMessage));
    }

    [Fact]
    public async Task CacheResponseAsync_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        services.TryAddSingleton<IHttpETagCache, MemoryETagCache>();
        await using var serviceProvider = services.BuildServiceProvider();

        var handler = new ETagPipelineHandler(serviceProvider.GetRequiredService<IHttpETagCache>());

        await Assert.ThrowsAsync<ArgumentNullException>(() => handler.CacheResponseAsync(null!, null!, null!));
        await Assert.ThrowsAsync<ArgumentException>(() => handler.CacheResponseAsync(string.Empty, null!, null!));
        await Assert.ThrowsAsync<ArgumentException>(() => handler.CacheResponseAsync(" ", null!, null!));

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.CacheResponseAsync("GET:https://furion.net/", null!, null!));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.CacheResponseAsync("GET:https://furion.net/", new HttpResponseMessage(), null!));
    }

    [Fact]
    public async Task CacheResponseAsync_ReturnOK()
    {
        var services = new ServiceCollection();
        services.TryAddSingleton<IHttpETagCache, MemoryETagCache>();
        await using var serviceProvider = services.BuildServiceProvider();

        var cache = serviceProvider.GetRequiredService<IHttpETagCache>();
        var handler = new ETagPipelineHandler(cache);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "Furion");
        httpResponseMessage.Content =
            new StringContent("让 .NET 开发更简单，更通用，更流行。", Encoding.UTF8, MediaTypeNames.Text.Plain);
        httpResponseMessage.Headers.ETag = new EntityTagHeaderValue("\"675af34563dc-tr34\"");

        await handler.CacheResponseAsync("GET:https://furion.net/", httpResponseMessage,
            httpResponseMessage.Headers.ETag);

        var memoryCache = cache as MemoryETagCache;
        Assert.NotNull(memoryCache);
        Assert.Single(memoryCache._cache);
        var item = memoryCache._cache.First().Value;
        Assert.Equal("675af34563dc-tr34", item.ETag);
        Assert.Equal(HttpStatusCode.OK, item.StatusCode);
        Assert.NotNull(item.ContentBytes);
        Assert.Equal("让 .NET 开发更简单，更通用，更流行。", Encoding.UTF8.GetString(item.ContentBytes));
        Assert.NotNull(item.ContentHeaders);
        Assert.Equal(2, item.ContentHeaders.Count);
        Assert.Equal("text/plain; charset=utf-8", item.ContentHeaders["Content-Type"].First());
        Assert.Equal("51", item.ContentHeaders["Content-Length"].First());
        Assert.NotNull(item.ResponseHeaders);
        Assert.Equal(2, item.ResponseHeaders.Count);
        Assert.Contains(item.ResponseHeaders, h => h.Key == "framework" && h.Value.Contains("Furion"));
        Assert.Contains(item.ResponseHeaders, h => h.Key == "ETag" && h.Value.Contains("\"675af34563dc-tr34\""));
        Assert.Equal("OK", item.ReasonPhrase);
    }

    [Fact]
    public void BuildResponseFromCacheItem_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => ETagPipelineHandler.BuildResponseFromCacheItem(null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            ETagPipelineHandler.BuildResponseFromCacheItem(new HttpETagCacheItem(), null!));
    }

    [Fact]
    public void BuildResponseFromCacheItem_ReturnOK()
    {
        var services = new ServiceCollection();
        services.TryAddSingleton<IHttpETagCache, MemoryETagCache>();
        using var serviceProvider = services.BuildServiceProvider();

        var cache = serviceProvider.GetRequiredService<IHttpETagCache>();
        var handler = new ETagPipelineHandler(cache);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "Furion");
        httpResponseMessage.Content =
            new StringContent("让 .NET 开发更简单，更通用，更流行。", Encoding.UTF8, MediaTypeNames.Text.Plain);
        httpResponseMessage.Headers.ETag = new EntityTagHeaderValue("\"675af34563dc-tr34\"");

        AsyncUtility.RunSync(() => handler.CacheResponseAsync("GET:https://furion.net/", httpResponseMessage,
            httpResponseMessage.Headers.ETag));

        var memoryCache = cache as MemoryETagCache;
        Assert.NotNull(memoryCache);
        Assert.Single(memoryCache._cache);
        var item = memoryCache._cache.First().Value;

        var newHttpResponseMessage =
            ETagPipelineHandler.BuildResponseFromCacheItem(item,
                new HttpRequestMessage(HttpMethod.Get, "https://furion.net/"));
        Assert.NotNull(newHttpResponseMessage);
        Assert.Equal(HttpStatusCode.OK, newHttpResponseMessage.StatusCode);
        Assert.NotNull(newHttpResponseMessage.Content);
        Assert.Equal("让 .NET 开发更简单，更通用，更流行。",
            AsyncUtility.RunSync(() =>
                newHttpResponseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)));
        Assert.Equal("OK", newHttpResponseMessage.ReasonPhrase);
        Assert.Equal("text/plain; charset=utf-8", newHttpResponseMessage.Content.Headers.ContentType?.ToString());
        Assert.Equal("Furion", newHttpResponseMessage.Headers.GetValues("framework").First());
        Assert.NotNull(newHttpResponseMessage.RequestMessage);
        Assert.True(
            newHttpResponseMessage.RequestMessage.Options.TryGetValue(
                new HttpRequestOptionsKey<bool>(Constants.ETAG_CACHED_KEY), out var value));
        Assert.True(value);
    }
}