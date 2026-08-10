// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式远程请求代理类
/// </summary>
public class HttpDeclarativeDispatchProxy : DispatchProxyAsync
{
    /// <inheritdoc cref="IHttpRemoteService" />
    public IHttpRemoteService RemoteService { get; internal set; } = null!;

    /// <summary>
    ///     实际被代理的接口类型
    /// </summary>
    public Type InterfaceType { get; internal set; } = null!;

    /// <inheritdoc />
    public override object Invoke(MethodInfo method, object[] args) =>
        RemoteService.Declarative(method, args, InterfaceType)!;

    /// <inheritdoc />
    public override Task InvokeAsync(MethodInfo method, object[] args) =>
        InvokeAsyncT<VoidContent>(method, args);

    /// <inheritdoc />
    public override Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args) =>
        RemoteService.DeclarativeAsync<T>(method, args, InterfaceType)!;

    /// <inheritdoc />
    public override ValueTask InvokeValueTaskAsync(MethodInfo method, object[] args) => new(InvokeAsync(method, args));

    /// <inheritdoc />
    public override ValueTask<T> InvokeValueTaskAsyncT<T>(MethodInfo method, object[] args) =>
        new(InvokeAsyncT<T>(method, args));
}