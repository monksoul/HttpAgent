// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpAssertionExceptionTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var exception = new HttpAssertionException("出错了");
        Assert.Equal("出错了", exception.Message);
    }

    [Fact]
    public void Throw_ReturnOK()
    {
        var exception = Assert.Throws<HttpAssertionException>(() => HttpAssertionException.Throw("出错了"));
        Assert.Equal("出错了", exception.Message);
    }

    [Fact]
    public async Task ThrowAsync_ReturnOK()
    {
        var exception =
            await Assert.ThrowsAsync<HttpAssertionException>(async () =>
                await HttpAssertionException.ThrowAsync("出错了"));
        Assert.Equal("出错了", exception.Message);
    }
}