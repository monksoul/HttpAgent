// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpAssertionBuilderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        Assert.NotNull(httpAssertionBuilder);
        Assert.NotNull(httpAssertionBuilder._requestAssertions);
        Assert.Empty(httpAssertionBuilder._requestAssertions);
        Assert.NotNull(httpAssertionBuilder._responseAssertions);
        Assert.Empty(httpAssertionBuilder._responseAssertions);
    }

    [Fact]
    public void AddAssertion_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.AddAssertion(null!));
    }

    [Fact]
    public void AddAssertion_AddsToResponseAssertions_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        httpAssertionBuilder.AddAssertion(_ => Task.CompletedTask);
        Assert.Single(httpAssertionBuilder._responseAssertions);
        Assert.Empty(httpAssertionBuilder._requestAssertions);

        httpAssertionBuilder.AddAssertion(_ => Task.CompletedTask);
        Assert.Equal(2, httpAssertionBuilder._responseAssertions.Count);
        Assert.Empty(httpAssertionBuilder._requestAssertions);
    }

    [Fact]
    public void GetRequestAssertions_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder.GetRequestAssertions());

        httpAssertionBuilder.RequestMethod(HttpMethod.Get);
        Assert.Single(httpAssertionBuilder.GetRequestAssertions());

        httpAssertionBuilder.RequestUri("https://furion.net");
        Assert.Equal(2, httpAssertionBuilder.GetRequestAssertions().Count);
    }

    [Fact]
    public void GetResponseAssertions_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder.GetResponseAssertions());

        httpAssertionBuilder.AddAssertion(_ => Task.CompletedTask);
        Assert.Single(httpAssertionBuilder.GetResponseAssertions());

        httpAssertionBuilder.ResponseStatusCode(200);
        Assert.Equal(2, httpAssertionBuilder.GetResponseAssertions().Count);
    }

    [Fact]
    public void RequestAssertionMethods_AddToRequestAssertions_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        httpAssertionBuilder.RequestMethod(HttpMethod.Post);
        Assert.Single(httpAssertionBuilder._requestAssertions);
        Assert.Empty(httpAssertionBuilder._responseAssertions);

        httpAssertionBuilder.RequestUri("https://api.example.com");
        httpAssertionBuilder.RequestHeaderExists("Content-Type");
        httpAssertionBuilder.RequestHeaderEquals("Accept", "application/json");
        httpAssertionBuilder.RequestHeaderContains("Authorization", "Bearer");
        httpAssertionBuilder.RequestContentContains("hello", TestContext.Current.CancellationToken);
        httpAssertionBuilder.RequestContentEquals("hello world", TestContext.Current.CancellationToken);

        Assert.Equal(7, httpAssertionBuilder._requestAssertions.Count);
        Assert.Empty(httpAssertionBuilder._responseAssertions);
    }

    [Fact]
    public void ResponseAssertionMethods_AddToResponseAssertions_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        httpAssertionBuilder.ResponseStatusCode(200);
        httpAssertionBuilder.ResponseStatusCodeIn(200, 201);
        httpAssertionBuilder.ResponseIsSuccessStatusCode();
        httpAssertionBuilder.ResponseContentContains("ok", TestContext.Current.CancellationToken);
        httpAssertionBuilder.ResponseContentEquals("ok", TestContext.Current.CancellationToken);
        httpAssertionBuilder.ResponseContentMatches(@"\w+", TestContext.Current.CancellationToken);
        httpAssertionBuilder.ResponseContentNotEmpty(TestContext.Current.CancellationToken);
        httpAssertionBuilder.ResponseHeaderExists("Server");
        httpAssertionBuilder.ResponseHeaderEquals("X-Custom", "value");
        httpAssertionBuilder.ResponseHeaderContains("X-Custom", "val");
        httpAssertionBuilder.ResponseHeaderNotExists("X-Removed");
        httpAssertionBuilder.ResponseDurationUnder(500);

        Assert.Equal(12, httpAssertionBuilder._responseAssertions.Count);
        Assert.Empty(httpAssertionBuilder._requestAssertions);
    }
}