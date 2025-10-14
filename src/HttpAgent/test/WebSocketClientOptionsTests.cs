// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class WebSocketClientOptionsTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new WebSocketClientOptions((Uri)null!));
        Assert.Throws<ArgumentNullException>(() => new WebSocketClientOptions((string)null!));

        Assert.Throws<ArgumentException>(() => new WebSocketClientOptions(string.Empty));
        Assert.Throws<ArgumentException>(() => new WebSocketClientOptions(" "));

        Assert.Throws<ArgumentNullException>(() => new WebSocketClientOptions("ws://localhost:12345", null!));
        Assert.Throws<ArgumentNullException>(() => new WebSocketClientOptions(new Uri("ws://localhost:12345"), null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var options = new WebSocketClientOptions(new Uri("ws://localhost:12345"));
        Assert.NotNull(options.ServerUri);
        Assert.Null(options.Configure);
        Assert.Equal("ws://localhost:12345/", options.ServerUri.ToString());

        var options2 = new WebSocketClientOptions("ws://localhost:12345");
        Assert.Equal("ws://localhost:12345/", options2.ServerUri.ToString());
        Assert.Equal(TimeSpan.FromSeconds(2), options2.ReconnectInterval);
        Assert.Equal(10, options2.MaxReconnectRetries);
        Assert.Null(options2.Timeout);
        Assert.Equal(4096, options2.ReceiveBufferSize);
        Assert.Null(options2.Configure);

        var options3 = new WebSocketClientOptions(new Uri("ws://localhost:12345"), _ => { });
        Assert.NotNull(options3.ServerUri);
        Assert.NotNull(options3.Configure);
        Assert.Equal("ws://localhost:12345/", options3.ServerUri.ToString());

        var options4 = new WebSocketClientOptions("ws://localhost:12345", _ => { });
        Assert.NotNull(options4.ServerUri);
        Assert.NotNull(options4.Configure);
        Assert.Equal("ws://localhost:12345/", options4.ServerUri.ToString());
    }
}