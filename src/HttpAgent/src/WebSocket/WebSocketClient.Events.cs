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
    public event EventHandler<EventArgs>? Connecting;

    /// <summary>
    ///     连接成功时触发事件
    /// </summary>
    public event EventHandler<EventArgs>? Connected;

    /// <summary>
    ///     开始重新连接时触发事件
    /// </summary>
    public event EventHandler<EventArgs>? Reconnecting;

    /// <summary>
    ///     重新连接成功时触发事件
    /// </summary>
    public event EventHandler<EventArgs>? Reconnected;

    /// <summary>
    ///     开始关闭连接时触发事件
    /// </summary>
    public event EventHandler<EventArgs>? Closing;

    /// <summary>
    ///     关闭连接成功时触发事件
    /// </summary>
    public event EventHandler<EventArgs>? Closed;

    /// <summary>
    ///     开始接收消息时触发事件
    /// </summary>
    public event EventHandler<EventArgs>? ReceivingStarted;

    /// <summary>
    ///     停止接收消息时触发事件
    /// </summary>
    public event EventHandler<EventArgs>? ReceivingStopped;

    /// <summary>
    ///     接收文本消息事件
    /// </summary>
    public event EventHandler<WebSocketTextReceiveResult>? TextReceived;

    /// <summary>
    ///     接收二进制消息事件
    /// </summary>
    public event EventHandler<WebSocketBinaryReceiveResult>? BinaryReceived;

    /// <summary>
    ///     触发开始连接事件
    /// </summary>
    internal void OnConnecting() => Connecting?.TryInvoke(this, EventArgs.Empty);

    /// <summary>
    ///     触发连接成功事件
    /// </summary>
    internal void OnConnected() => Connected?.TryInvoke(this, EventArgs.Empty);

    /// <summary>
    ///     触发开始重新连接事件
    /// </summary>
    internal void OnReconnecting() => Reconnecting?.TryInvoke(this, EventArgs.Empty);

    /// <summary>
    ///     触发重新连接成功事件
    /// </summary>
    internal void OnReconnected() => Reconnected?.TryInvoke(this, EventArgs.Empty);

    /// <summary>
    ///     触发开始关闭连接事件
    /// </summary>
    internal void OnClosing() => Closing?.TryInvoke(this, EventArgs.Empty);

    /// <summary>
    ///     触发关闭连接成功事件
    /// </summary>
    internal void OnClosed() => Closed?.TryInvoke(this, EventArgs.Empty);

    /// <summary>
    ///     触发开始接收消息事件
    /// </summary>
    internal void OnReceivingStarted() => ReceivingStarted?.TryInvoke(this, EventArgs.Empty);

    /// <summary>
    ///     触发停止接收消息事件
    /// </summary>
    internal void OnReceivingStopped() => ReceivingStopped?.TryInvoke(this, EventArgs.Empty);

    /// <summary>
    ///     触发接收文本消息事件
    /// </summary>
    /// <param name="receiveResult">
    ///     <see cref="WebSocketTextReceiveResult" />
    /// </param>
    internal void OnTextReceived(WebSocketTextReceiveResult receiveResult) =>
        TextReceived?.TryInvoke(this, receiveResult);

    /// <summary>
    ///     触发接收二进制消息事件
    /// </summary>
    /// <param name="receiveResult">
    ///     <see cref="WebSocketBinaryReceiveResult" />
    /// </param>
    internal void OnBinaryReceived(WebSocketBinaryReceiveResult receiveResult) =>
        BinaryReceived?.TryInvoke(this, receiveResult);
}