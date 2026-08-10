// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <inheritdoc cref="RequestEventHandlerAttribute" />
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class RequestEventHandlerAttribute<TRequestEventHandler> : RequestEventHandlerAttribute
    where TRequestEventHandler : IHttpRequestEventHandler
{
    /// <inheritdoc />
    public RequestEventHandlerAttribute() : base(typeof(TRequestEventHandler))
    {
    }
}

/// <summary>
///     HTTP 声明式事件处理程序特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public class RequestEventHandlerAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="RequestEventHandlerAttribute" />
    /// </summary>
    /// <param name="handlerType">实现 <see cref="IHttpRequestEventHandler" /> 接口的类型</param>
    public RequestEventHandlerAttribute(Type handlerType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(handlerType);

        HandlerType = handlerType;
    }

    /// <summary>
    ///     实现 <see cref="IHttpRequestEventHandler" /> 接口的类型
    /// </summary>
    public Type HandlerType { get; }
}