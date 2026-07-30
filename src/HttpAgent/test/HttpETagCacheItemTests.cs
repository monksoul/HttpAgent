// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpETagCacheItemTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var httpETagCacheItem = new HttpETagCacheItem();
        Assert.Null(httpETagCacheItem.ETag);
        Assert.Equal(0, (int)httpETagCacheItem.StatusCode);
        Assert.Null(httpETagCacheItem.ContentBytes);
        Assert.Null(httpETagCacheItem.ContentHeaders);
        Assert.Null(httpETagCacheItem.ResponseHeaders);
        Assert.Null(httpETagCacheItem.ReasonPhrase);
    }
}