// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpAssertionBuilderAssertionsTests
{
    [Fact]
    public async Task StatusCode_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._assertions);

        httpAssertionBuilder.StatusCode(200).StatusCode(HttpStatusCode.NoContent);
        Assert.Equal(2, httpAssertionBuilder._assertions.Count);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        var assertion = httpAssertionBuilder._assertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected status code to be 200, but found 500.", exception.Message);

        var assertion1 = httpAssertionBuilder._assertions[1];
        var exception2 =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion1(httpAssertionContext));
        Assert.Equal("Expected status code to be 204, but found 500.", exception2.Message);
    }

    [Fact]
    public void StatusCodeIn_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.StatusCodeIn(null!));
        var exception = Assert.Throws<ArgumentException>(() => httpAssertionBuilder.StatusCodeIn());
        Assert.Equal("The allowed status codes array cannot be null or empty. (Parameter 'allowedStatusCodes')",
            exception.Message);
    }

    [Fact]
    public async Task StatusCodeIn_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._assertions);

        httpAssertionBuilder.StatusCodeIn(200, 204);
        Assert.Single(httpAssertionBuilder._assertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        var assertion = httpAssertionBuilder._assertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected status code to be one of [200, 204], but found 500.", exception.Message);
    }

    [Fact]
    public async Task IsSuccessStatusCode_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._assertions);

        httpAssertionBuilder.IsSuccessStatusCode();
        Assert.Single(httpAssertionBuilder._assertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        var assertion = httpAssertionBuilder._assertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected request to be successful (2xx status code), but found status code 500.",
            exception.Message);
    }

    [Fact]
    public void ContentContains_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.ContentContains(null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.ContentContains(string.Empty));
    }

    [Fact]
    public async Task ContentContains_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._assertions);

        httpAssertionBuilder.ContentContains("Hello");
        Assert.Single(httpAssertionBuilder._assertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Content = new StringContent("Hello World!");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        var assertion = httpAssertionBuilder._assertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.Content = new StringContent("Furion YYDS!");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response content to contain 'Hello', but it was not found.",
            exception.Message);
    }

    [Fact]
    public void HeaderExists_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.HeaderExists(null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.HeaderExists(string.Empty));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.HeaderExists(" "));
    }

    [Fact]
    public async Task HeaderExists_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._assertions);

        httpAssertionBuilder.HeaderExists("framework");
        Assert.Single(httpAssertionBuilder._assertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "Furion");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        var assertion = httpAssertionBuilder._assertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.Headers.Remove("framework");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response header 'framework' to exist, but it was not found.",
            exception.Message);

        httpResponseMessage.Content = new StringContent("Hello World!");
        httpResponseMessage.Content.Headers.TryAddWithoutValidation("framework", "Furion");

        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);
        await assertion(httpAssertionContext);
    }

    [Fact]
    public void HeaderEquals_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.HeaderEquals(null!, null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.HeaderEquals(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.HeaderEquals(" ", null!));

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.HeaderEquals("framework", null!));
    }

    [Fact]
    public async Task HeaderEquals_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._assertions);

        httpAssertionBuilder.HeaderEquals("framework", "furion");
        Assert.Single(httpAssertionBuilder._assertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "furion");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        var assertion = httpAssertionBuilder._assertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.Headers.Remove("framework");
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "Furion");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);
        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response header 'framework' to be 'furion', but found 'Furion'.",
            exception.Message);

        httpResponseMessage.Headers.Remove("framework");
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "aspnetcore");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        var exception2 =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response header 'framework' to be 'furion', but found 'aspnetcore'.",
            exception2.Message);
    }

    [Fact]
    public void HeaderContains_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.HeaderContains(null!, null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.HeaderContains(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.HeaderContains(" ", null!));

        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.HeaderContains("framework", null!));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.HeaderContains("framework", string.Empty));
        Assert.Throws<ArgumentException>(() => httpAssertionBuilder.HeaderContains("framework", " "));
    }

    [Fact]
    public async Task HeaderContains_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._assertions);

        httpAssertionBuilder.HeaderContains("framework", "furion");
        Assert.Single(httpAssertionBuilder._assertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "furion");
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        var assertion = httpAssertionBuilder._assertions[0];
        await assertion(httpAssertionContext);

        httpResponseMessage.Headers.Remove("framework");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);
        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected response header 'framework' to contain 'furion', but the header was not found.",
            exception.Message);

        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "Furion");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);
        await assertion(httpAssertionContext);

        httpResponseMessage.Headers.Remove("framework");
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "aspnetcore");
        httpResponseMessage.Headers.TryAddWithoutValidation("framework", "dotnetcore");
        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        var exception2 =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal(
            "Expected response header 'framework' to contain 'furion', but actual values were: [aspnetcore, dotnetcore].",
            exception2.Message);
    }

    [Fact]
    public void DurationUnder_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        var exception = Assert.Throws<ArgumentException>(() => httpAssertionBuilder.DurationUnder(0));
        Assert.Equal("Max milliseconds must be greater than 0. (Parameter 'maxMilliseconds')", exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => httpAssertionBuilder.DurationUnder(-1));
        Assert.Equal("Max milliseconds must be greater than 0. (Parameter 'maxMilliseconds')", exception2.Message);

        var exception3 = Assert.Throws<ArgumentException>(() => httpAssertionBuilder.DurationUnder(TimeSpan.Zero));
        Assert.Equal("Max duration must be greater than 0. (Parameter 'maxDuration')", exception3.Message);

        var exception4 =
            Assert.Throws<ArgumentException>(() =>
                httpAssertionBuilder.DurationUnder(TimeSpan.FromMilliseconds(-1000)));
        Assert.Equal("Max duration must be greater than 0. (Parameter 'maxDuration')", exception4.Message);
    }

    [Fact]
    public async Task DurationUnder_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder._assertions);

        httpAssertionBuilder.DurationUnder(100);
        Assert.Single(httpAssertionBuilder._assertions);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        var services = new ServiceCollection();
        await using var serviceProvider = services.BuildServiceProvider();
        var httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 100, serviceProvider);

        var assertion = httpAssertionBuilder._assertions[0];
        await assertion(httpAssertionContext);

        httpAssertionContext = new HttpAssertionContext(httpResponseMessage, 101, serviceProvider);
        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () => await assertion(httpAssertionContext));
        Assert.Equal("Expected request duration to be under 100.00ms, but it took 101.00ms.",
            exception.Message);
    }
}