// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpAssertionBuilderResponseAssertionsTests
{
    [Fact]
    public async Task ResponseStatusCode_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._responseAssertions);

        httpAssertionBuilder.ResponseStatusCode(200).ResponseStatusCode(HttpStatusCode.NoContent);
        Assert.Equal(2, httpAssertionBuilder._responseAssertions.Count);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var assertion = httpAssertionBuilder._responseAssertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response status code to be 200, but found 500.", exception.Message);

        var assertion1 = httpAssertionBuilder._responseAssertions[1];
        var exception2 =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion1(httpAssertionContext));
        Assert.Equal("Expected response status code to be 204, but found 500.", exception2.Message);
    }

    [Fact]
    public void ResponseStatusCodeIn_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.ResponseStatusCodeIn(null!));
        var exception = Assert.Throws<ArgumentException>(() => httpAssertionBuilder.ResponseStatusCodeIn());
        Assert.Equal("The allowed status codes array cannot be null or empty. (Parameter 'allowedStatusCodes')",
            exception.Message);
    }

    [Fact]
    public async Task ResponseStatusCodeIn_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._responseAssertions);

        httpAssertionBuilder.ResponseStatusCodeIn(200, 204);
        Assert.Single(httpAssertionBuilder._responseAssertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var assertion = httpAssertionBuilder._responseAssertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response status code to be one of [200, 204], but found 500.", exception.Message);
    }

    [Fact]
    public async Task ResponseIsSuccessStatusCode_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._responseAssertions);

        httpAssertionBuilder.ResponseIsSuccessStatusCode();
        Assert.Single(httpAssertionBuilder._responseAssertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var assertion = httpAssertionBuilder._responseAssertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response to be successful (2xx status code), but found status code 500.",
            exception.Message);
    }

    [Fact]
    public void ResponseContentContains_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() =>
            httpAssertionBuilder.ResponseContentContains(null!, TestContext.Current.CancellationToken));
        Assert.Throws<ArgumentException>(() =>
            httpAssertionBuilder.ResponseContentContains(string.Empty, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ResponseContentContains_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._responseAssertions);

        httpAssertionBuilder.ResponseContentContains("Hello", TestContext.Current.CancellationToken);
        Assert.Single(httpAssertionBuilder._responseAssertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Content = new StringContent("Hello World!");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var assertion = httpAssertionBuilder._responseAssertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.Content = new StringContent("Furion YYDS!");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response content to contain 'Hello', but it was not found.",
            exception.Message);
    }

    [Fact]
    public void ResponseContentEquals_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() =>
            httpAssertionBuilder.ResponseContentEquals(null!, TestContext.Current.CancellationToken));
        Assert.Throws<ArgumentException>(() =>
            httpAssertionBuilder.ResponseContentEquals(string.Empty, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ResponseContentEquals_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._responseAssertions);

        httpAssertionBuilder.ResponseContentEquals("Hello World!", TestContext.Current.CancellationToken);
        Assert.Single(httpAssertionBuilder._responseAssertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Content = new StringContent("Hello World!");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var assertion = httpAssertionBuilder._responseAssertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.Content = new StringContent("Furion YYDS!");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response content to be 'Hello World!', but found 'Furion YYDS!'.",
            exception.Message);
    }

    [Fact]
    public void ResponseContentMatches_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() =>
            httpAssertionBuilder.ResponseContentMatches(null!, TestContext.Current.CancellationToken));
        Assert.Throws<ArgumentException>(() =>
            httpAssertionBuilder.ResponseContentMatches(string.Empty, TestContext.Current.CancellationToken));
        Assert.Throws<ArgumentException>(() =>
            httpAssertionBuilder.ResponseContentMatches(" ", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ResponseContentMatches_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._responseAssertions);

        httpAssertionBuilder.ResponseContentMatches(@".ello\s*World!", TestContext.Current.CancellationToken);
        Assert.Single(httpAssertionBuilder._responseAssertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Content = new StringContent("Hello World!");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var assertion = httpAssertionBuilder._responseAssertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.Content = new StringContent("Furion YYDS!");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal(@"Expected response content to match regex '.ello\s*World!', but it did not.",
            exception.Message);
    }

    [Fact]
    public async Task ResponseContentNotEmpty_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._responseAssertions);

        httpAssertionBuilder.ResponseContentNotEmpty(TestContext.Current.CancellationToken);
        Assert.Single(httpAssertionBuilder._responseAssertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Content = new StringContent("Hello World!");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var assertion = httpAssertionBuilder._responseAssertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.Content = new StringContent("");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response content not to be empty.", exception.Message);
    }

    [Fact]
    public void ResponseHeaderExists_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.ResponseHeaderExists(null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.ResponseHeaderExists(string.Empty));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.ResponseHeaderExists(" "));
    }

    [Fact]
    public async Task ResponseHeaderExists_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._responseAssertions);

        httpAssertionBuilder.ResponseHeaderExists("framework");
        Assert.Single(httpAssertionBuilder._responseAssertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "Furion");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var assertion = httpAssertionBuilder._responseAssertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.Headers.Remove("framework");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response header 'framework' to exist, but it was not found.",
            exception.Message);

        httpResponseMessage.Content = new StringContent("Hello World!");
        httpResponseMessage.Content.Headers.TryAddWithoutValidation("framework", "Furion");

        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);
        await assertion(httpAssertionContext);
    }

    [Fact]
    public void ResponseHeaderEquals_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.ResponseHeaderEquals(null!, null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.ResponseHeaderEquals(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.ResponseHeaderEquals(" ", null!));

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.ResponseHeaderEquals("framework", null!));
    }

    [Fact]
    public async Task ResponseHeaderEquals_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._responseAssertions);

        httpAssertionBuilder.ResponseHeaderEquals("framework", "furion");
        Assert.Single(httpAssertionBuilder._responseAssertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "furion");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var assertion = httpAssertionBuilder._responseAssertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.Headers.Remove("framework");
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "Furion");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);
        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response header 'framework' to be 'furion', but found 'Furion'.",
            exception.Message);

        httpResponseMessage.Headers.Remove("framework");
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "aspnetcore");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var exception2 =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response header 'framework' to be 'furion', but found 'aspnetcore'.",
            exception2.Message);
    }

    [Fact]
    public void ResponseHeaderContains_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.ResponseHeaderContains(null!, null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.ResponseHeaderContains(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.ResponseHeaderContains(" ", null!));

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.ResponseHeaderContains("framework", null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.ResponseHeaderContains("framework", string.Empty));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.ResponseHeaderContains("framework", " "));
    }

    [Fact]
    public async Task ResponseHeaderContains_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._responseAssertions);

        httpAssertionBuilder.ResponseHeaderContains("framework", "furion");
        Assert.Single(httpAssertionBuilder._responseAssertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "furion");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var assertion = httpAssertionBuilder._responseAssertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.Headers.Remove("framework");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);
        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response header 'framework' to contain 'furion', but the header was not found.",
            exception.Message);

        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "Furion");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);
        await assertion(httpAssertionContext);

        httpResponseMessage.Headers.Remove("framework");
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "aspnetcore");
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "dotnetcore");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var exception2 =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal(
            "Expected response header 'framework' to contain 'furion', but actual values were: [aspnetcore, dotnetcore].",
            exception2.Message);
    }

    [Fact]
    public void ResponseHeaderNotExists_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.ResponseHeaderNotExists(null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.ResponseHeaderNotExists(string.Empty));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.ResponseHeaderNotExists(" "));
    }

    [Fact]
    public async Task ResponseHeaderNotExists_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._responseAssertions);

        httpAssertionBuilder.ResponseHeaderNotExists("framework");
        Assert.Single(httpAssertionBuilder._responseAssertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var assertion = httpAssertionBuilder._responseAssertions[0];
        await assertion(httpAssertionContext);

        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "Furion");
        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response header 'framework' not to exist, but it was found.",
            exception.Message);
    }

    [Fact]
    public void ResponseDurationUnder_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        var exception = Assert.Throws<ArgumentException>(() => httpAssertionBuilder.ResponseDurationUnder(0));
        Assert.Equal("Max milliseconds must be greater than 0. (Parameter 'maxMilliseconds')", exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => httpAssertionBuilder.ResponseDurationUnder(-1));
        Assert.Equal("Max milliseconds must be greater than 0. (Parameter 'maxMilliseconds')", exception2.Message);

        var exception3 =
            Assert.Throws<ArgumentException>(() => httpAssertionBuilder.ResponseDurationUnder(TimeSpan.Zero));
        Assert.Equal("Max duration must be greater than 0. (Parameter 'maxDuration')", exception3.Message);

        var exception4 =
            Assert.Throws<ArgumentException>(() =>
                httpAssertionBuilder.ResponseDurationUnder(TimeSpan.FromMilliseconds(-1000)));
        Assert.Equal("Max duration must be greater than 0. (Parameter 'maxDuration')", exception4.Message);
    }

    [Fact]
    public async Task ResponseDurationUnder_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._responseAssertions);

        httpAssertionBuilder.ResponseDurationUnder(100);
        Assert.Single(httpAssertionBuilder._responseAssertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 100, serviceProvider);

        var assertion = httpAssertionBuilder._responseAssertions[0];
        await assertion(httpAssertionContext);

        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 101, serviceProvider);
        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response duration to be under 100.00ms, but it took 101.00ms.",
            exception.Message);
    }

    [Fact]
    public void ResponseSatisfies_Sync_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Throws<ArgumentNullException>(() =>
            httpAssertionBuilder.ResponseSatisfies((Action<HttpResponseMessage>)null!));
    }

    [Fact]
    public async Task ResponseSatisfies_Sync_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._responseAssertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Headers.TryAddWithoutValidation("X-Custom", "Passed");

        httpAssertionBuilder.ResponseSatisfies(r =>
            Assert.Equal("Passed", r.Headers.GetValues("X-Custom").First()));
        Assert.Single(httpAssertionBuilder._responseAssertions);

        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 0, serviceProvider);

        var assertion = httpAssertionBuilder._responseAssertions[0];
        await assertion(httpAssertionContext);

        httpAssertionBuilder.ResponseSatisfies(r => throw new InvalidOperationException("sync error"));
        assertion = httpAssertionBuilder._responseAssertions[1];

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("sync error", exception.Message);
    }

    [Fact]
    public void ResponseSatisfies_Async_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Throws<ArgumentNullException>(() =>
            httpAssertionBuilder.ResponseSatisfies(null!));
    }

    [Fact]
    public async Task ResponseSatisfies_Async_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._responseAssertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Accepted);

        httpAssertionBuilder.ResponseSatisfies(async r =>
        {
            await Task.Yield();
            Assert.Equal(HttpStatusCode.Accepted, r.StatusCode);
        });
        Assert.Single(httpAssertionBuilder._responseAssertions);

        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, null, 0, serviceProvider);

        var assertion = httpAssertionBuilder._responseAssertions[0];
        await assertion(httpAssertionContext);

        httpAssertionBuilder.ResponseSatisfies(r => throw new InvalidOperationException("async error"));
        assertion = httpAssertionBuilder._responseAssertions[1];

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("async error", exception.Message);
    }
}