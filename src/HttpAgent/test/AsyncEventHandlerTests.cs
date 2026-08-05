// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class AsyncEventHandlerTests
{
    [Fact]
    public void New_ReturnOK()
    {
        AsyncEventHandler<WebSocketTextReceiveResult> _ = (_, _) => Task.CompletedTask;
    }

    [Fact]
    public async Task TryInvokeAsync_ReturnOK()
    {
        var i = 0;
        AsyncEventHandler<WebSocketTextReceiveResult?> handler = (_, _) =>
        {
            i++;
            return Task.CompletedTask;
        };

        await handler.TryInvokeAsync(new WebSocketTextReceiveResult(0, true), null);
        Assert.Equal(1, i);

        AsyncEventHandler<WebSocketTextReceiveResult?> handler2 = (_, _) => throw new Exception("出错了");
        await handler2.TryInvokeAsync(new WebSocketTextReceiveResult(0, true), null);
    }
}