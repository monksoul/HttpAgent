// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ContentLengthValidationPipelineHandlerTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var handler = new ContentLengthValidationPipelineHandler();
        Assert.NotNull(handler);
    }

    [Fact]
    public void CheckContentLengthWithinLimit_Invalid_Parameters()
    {
        var httpRequestBuilder =
            HttpRequestBuilder.Get("http://localhost:5000/test").SetMaxResponseContentBufferSize(10);

        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Content = new StringContent("Hello World!");

        var exception = Assert.Throws<HttpRequestException>(() =>
            ContentLengthValidationPipelineHandler.CheckContentLengthWithinLimit(httpRequestBuilder,
                httpResponseMessage));
        Assert.Equal("Cannot write more bytes to the buffer than the configured maximum buffer size: `10`.",
            exception.Message);
    }

    [Fact]
    public void CheckContentLengthWithinLimit_ReturnOK()
    {
        var httpRequestBuilder = HttpRequestBuilder.Get("http://localhost:5000/test");
        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Content = new StringContent("Hello World!");

        ContentLengthValidationPipelineHandler.CheckContentLengthWithinLimit(httpRequestBuilder, httpResponseMessage);

        var httpRequestBuilder2 =
            HttpRequestBuilder.Get("http://localhost:5000/test").SetMaxResponseContentBufferSize(30);
        var httpResponseMessage2 = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage2.Content = new StringContent("Hello World!");

        ContentLengthValidationPipelineHandler.CheckContentLengthWithinLimit(httpRequestBuilder2, httpResponseMessage2);
    }
}