// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class AsyncEnumerableContentConverterTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var converter = new AsyncEnumerableContentConverter<ObjectModel>();
        Assert.NotNull(converter);
        Assert.True(
            typeof(IHttpContentConverter<IAsyncEnumerable<ObjectModel>>).IsAssignableFrom(
                typeof(AsyncEnumerableContentConverter<ObjectModel>)));
        Assert.False(converter.KeepsResponseAlive);
    }

    [Fact]
    public async Task Read_ReturnOK()
    {
        using var stringContent = new StringContent("""[{"id":10, "name":"furion"}]""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new AsyncEnumerableContentConverter<ObjectModel>();
        var asyncEnumerable = converter.Read(typeof(IAsyncEnumerable<ObjectModel>), httpResponseMessage);
        Assert.NotNull(asyncEnumerable);
        await foreach (var item in asyncEnumerable as IAsyncEnumerable<ObjectModel>)
        {
            Assert.Equal(10, item?.Id);
            Assert.Equal("furion", item?.Name);
            break;
        }

        using var stringContent2 = new StringContent("""[{"Id":10, "Name":"furion"}]""");
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        var converter2 = new AsyncEnumerableContentConverter<ObjectModel>();
        var asyncEnumerable2 = converter2.Read(typeof(IAsyncEnumerable<ObjectModel>), httpResponseMessage2);
        Assert.NotNull(asyncEnumerable2);
        await foreach (var item in asyncEnumerable2 as IAsyncEnumerable<ObjectModel>)
        {
            Assert.Equal(10, item?.Id);
            Assert.Equal("furion", item?.Name);
            break;
        }
    }

    [Fact]
    public async Task ReadAsync_ReturnOK()
    {
        using var stringContent = new StringContent("""[{"id":10, "name":"furion"}]""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new AsyncEnumerableContentConverter<ObjectModel>();
        var asyncEnumerable = await converter.ReadAsync(typeof(IAsyncEnumerable<ObjectModel>), httpResponseMessage);
        Assert.NotNull(asyncEnumerable);
        await foreach (var item in asyncEnumerable as IAsyncEnumerable<ObjectModel>)
        {
            Assert.Equal(10, item?.Id);
            Assert.Equal("furion", item?.Name);
            break;
        }

        using var stringContent2 = new StringContent("""[{"Id":10, "Name":"furion"}]""");
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        var converter2 = new AsyncEnumerableContentConverter<ObjectModel>();
        var asyncEnumerable2 = await converter2.ReadAsync(typeof(IAsyncEnumerable<ObjectModel>), httpResponseMessage2);
        Assert.NotNull(asyncEnumerable2);
        await foreach (var item in asyncEnumerable2 as IAsyncEnumerable<ObjectModel>)
        {
            Assert.Equal(10, item?.Id);
            Assert.Equal("furion", item?.Name);
            break;
        }
    }

    [Fact]
    public async Task Read_WithType_ReturnOK()
    {
        using var stringContent = new StringContent("""[{"id":10, "name":"furion"}]""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new AsyncEnumerableContentConverter<ObjectModel>();
        var asyncEnumerable = converter.Read(httpResponseMessage);
        Assert.NotNull(asyncEnumerable);
        await foreach (var item in asyncEnumerable)
        {
            Assert.Equal(10, item?.Id);
            Assert.Equal("furion", item?.Name);
            break;
        }

        using var stringContent2 = new StringContent("""[{"Id":10, "Name":"furion"}]""");
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        var converter2 = new AsyncEnumerableContentConverter<ObjectModel>();
        var asyncEnumerable2 = converter2.Read(httpResponseMessage2);
        Assert.NotNull(asyncEnumerable2);
        await foreach (var item in asyncEnumerable2)
        {
            Assert.Equal(10, item?.Id);
            Assert.Equal("furion", item?.Name);
            break;
        }
    }

    [Fact]
    public async Task ReadAsync_WithType_ReturnOK()
    {
        using var stringContent = new StringContent("""[{"id":10, "name":"furion"}]""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new AsyncEnumerableContentConverter<ObjectModel>();
        var asyncEnumerable = await converter.ReadAsync(httpResponseMessage);
        Assert.NotNull(asyncEnumerable);
        await foreach (var item in asyncEnumerable)
        {
            Assert.Equal(10, item?.Id);
            Assert.Equal("furion", item?.Name);
            break;
        }

        using var stringContent2 = new StringContent("""[{"Id":10, "Name":"furion"}]""");
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        var converter2 = new AsyncEnumerableContentConverter<ObjectModel>();
        var asyncEnumerable2 = await converter2.ReadAsync(httpResponseMessage2);
        Assert.NotNull(asyncEnumerable2);
        await foreach (var item in asyncEnumerable2)
        {
            Assert.Equal(10, item?.Id);
            Assert.Equal("furion", item?.Name);
            break;
        }
    }

    public class ObjectModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}