// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON 响应反序列化包装器
/// </summary>
public sealed class JsonResponseWrapper
{
    /// <summary>
    ///     目标结果属性值访问器缓存字典
    /// </summary>
    internal readonly ConcurrentDictionary<Type, Func<object, object?>> _getterCache;

    /// <summary>
    ///     <inheritdoc cref="JsonResponseWrapper" />
    /// </summary>
    /// <param name="genericType">包装泛型类型定义类型</param>
    /// <param name="propertyName">目标结果属性名</param>
    /// <exception cref="ArgumentException"></exception>
    public JsonResponseWrapper(Type genericType, string propertyName)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(genericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        // 检查是否是泛型定义类型且参数数量为一个
        if (!genericType.IsGenericTypeDefinition || genericType.GetGenericArguments().Length != 1)
        {
            throw new ArgumentException(
                $"The provided type must be a generic type definition (e.g., typeof(ApiResult<>)). Actual type: {genericType}.",
                nameof(genericType));
        }

        GenericType = genericType;
        PropertyName = propertyName;
        _getterCache = new ConcurrentDictionary<Type, Func<object, object?>>();
    }

    /// <summary>
    ///     包装泛型类型定义类型
    /// </summary>
    public Type GenericType { get; }

    /// <summary>
    ///     目标结果属性名
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    ///     获取目标结果值
    /// </summary>
    /// <param name="instance">
    ///     包装类型的具体实例（例如 <![CDATA[ApiResult<T>]]> 的实例）
    /// </param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public object? GetResultValue(object? instance)
    {
        // 空检查
        if (instance is null)
        {
            return null;
        }

        // 检查实例类型是否和包装泛型类型定义类型一致
        var actualType = instance.GetType();
        if (!actualType.IsGenericType || actualType.GetGenericTypeDefinition() != GenericType)
        {
            throw new ArgumentException(
                $"The instance type '{actualType}' is not a constructed generic type of '{GenericType}'.",
                nameof(instance));
        }

        // 获取或构建属性值访问器
        var getter = _getterCache.GetOrAdd(actualType, BuildGetter);

        return getter(instance);
    }

    /// <summary>
    ///     为指定的具体泛型类型动态构建属性值访问器
    /// </summary>
    /// <param name="concreteType">
    ///     已构造的泛型类型（例如 <![CDATA[ApiResult<string>]]>）
    /// </param>
    /// <returns>
    ///     <see cref="Func{T,TResult}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal Func<object, object?> BuildGetter(Type concreteType)
    {
        // 获取属性对象
        var property = concreteType.GetProperty(PropertyName, BindingFlags.Public | BindingFlags.Instance) ??
                       throw new InvalidOperationException(
                           $"Property '{PropertyName}' not found on type '{concreteType}'.");

        // 构建 instance => instance.属性名 表达式
        var param = Expression.Parameter(typeof(object), "instance");
        var cast = Expression.Convert(param, concreteType);
        var propertyAccess = Expression.Property(cast, property);
        var convertToObject = Expression.Convert(propertyAccess, typeof(object));

        // 编译 Lambda 表达式
        return Expression.Lambda<Func<object, object?>>(convertToObject, param).Compile();
    }
}