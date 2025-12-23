// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpDeclarativeBuilderTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new HttpDeclarativeBuilder(null!, null!));

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method1));
        Assert.Throws<ArgumentNullException>(() => new HttpDeclarativeBuilder(method!, null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method1));
        var builder = new HttpDeclarativeBuilder(method!, []);

        Assert.NotNull(builder.Method);
        Assert.NotNull(builder.Args);
        Assert.False(builder._hasLoadedExtractors);

        Dictionary<Type, IHttpDeclarativeExtractor> extractors = new()
        {
            { typeof(BaseAddressDeclarativeExtractor), new BaseAddressDeclarativeExtractor() },
            { typeof(ValidationDeclarativeExtractor), new ValidationDeclarativeExtractor() },
            {
                typeof(PerformanceOptimizationDeclarativeExtractor),
                new PerformanceOptimizationDeclarativeExtractor()
            },
            { typeof(AutoSetHostHeaderDeclarativeExtractor), new AutoSetHostHeaderDeclarativeExtractor() },
            { typeof(HttpClientNameDeclarativeExtractor), new HttpClientNameDeclarativeExtractor() },
            { typeof(TraceIdentifierDeclarativeExtractor), new TraceIdentifierDeclarativeExtractor() },
            { typeof(ProfilerDeclarativeExtractor), new ProfilerDeclarativeExtractor() },
            { typeof(SimulateBrowserDeclarativeExtractor), new SimulateBrowserDeclarativeExtractor() },
            { typeof(AcceptLanguageDeclarativeExtractor), new AcceptLanguageDeclarativeExtractor() },
            { typeof(DisableCacheDeclarativeExtractor), new DisableCacheDeclarativeExtractor() },
            {
                typeof(EnsureSuccessStatusCodeDeclarativeExtractor),
                new EnsureSuccessStatusCodeDeclarativeExtractor()
            },
            { typeof(TimeoutDeclarativeExtractor), new TimeoutDeclarativeExtractor() },
            { typeof(PathSegmentDeclarativeExtractor), new PathSegmentDeclarativeExtractor() },
            { typeof(QueryDeclarativeExtractor), new QueryDeclarativeExtractor() },
            { typeof(PathDeclarativeExtractor), new PathDeclarativeExtractor() },
            { typeof(CookieDeclarativeExtractor), new CookieDeclarativeExtractor() },
            { typeof(RefererDeclarativeExtractor), new RefererDeclarativeExtractor() },
            { typeof(HeaderDeclarativeExtractor), new HeaderDeclarativeExtractor() },
            { typeof(PropertyDeclarativeExtractor), new PropertyDeclarativeExtractor() },
            { typeof(HttpVersionDeclarativeExtractor), new HttpVersionDeclarativeExtractor() },
            { typeof(SuppressExceptionsDeclarativeExtractor), new SuppressExceptionsDeclarativeExtractor() },
            { typeof(RequestEventHandlerDeclarativeExtractor), new RequestEventHandlerDeclarativeExtractor() },
            { typeof(JsonResponseWrappingDeclarativeExtractor), new JsonResponseWrappingDeclarativeExtractor() },
            { typeof(BodyDeclarativeExtractor), new BodyDeclarativeExtractor() }
        };

        Assert.True(extractors.Keys.ToHashSet().SetEquals(HttpDeclarativeBuilder._extractors.Keys));

        Dictionary<Type, IHttpDeclarativeExtractor> freezeExtractors = new()
        {
            { typeof(MultipartDeclarativeExtractor), new MultipartDeclarativeExtractor() },
            {
                typeof(HttpMultipartFormDataBuilderDeclarativeExtractor),
                new HttpMultipartFormDataBuilderDeclarativeExtractor()
            },
            { typeof(HttpRequestBuilderDeclarativeExtractor), new HttpRequestBuilderDeclarativeExtractor() }
        };

        Assert.True(freezeExtractors.Keys.ToHashSet().SetEquals(HttpDeclarativeBuilder._frozenExtractors.Keys));
    }

    [Fact]
    public void Build_Invalid_Parameters()
    {
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.InvalidMethod));
        var builder = new HttpDeclarativeBuilder(method!, []);

        Assert.Throws<ArgumentNullException>(() => builder.Build(null!));

        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build(new HttpRemoteOptions()));
        Assert.Equal(
            $"No `[HttpMethod]` annotation was found in method `System.Threading.Tasks.Task InvalidMethod()` of type `{typeof(IHttpDeclarativeTest).ToFriendlyString()}`.",
            exception.Message);
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method1))!;
        var builder = new HttpDeclarativeBuilder(method, []);

        var httpRequestBuilder = builder.Build(new HttpRemoteOptions());
        Assert.Equal(HttpMethod.Get, httpRequestBuilder.HttpMethod);
        Assert.Equal("https://furion.net/", httpRequestBuilder.RequestUri?.ToString());
        Assert.True(builder._hasLoadedExtractors);

        Assert.Equal(
            $"System.Threading.Tasks.Task<System.String> Method1() | {method.DeclaringType.ToFriendlyString()}",
            httpRequestBuilder.Properties["__DECLARATIVE_METHOD__"]);
    }

    [Fact]
    public void Build_WithExtractors_ReturnOK()
    {
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method1));
        var builder = new HttpDeclarativeBuilder(method!, []);

        var httpRequestBuilder = builder.Build(new HttpRemoteOptions
        {
            HttpDeclarativeExtractors = [() => [new CustomHttpDeclarativeExtractor()]]
        });
        Assert.Equal(HttpMethod.Get, httpRequestBuilder.HttpMethod);
        Assert.Equal("https://furion.net/", httpRequestBuilder.RequestUri?.ToString());
        Assert.True(builder._hasLoadedExtractors);

        Dictionary<Type, IHttpDeclarativeExtractor> extractors = new()
        {
            { typeof(BaseAddressDeclarativeExtractor), new BaseAddressDeclarativeExtractor() },
            { typeof(ValidationDeclarativeExtractor), new ValidationDeclarativeExtractor() },
            {
                typeof(PerformanceOptimizationDeclarativeExtractor),
                new PerformanceOptimizationDeclarativeExtractor()
            },
            { typeof(AutoSetHostHeaderDeclarativeExtractor), new AutoSetHostHeaderDeclarativeExtractor() },
            { typeof(HttpClientNameDeclarativeExtractor), new HttpClientNameDeclarativeExtractor() },
            { typeof(TraceIdentifierDeclarativeExtractor), new TraceIdentifierDeclarativeExtractor() },
            { typeof(ProfilerDeclarativeExtractor), new ProfilerDeclarativeExtractor() },
            { typeof(SimulateBrowserDeclarativeExtractor), new SimulateBrowserDeclarativeExtractor() },
            { typeof(AcceptLanguageDeclarativeExtractor), new AcceptLanguageDeclarativeExtractor() },
            { typeof(DisableCacheDeclarativeExtractor), new DisableCacheDeclarativeExtractor() },
            {
                typeof(EnsureSuccessStatusCodeDeclarativeExtractor),
                new EnsureSuccessStatusCodeDeclarativeExtractor()
            },
            { typeof(TimeoutDeclarativeExtractor), new TimeoutDeclarativeExtractor() },
            { typeof(PathSegmentDeclarativeExtractor), new PathSegmentDeclarativeExtractor() },
            { typeof(QueryDeclarativeExtractor), new QueryDeclarativeExtractor() },
            { typeof(PathDeclarativeExtractor), new PathDeclarativeExtractor() },
            { typeof(CookieDeclarativeExtractor), new CookieDeclarativeExtractor() },
            { typeof(RefererDeclarativeExtractor), new RefererDeclarativeExtractor() },
            { typeof(HeaderDeclarativeExtractor), new HeaderDeclarativeExtractor() },
            { typeof(PropertyDeclarativeExtractor), new PropertyDeclarativeExtractor() },
            { typeof(HttpVersionDeclarativeExtractor), new HttpVersionDeclarativeExtractor() },
            { typeof(SuppressExceptionsDeclarativeExtractor), new SuppressExceptionsDeclarativeExtractor() },
            { typeof(RequestEventHandlerDeclarativeExtractor), new RequestEventHandlerDeclarativeExtractor() },
            { typeof(JsonResponseWrappingDeclarativeExtractor), new JsonResponseWrappingDeclarativeExtractor() },
            { typeof(BodyDeclarativeExtractor), new BodyDeclarativeExtractor() },
            { typeof(CustomHttpDeclarativeExtractor), new CustomHttpDeclarativeExtractor() }
        };

        Assert.True(extractors.Keys.ToHashSet().SetEquals(HttpDeclarativeBuilder._extractors.Keys));

        Dictionary<Type, IHttpDeclarativeExtractor> freezeExtractors = new()
        {
            { typeof(MultipartDeclarativeExtractor), new MultipartDeclarativeExtractor() },
            {
                typeof(HttpMultipartFormDataBuilderDeclarativeExtractor),
                new HttpMultipartFormDataBuilderDeclarativeExtractor()
            },
            { typeof(HttpRequestBuilderDeclarativeExtractor), new HttpRequestBuilderDeclarativeExtractor() }
        };

        Assert.True(freezeExtractors.Keys.ToHashSet().SetEquals(HttpDeclarativeBuilder._frozenExtractors.Keys));
    }
}