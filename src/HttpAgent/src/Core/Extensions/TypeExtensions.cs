// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     <see cref="Type" /> 扩展类
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    ///     检查类型是否是数组或集合类型
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <param name="underlyingType">元素类型</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsArrayOrCollection(this Type type, [NotNullWhen(true)] out Type? underlyingType)
    {
        underlyingType = null;

        // 检查类型是否是数组类型
        if (type.IsArray)
        {
            underlyingType = type.GetElementType()!;
            return true;
        }

        // 如果不是泛型类型
        if (!type.IsGenericType)
        {
            return false;
        }

        // 获取泛型参数
        var genericArguments = type.GetGenericArguments();

        // 检查类型是否是为单个泛型参数类型且实现了 IEnumerable<> 接口
        if (genericArguments.Length != 1 || !typeof(IEnumerable<>).IsDefinitionEquals(type))
        {
            return false;
        }

        underlyingType = genericArguments[0];
        return true;
    }

    /// <summary>
    ///     检查类型是否是基本类型
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsBasicType(this Type type)
    {
        while (true)
        {
            // 如果是基元类型则直接返回
            if (type.IsPrimitive)
            {
                return true;
            }

            // 处理可空类型
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                return type == typeof(string) || type == typeof(decimal) ||
                       type == typeof(Guid) ||
                       type == typeof(DateTime) ||
                       type == typeof(DateTimeOffset) || type == typeof(DateOnly) || type == typeof(TimeSpan) ||
                       type == typeof(TimeOnly) || type == typeof(char) || type == typeof(IntPtr) ||
                       type == typeof(UIntPtr);
            }

            var underlyingType = type.GetGenericArguments()[0];
            type = underlyingType;
        }
    }

    /// <summary>
    ///     检查类型是否是基本类型或枚举类型或由它们组成的数组或集合类型
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsBaseTypeOrEnumOrCollection(this Type type) =>
        type.IsBasicType() || type.IsEnum || (type.IsArrayOrCollection(out var underlyingType) &&
                                              underlyingType.IsBaseTypeOrEnumOrCollection());

    /// <summary>
    ///     检查类型是否是静态类型
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsStatic(this Type type) => type is { IsSealed: true, IsAbstract: true };

    /// <summary>
    ///     检查类型是否可实例化
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsInstantiable(this Type type) =>
        type is { IsClass: true, IsAbstract: false }
        && !type.IsStatic();

    /// <summary>
    ///     检查类型是否定义了公开无参构造函数
    /// </summary>
    /// <remarks>用于 <see cref="Activator.CreateInstance(Type)" /> 实例化</remarks>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool HasDefinePublicParameterlessConstructor(this Type type) =>
        type.IsInstantiable()
        && type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, Type.EmptyTypes) is not null;

    /// <summary>
    ///     检查类型和指定类型定义是否相等
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <param name="compareType">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsDefinitionEqual(this Type type, Type? compareType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(compareType);

        return type == compareType
               || (type.IsGenericType
                   && compareType.IsGenericType
                   && type.IsGenericTypeDefinition // 💡
                   && type == compareType.GetGenericTypeDefinition());
    }

    /// <summary>
    ///     检查类型和指定类型定义是否相等
    /// </summary>
    /// <remarks>将查找所有派生的基类和实现的接口。</remarks>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <param name="compareType">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsDefinitionEquals(this Type type, Type? compareType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(compareType);

        // 检查类型和指定类型定义是否相等
        if (type.IsDefinitionEqual(compareType))
        {
            return true;
        }

        // 递归查找所有基类
        var baseType = compareType.BaseType;
        while (baseType is not null && baseType != typeof(object))
        {
            // 检查类型和指定类型定义是否相等
            if (type.IsDefinitionEqual(baseType))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        // 检查所有实现的接口定义是否一致
        return compareType.GetInterfaces().Any(type.IsDefinitionEqual);
    }

    /// <summary>
    ///     检查类型是否是 <see cref="KeyValuePair{TKey,TValue}" /> 类型
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsKeyValuePair(this Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);

    /// <summary>
    ///     检查类型是否是键值对集合类型
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <param name="isKeyValuePairCollection">是否是 <see cref="KeyValuePair{TKey,TValue}" /> 集合类型</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsKeyValueCollection(this Type type, out bool isKeyValuePairCollection)
    {
        isKeyValuePairCollection = false;

        // 如果类型不是一个集合类型则直接返回
        if (!typeof(IEnumerable).IsAssignableFrom(type))
        {
            return false;
        }

        // 如果是 Hashtable 或 NameValueCollection 则直接返回
        if (type == typeof(Hashtable) || type == typeof(NameValueCollection))
        {
            return true;
        }

        // 如果是 IDictionary<,> 类型则直接返回
        if ((type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            || type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
        {
            isKeyValuePairCollection = type.GetInterfaces()
                .Any(i => i.IsGenericType &&
                          ((i.GetGenericTypeDefinition() == typeof(ICollection<>) &&
                            i.GetGenericArguments()[0].IsKeyValuePair()) ||
                           (i.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                            i.GetGenericArguments()[0].IsKeyValuePair())));
            return true;
        }

        // 检查是否是 KeyValuePair<,> 数组类型
        if (type.IsArray)
        {
            // 获取数组元素类型
            var elementType = type.GetElementType();

            // 检查元素类型是否是 KeyValuePair<,> 类型
            if (elementType is null || !elementType.IsKeyValuePair())
            {
                return false;
            }

            isKeyValuePairCollection = true;
            return true;
        }

        // 检查是否是 KeyValuePair<,> 集合类型
        if (type is not { IsGenericType: true, GenericTypeArguments.Length: 1 }
            || !type.GenericTypeArguments[0].IsKeyValuePair())
        {
            return false;
        }

        isKeyValuePairCollection = true;
        return true;
    }

    /// <summary>
    ///     获取 <see cref="KeyValuePair{TKey,TValue}" /> 或 <c>Newtonsoft.Json.Linq.JProperty</c> 类型键值属性值访问器
    /// </summary>
    /// <param name="keyValuePairType">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="Tuple{T1,T2}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal static (Func<object, object?> KeyGetter, Func<object, object?> ValueGetter)
        GetKeyValuePairOrJPropertyGetters(this Type keyValuePairType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(keyValuePairType);

        // 检查类型是否是 KeyValuePair<,> 类型或者是 Newtonsoft.Json.Linq.JProperty 类型
        if (keyValuePairType.IsKeyValuePair() || keyValuePairType.FullName == "Newtonsoft.Json.Linq.JProperty")
        {
            // 反射搜索成员方式
            const BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;

            // 创建 Key/Name 和 Value 属性值访问器
            var keyGetter =
                keyValuePairType.CreatePropertyGetter(keyValuePairType.GetProperty("Key", bindingAttr) ??
                                                      keyValuePairType.GetProperty("Name", bindingAttr)!);
            var valueGetter =
                keyValuePairType.CreatePropertyGetter(keyValuePairType.GetProperty("Value",
                    bindingAttr)!);

            return (keyGetter, valueGetter);
        }

        throw new InvalidOperationException(
            $"The type `{keyValuePairType}` is not a `KeyValuePair<,>` or `Newtonsoft.Json.Linq.JProperty` type.");
    }

    /// <summary>
    ///     创建实例属性值设置器
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <remarks>不支持 <c>struct</c> 类型设置属性值。</remarks>
    /// <param name="propertyInfo">
    ///     <see cref="PropertyInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="Action{T1, T2}" />
    /// </returns>
    internal static Action<object, object?> CreatePropertySetter(this Type type, PropertyInfo propertyInfo)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(propertyInfo);

        // 创建一个新的动态方法，并为其命名，命名格式为类型全名_设置_属性名
        var setterMethod = new DynamicMethod(
            $"{type.FullName}_Set_{propertyInfo.Name}",
            null,
            [typeof(object), typeof(object)],
            typeof(TypeExtensions).Module,
            true
        );

        // 获取动态方法的 IL 生成器
        var ilGenerator = setterMethod.GetILGenerator();

        // 获取属性的设置方法，并允许非公开访问
        var setMethod = propertyInfo.GetSetMethod(true);

        // 空检查
        ArgumentNullException.ThrowIfNull(setMethod);

        // 将目标对象加载到堆栈上，并将其转换为所需的类型
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Castclass, type);

        // 将要分配的值加载到堆栈上
        ilGenerator.Emit(OpCodes.Ldarg_1);

        // 检查属性类型是否为值类型
        // 将值转换为属性类型
        // 对值进行拆箱，转换为适当的值类型
        ilGenerator.Emit(propertyInfo.PropertyType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass,
            propertyInfo.PropertyType);

        // 在目标对象上调用设置方法
        ilGenerator.Emit(OpCodes.Callvirt, setMethod);

        // 从动态方法返回
        ilGenerator.Emit(OpCodes.Ret);

        // 创建一个委托并将其转换为适当的 Action 类型
        return (Action<object, object?>)setterMethod.CreateDelegate(typeof(Action<object, object?>));
    }

    /// <summary>
    ///     创建实例属性值访问器
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <param name="propertyInfo">
    ///     <see cref="PropertyInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="Func{T1, T2}" />
    /// </returns>
    internal static Func<object, object?> CreatePropertyGetter(this Type type, PropertyInfo propertyInfo)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(propertyInfo);

        // 创建一个新的动态方法，并为其命名，命名格式为类型全名_获取_属性名
        var dynamicMethod = new DynamicMethod(
            $"{type.FullName}_Get_{propertyInfo.Name}",
            typeof(object),
            [typeof(object)],
            typeof(TypeExtensions).Module,
            true
        );

        // 获取动态方法的 IL 生成器
        var ilGenerator = dynamicMethod.GetILGenerator();

        // 获取属性的获取方法，并允许非公开访问
        var getMethod = propertyInfo.GetGetMethod(true);

        // 空检查
        ArgumentNullException.ThrowIfNull(getMethod);

        // 将目标对象加载到堆栈上
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(type.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, type);

        // 调用获取方法
        ilGenerator.EmitCall(OpCodes.Callvirt, getMethod, null);

        // 如果属性类型为值类型，则装箱为 object 类型
        if (propertyInfo.PropertyType.IsValueType)
        {
            ilGenerator.Emit(OpCodes.Box, propertyInfo.PropertyType);
        }

        // 从动态方法返回
        ilGenerator.Emit(OpCodes.Ret);

        // 创建一个委托并将其转换为适当的 Func 类型
        return (Func<object, object?>)dynamicMethod.CreateDelegate(typeof(Func<object, object?>));
    }

    /// <summary>
    ///     输出类型的友好字符串
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string? ToFriendlyString(this Type? type)
    {
        // 空检查
        if (type is null)
        {
            return null;
        }

        // 检查是否是泛型类型
        if (type.IsGenericType)
        {
            // 获取类型名称
            var typeName = type.Name.Split('`')[0];

            // 获取泛型参数
            var genericArguments = type.GetGenericArguments().Select(ToFriendlyString).ToArray();

            return $"{type.Namespace}.{typeName}<{string.Join(',', genericArguments)}>";
        }

        // 检查是否是非泛型且不是数组类型
        // ReSharper disable once InvertIf
        if (type.IsArray)
        {
            var rank = new string(',', type.GetArrayRank() - 1);
            return $"{type.GetElementType()!.ToFriendlyString()}[{rank}]";
        }

        return type.FullName ?? type.Name;
    }
}