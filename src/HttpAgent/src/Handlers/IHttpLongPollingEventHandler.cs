﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     长轮询事件处理程序
/// </summary>
public interface IHttpLongPollingEventHandler
{
    /// <summary>
    ///     用于接收服务器返回 <c>200~299</c> 状态码的数据的操作
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="Task" />
    /// </returns>
    Task OnDataReceivedAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken);

    /// <summary>
    ///     用于接收服务器返回非 <c>200~299</c> 状态码的数据的操作
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="Task" />
    /// </returns>
    Task OnErrorAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken);

    /// <summary>
    ///     用于响应标头包含 <c>X-End-Of-Stream</c> 时触发的操作
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="Task" />
    /// </returns>
    Task OnEndOfStreamAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken);
}