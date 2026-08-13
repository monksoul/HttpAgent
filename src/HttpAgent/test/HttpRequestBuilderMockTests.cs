// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRequestBuilderMockTests
{
    [Fact]
    public void MockResponse_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.MockResponse(null!));
        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.MockResponse<ContentModel>(null!));
    }

    [Fact]
    public void MockResponse_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var httpResponseMessage = new HttpResponseMessage();
        httpRequestBuilder.MockResponse(httpResponseMessage);
        Assert.NotNull(httpRequestBuilder.MockedResponse);
        Assert.Null(httpRequestBuilder.MockedException);

        httpRequestBuilder.MockResponse(new HttpResponseMessage());
        Assert.NotSame(httpResponseMessage, httpRequestBuilder.MockedResponse);

        Assert.True(
            (bool?)typeof(HttpResponseMessage).GetField("_disposed", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(httpResponseMessage));

        httpRequestBuilder.MockResponse(new ContentModel { Id = 1, Name = "Furion" });
        Assert.NotNull(httpRequestBuilder.MockedResponse);
        Assert.Null(httpRequestBuilder.MockedException);
        Assert.Equal(HttpStatusCode.OK, httpRequestBuilder.MockedResponse.StatusCode);
        Assert.Equal("application/json; charset=utf-8",
            httpRequestBuilder.MockedResponse.Content.Headers.ContentType?.ToString());
        Assert.Equal("{\"id\":1,\"name\":\"Furion\"}",
            AsyncUtility.RunSync(() =>
                httpRequestBuilder.MockedResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)));
    }

    [Fact]
    public void MockException_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.MockException(null!));
    }

    [Fact]
    public void MockException_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var httpResponseMessage = new HttpResponseMessage();
        httpRequestBuilder.MockResponse(httpResponseMessage).MockException(new Exception("出错了"));

        Assert.Null(httpRequestBuilder.MockedResponse);
        Assert.True(
            (bool?)typeof(HttpResponseMessage).GetField("_disposed", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(httpResponseMessage));

        Assert.NotNull(httpRequestBuilder.MockedException);
        Assert.Null(httpRequestBuilder.MockedResponse);
    }

    [Fact]
    public void ClearMock_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var httpResponseMessage = new HttpResponseMessage();
        httpRequestBuilder.MockResponse(httpResponseMessage);

        httpRequestBuilder.ClearMock();

        Assert.Null(httpRequestBuilder.MockedResponse);
        Assert.True(
            (bool?)typeof(HttpResponseMessage).GetField("_disposed", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(httpResponseMessage));
        Assert.Null(httpRequestBuilder.MockedResponse);
    }

    [Fact]
    public void IsMocked_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.False(httpRequestBuilder.IsMocked());

        httpRequestBuilder.MockResponse(new HttpResponseMessage());
        Assert.True(httpRequestBuilder.IsMocked());

        httpRequestBuilder.MockException(new Exception("出错了"));
        Assert.True(httpRequestBuilder.IsMocked());

        httpRequestBuilder.ClearMock();
    }

    private class ContentModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}