// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRequestPipelineContextTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new HttpRequestPipelineContext(null!, null!, default, null!, CancellationToken.None));
        Assert.Throws<ArgumentNullException>(() =>
            new HttpRequestPipelineContext(HttpRequestBuilder.Get("http://localhost"), null!, default, null!,
                CancellationToken.None));
        Assert.Throws<ArgumentNullException>(() =>
            new HttpRequestPipelineContext(HttpRequestBuilder.Get("http://localhost"), new HttpClient(), default, null!,
                CancellationToken.None));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var context = new HttpRequestPipelineContext(HttpRequestBuilder.Get("http://localhost"), new HttpClient(),
            HttpCompletionOption.ResponseContentRead,
            (_, _, _, _) => Task.FromResult(new HttpResponseMessage()),
            CancellationToken.None);

        Assert.NotNull(context.OriginalBuilder);
        Assert.NotNull(context.Builder);
        Assert.NotNull(context.HttpClient);
        Assert.Equal(HttpCompletionOption.ResponseContentRead, context.CompletionOption);
        Assert.Equal(CancellationToken.None, context.CancellationToken);
        Assert.NotNull(context.SendAsync);
        Assert.Null(context.RequestMessage);
        Assert.Null(context.ResponseMessage);
        Assert.Equal(0, context.RequestDuration);
        Assert.NotNull(context.Items);
        Assert.Empty(context.Items);
    }
}