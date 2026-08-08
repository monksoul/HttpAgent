// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class MemoryETagCacheTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var memoryETagCache = new MemoryETagCache();
        Assert.NotNull(memoryETagCache._cache);
        Assert.Empty(memoryETagCache._cache);
    }

    [Fact]
    public void TryGet_Invalid_Parameters()
    {
        var memoryETagCache = new MemoryETagCache();
        Assert.Throws<ArgumentNullException>(() => memoryETagCache.TryGet(null!, out _));
        Assert.Throws<ArgumentException>(() => memoryETagCache.TryGet(string.Empty, out _));
        Assert.Throws<ArgumentException>(() => memoryETagCache.TryGet(" ", out _));
    }

    [Fact]
    public void TryGet_ReturnOK()
    {
        var memoryETagCache = new MemoryETagCache();
        Assert.False(memoryETagCache.TryGet("GET:http:/furion.net/", out _));

        memoryETagCache.Set("GET:http:/furion.net/", new HttpETagCacheItem());
        Assert.True(memoryETagCache.TryGet("GET:http:/furion.net/", out var eTagCacheItem));
        Assert.NotNull(eTagCacheItem);
    }

    [Fact]
    public void Set_Invalid_Parameters()
    {
        var memoryETagCache = new MemoryETagCache();
        Assert.Throws<ArgumentNullException>(() => memoryETagCache.Set(null!, null!));
        Assert.Throws<ArgumentException>(() => memoryETagCache.Set(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => memoryETagCache.Set(" ", null!));

        Assert.Throws<ArgumentNullException>(() => memoryETagCache.Set("GET:http:/furion.net/", null!));
    }

    [Fact]
    public void Set_ReturnOK()
    {
        var memoryETagCache = new MemoryETagCache();
        memoryETagCache.Set("GET:http:/furion.net/", new HttpETagCacheItem());
        Assert.Single(memoryETagCache._cache);

        memoryETagCache.Set("GET:http:/furion.net/", new HttpETagCacheItem());
        Assert.Single(memoryETagCache._cache);
    }

    [Fact]
    public void Remove_Invalid_Parameters()
    {
        var memoryETagCache = new MemoryETagCache();
        Assert.Throws<ArgumentNullException>(() => memoryETagCache.Remove(null!));
        Assert.Throws<ArgumentException>(() => memoryETagCache.Remove(string.Empty));
        Assert.Throws<ArgumentException>(() => memoryETagCache.Remove(" "));
    }

    [Fact]
    public void Remove_ReturnOK()
    {
        var memoryETagCache = new MemoryETagCache();
        memoryETagCache.Set("GET:http:/furion.net/", new HttpETagCacheItem());
        Assert.Single(memoryETagCache._cache);

        memoryETagCache.Remove("GET:http:/furion.net/");
        Assert.Empty(memoryETagCache._cache);
    }
}