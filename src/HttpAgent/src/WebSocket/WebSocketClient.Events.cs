// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     WebSocket 客户端
/// </summary>
public sealed partial class WebSocketClient
{
    /// <summary>
    ///     开始连接时触发事件
    /// </summary>
    public event AsyncEventHandler<EventArgs>? Connecting;

    /// <summary>
    ///     连接成功时触发事件
    /// </summary>
    public event AsyncEventHandler<EventArgs>? Connected;

    /// <summary>
    ///     开始重新连接时触发事件
    /// </summary>
    public event AsyncEventHandler<EventArgs>? Reconnecting;

    /// <summary>
    ///     重新连接成功时触发事件
    /// </summary>
    public event AsyncEventHandler<EventArgs>? Reconnected;

    /// <summary>
    ///     开始关闭连接时触发事件
    /// </summary>
    public event AsyncEventHandler<EventArgs>? Closing;

    /// <summary>
    ///     关闭连接成功时触发事件
    /// </summary>
    public event AsyncEventHandler<EventArgs>? Closed;

    /// <summary>
    ///     开始接收消息时触发事件
    /// </summary>
    public event AsyncEventHandler<EventArgs>? ReceivingStarted;

    /// <summary>
    ///     停止接收消息时触发事件
    /// </summary>
    public event AsyncEventHandler<EventArgs>? ReceivingStopped;

    /// <summary>
    ///     接收文本消息事件
    /// </summary>
    public event AsyncEventHandler<WebSocketTextReceiveResult>? TextReceived;

    /// <summary>
    ///     接收二进制消息事件
    /// </summary>
    public event AsyncEventHandler<WebSocketBinaryReceiveResult>? BinaryReceived;

    /// <summary>
    ///     触发开始连接事件
    /// </summary>
    internal async Task OnConnectingAsync() =>
        await Connecting.TryInvokeAsync(this, EventArgs.Empty);

    /// <summary>
    ///     触发连接成功事件
    /// </summary>
    internal async Task OnConnectedAsync() =>
        await Connected.TryInvokeAsync(this, EventArgs.Empty);

    /// <summary>
    ///     触发开始重新连接事件
    /// </summary>
    internal async Task OnReconnectingAsync() =>
        await Reconnecting.TryInvokeAsync(this, EventArgs.Empty);

    /// <summary>
    ///     触发重新连接成功事件
    /// </summary>
    internal async Task OnReconnectedAsync() =>
        await Reconnected.TryInvokeAsync(this, EventArgs.Empty);

    /// <summary>
    ///     触发开始关闭连接事件
    /// </summary>
    internal async Task OnClosingAsync() =>
        await Closing.TryInvokeAsync(this, EventArgs.Empty);

    /// <summary>
    ///     触发关闭连接成功事件
    /// </summary>
    internal async Task OnClosedAsync() =>
        await Closed.TryInvokeAsync(this, EventArgs.Empty);

    /// <summary>
    ///     触发开始接收消息事件
    /// </summary>
    internal async Task OnReceivingStartedAsync() =>
        await ReceivingStarted.TryInvokeAsync(this, EventArgs.Empty);

    /// <summary>
    ///     触发停止接收消息事件
    /// </summary>
    internal async Task OnReceivingStoppedAsync() =>
        await ReceivingStopped.TryInvokeAsync(this, EventArgs.Empty);

    /// <summary>
    ///     触发接收文本消息事件
    /// </summary>
    /// <param name="receiveResult">
    ///     <see cref="WebSocketTextReceiveResult" />
    /// </param>
    internal async Task OnTextReceivedAsync(WebSocketTextReceiveResult receiveResult) =>
        await TextReceived.TryInvokeAsync(this, receiveResult);

    /// <summary>
    ///     触发接收二进制消息事件
    /// </summary>
    /// <param name="receiveResult">
    ///     <see cref="WebSocketBinaryReceiveResult" />
    /// </param>
    internal async Task OnBinaryReceivedAsync(WebSocketBinaryReceiveResult receiveResult) =>
        await BinaryReceived.TryInvokeAsync(this, receiveResult);
}