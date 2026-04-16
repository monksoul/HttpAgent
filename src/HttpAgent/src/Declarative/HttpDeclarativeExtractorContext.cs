// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式提取器上下文
/// </summary>
public sealed class HttpDeclarativeExtractorContext : IServiceProvider
{
    /// <summary>
    ///     冻结参数类型集合
    /// </summary>
    /// <remarks>此类参数类型不应作为外部提取对象。</remarks>
    internal static readonly Type[] _frozenParameterTypes =
    [
        typeof(Action<HttpRequestBuilder>), typeof(Action<HttpMultipartFormDataBuilder>), typeof(HttpCompletionOption),
        typeof(CancellationToken)
    ];

    /// <summary>
    ///     <see cref="IServiceProvider" /> 委托
    /// </summary>
    internal Func<Type, object?>? _serviceProvider;

    /// <summary>
    ///     <inheritdoc cref="HttpDeclarativeExtractorContext" />
    /// </summary>
    /// <param name="method">被调用方法</param>
    /// <param name="args">被调用方法的参数值数组</param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    internal HttpDeclarativeExtractorContext(MethodInfo method, object?[] args,
        IServiceProvider? serviceProvider = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(args);

        // 空检查
        if (serviceProvider is not null)
        {
            var localServiceProvider = serviceProvider;
            InitializeServiceProvider(localServiceProvider.GetService);
        }

        Method = method;
        Args = args;

        // 初始化被调用方法的参数键值字典
        Parameters = method.GetParameters().Select((p, i) => new { Parameter = p, Value = args[i] })
            .ToDictionary(u => u.Parameter, u => u.Value).AsReadOnly();

        // 初始化被调用方法的非冻结类型参数键值字典
        UnFrozenParameters = Parameters.Where(u => !IsFrozenParameter(u.Key)).ToDictionary(u => u.Key, u => u.Value)
            .AsReadOnly();
    }

    /// <summary>
    ///     被调用方法
    /// </summary>
    public MethodInfo Method { get; }

    /// <summary>
    ///     被调用方法的参数值数组
    /// </summary>
    public object?[] Args { get; }

    /// <summary>
    ///     被调用方法的参数键值字典
    /// </summary>
    public IReadOnlyDictionary<ParameterInfo, object?> Parameters { get; }

    /// <summary>
    ///     被调用方法的非冻结类型参数键值字典
    /// </summary>
    public IReadOnlyDictionary<ParameterInfo, object?> UnFrozenParameters { get; }

    /// <inheritdoc />
    public object? GetService(Type serviceType) => _serviceProvider?.Invoke(serviceType);

    /// <summary>
    ///     判断参数是否为冻结参数
    /// </summary>
    /// <remarks>此类参数不应作为外部提取对象。</remarks>
    /// <param name="parameter">
    ///     <see cref="ParameterInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public static bool IsFrozenParameter(ParameterInfo parameter)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(parameter);

        return _frozenParameterTypes.Contains(parameter.ParameterType);
    }

    /// <summary>
    ///     检查被调用方法是否定义了指定特性
    /// </summary>
    /// <param name="attribute">
    ///     <typeparamref name="TAttribute" />
    /// </param>
    /// <param name="inherit">是否在基类中搜索</param>
    /// <typeparam name="TAttribute">
    ///     <see cref="Attribute" />
    /// </typeparam>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public bool IsMethodDefined<TAttribute>([NotNullWhen(true)] out TAttribute? attribute, bool inherit = false)
        where TAttribute : Attribute =>
        Method.IsDefined(out attribute, inherit);

    /// <summary>
    ///     获取被调用方法指定特性的所有实例
    /// </summary>
    /// <param name="inherit">是否在基类中搜索</param>
    /// <param name="methodScanFirst">是否优先查找 <see cref="MethodInfo" /> 的特性。默认值为：<c>true</c>。</param>
    /// <typeparam name="TAttribute">
    ///     <see cref="Attribute" />
    /// </typeparam>
    /// <returns>
    ///     <typeparamref name="TAttribute" /><c>[]</c>
    /// </returns>
    public TAttribute[]? GetMethodDefinedCustomAttributes<TAttribute>(bool inherit = false, bool methodScanFirst = true)
        where TAttribute : Attribute =>
        Method.GetDefinedCustomAttributes<TAttribute>(inherit, methodScanFirst);

    /// <summary>
    ///     初始化 <see cref="IServiceProvider" />
    /// </summary>
    /// <param name="serviceProvider"><see cref="IServiceProvider" /> 委托</param>
    internal void InitializeServiceProvider(Func<Type, object?> serviceProvider) => _serviceProvider = serviceProvider;
}