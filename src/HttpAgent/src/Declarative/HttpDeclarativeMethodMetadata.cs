// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式方法元数据
/// </summary>
public sealed class HttpDeclarativeMethodMetadata
{
    /// <summary>
    ///     <inheritdoc cref="HttpDeclarativeMethodMetadata" />
    /// </summary>
    /// <param name="method">被调用方法</param>
    /// <param name="interfaceType">实际被代理的接口类型</param>
    /// <exception cref="ArgumentNullException">被调用方法</exception>
    public HttpDeclarativeMethodMetadata(MethodInfo method, Type interfaceType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(interfaceType);

        Method = method;
        InterfaceType = interfaceType;

        MethodAttributes = method.GetCustomAttributes(true).OfType<Attribute>().ToArray();
        InterfaceAttributes = new[] { interfaceType }.Concat(interfaceType.GetInterfaces())
            .SelectMany(t => t.GetCustomAttributes(true).OfType<Attribute>()).ToArray();
    }

    /// <summary>
    ///     被调用方法
    /// </summary>
    public MethodInfo Method { get; }

    /// <summary>
    ///     实际被代理的接口类型
    /// </summary>
    public Type InterfaceType { get; }

    /// <summary>
    ///     被调用方法的特性列表
    /// </summary>
    public IReadOnlyList<Attribute>? MethodAttributes { get; }

    /// <summary>
    ///     实际被代理的接口类型的特性列表
    /// </summary>
    public IReadOnlyList<Attribute>? InterfaceAttributes { get; }
}