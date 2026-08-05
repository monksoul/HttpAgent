// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class WebSocketClientEventsTests
{
    [Fact]
    public async Task New_ReturnOK()
    {
        var webSocketClient = new WebSocketClient("ws://localhost:12345");
        var events = new string[10];

        webSocketClient.Connecting += (_, _) =>
        {
            events[0] = nameof(webSocketClient.Connecting);
            return Task.CompletedTask;
        };
        webSocketClient.Connected += (_, _) =>
        {
            events[1] = nameof(webSocketClient.Connected);
            return Task.CompletedTask;
        };
        webSocketClient.Reconnecting += (_, _) =>
        {
            events[2] = nameof(webSocketClient.Reconnecting);
            return Task.CompletedTask;
        };
        webSocketClient.Reconnected += (_, _) =>
        {
            events[3] = nameof(webSocketClient.Reconnected);
            return Task.CompletedTask;
        };
        webSocketClient.Closing += (_, _) =>
        {
            events[4] = nameof(webSocketClient.Closing);
            return Task.CompletedTask;
        };
        webSocketClient.Closed += (_, _) =>
        {
            events[5] = nameof(webSocketClient.Closed);
            return Task.CompletedTask;
        };
        webSocketClient.ReceivingStarted += (_, _) =>
        {
            events[6] = nameof(webSocketClient.ReceivingStarted);
            return Task.CompletedTask;
        };
        webSocketClient.ReceivingStopped += (_, _) =>
        {
            events[7] = nameof(webSocketClient.ReceivingStopped);
            return Task.CompletedTask;
        };
        webSocketClient.TextReceived += (_, _) =>
        {
            events[8] = nameof(webSocketClient.TextReceived);
            return Task.CompletedTask;
        };
        webSocketClient.BinaryReceived += (_, _) =>
        {
            events[9] = nameof(webSocketClient.BinaryReceived);
            return Task.CompletedTask;
        };

        await webSocketClient.OnConnectingAsync();
        await webSocketClient.OnConnectedAsync();
        await webSocketClient.OnReconnectingAsync();
        await webSocketClient.OnReconnectedAsync();
        await webSocketClient.OnClosingAsync();
        await webSocketClient.OnClosedAsync();
        await webSocketClient.OnReceivingStartedAsync();
        await webSocketClient.OnReceivingStoppedAsync();
        await webSocketClient.OnTextReceivedAsync(new WebSocketTextReceiveResult(0, true, WebSocketCloseStatus.Empty,
            null));
        await webSocketClient.OnBinaryReceivedAsync(
            new WebSocketBinaryReceiveResult(0, true, WebSocketCloseStatus.Empty, null));

        Assert.Equal(
        [
            "Connecting", "Connected", "Reconnecting", "Reconnected", "Closing", "Closed", "ReceivingStarted",
            "ReceivingStopped", "TextReceived", "BinaryReceived"
        ], events);
    }
}