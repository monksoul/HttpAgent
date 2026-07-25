// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     <see cref="object" /> 扩展类
/// </summary>
internal static class ObjectExtensions
{
    /// <summary>
    ///     将对象转换为基于特定文化的字符串表示形式
    /// </summary>
    /// <param name="obj">
    ///     <see cref="object" />
    /// </param>
    /// <param name="culture">
    ///     <see cref="CultureInfo" />
    /// </param>
    /// <param name="enumAsString">指示是否将枚举类型的值作为名称输出，默认值为：<c>true</c>。若为 <c>false</c>，则输出枚举的值</param>
    /// <param name="separator">集合类型分隔符</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string? ToCultureString(this object? obj, CultureInfo culture, bool enumAsString = true,
        string separator = ",")
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(culture);

        return obj switch
        {
            null => null,
            string s => s,
            DateTime dt => dt.ToString("o", culture),
            DateTimeOffset df => df.ToString("o", culture),
            DateOnly od => od.ToString("yyyy-MM-dd", culture),
            TimeOnly ot => ot.ToString("HH':'mm':'ss", culture),
            Enum e when enumAsString => e.ToString(),
            Enum e => Convert.ChangeType(e, Enum.GetUnderlyingType(e.GetType())).ToString(),
            IEnumerable e and not string when typeof(IEnumerable<>).IsDefinitionEquals(e.GetType()) => string.Join(
                separator, e.Cast<object>()),
            _ => obj.ToString()
        };
    }

    /// <summary>
    ///     将对象转换为基于 <see cref="CultureInfo.InvariantCulture" /> 文化的字符串表示形式
    /// </summary>
    /// <param name="obj">
    ///     <see cref="object" />
    /// </param>
    /// <param name="enumAsString">指示是否将枚举类型的值作为名称输出，默认值为：<c>true</c>。若为 <c>false</c>，则输出枚举的值</param>
    /// <param name="separator">集合类型分隔符</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string?
        ToInvariantCultureString(this object? obj, bool enumAsString = true, string separator = ",") =>
        obj.ToCultureString(CultureInfo.InvariantCulture, enumAsString, separator);

    /// <summary>
    ///     将对象转换为 <see cref="IDictionary{TKey,TValue}" /> 类型对象
    /// </summary>
    /// <param name="obj">
    ///     <see cref="object" />
    /// </param>
    /// <param name="returnPropertyInfo">
    ///     当无法通过已知类型转换而回退到反射读取公开属性时，如果为 <c>true</c> 则字典的值将保存 <see cref="PropertyInfo" />
    ///     对象，否则保存属性的实际值。默认值为 <c>false</c>
    /// </param>
    /// <returns>
    ///     <see cref="IDictionary{TKey,TValue}" />
    /// </returns>
    /// <exception cref="NotSupportedException"></exception>
    internal static IDictionary<object, object?>? ObjectToDictionary(this object? obj, bool returnPropertyInfo = false)
    {
        // 空检查
        if (obj is null)
        {
            return null;
        }

        // 获取对象类型
        var objType = obj.GetType();

        // 初始化不受支持的类型转换的异常消息字符串
        var notSupportedExceptionMessage =
            $"Conversion of parameter 'obj' from type `{objType}` to type `IDictionary<object, object?>` is not supported.";

        // 检查类型是否是基本类型或 void 类型
        if (objType.IsBasicType() || objType == typeof(void))
        {
            throw new NotSupportedException(notSupportedExceptionMessage);
        }

        // 检查类型是否是枚举类型
        if (objType.IsEnum)
        {
            // 转换为字典类型并返回
            return new Dictionary<object, object?> { { Enum.GetName(objType, obj)!, Convert.ToInt32(obj) } };
        }

        // 检查类型是否是 KeyValuePair<,> 单个类型
        if (objType.IsKeyValuePair())
        {
            // 获取 Key 和 Value 属性值访问器
            var getters = objType.GetKeyValuePairOrJPropertyGetters();

            // 转换为字典类型并返回
            return new Dictionary<object, object?> { { getters.KeyGetter(obj)!, getters.ValueGetter(obj) } };
        }

        // 处理 System.Text.Json 类型
        switch (obj)
        {
            case JsonDocument jsonDocument:
                return jsonDocument.RootElement.ObjectToDictionary(returnPropertyInfo);
            case JsonElement { ValueKind: JsonValueKind.Object } jsonElement:
                // 转换为字典类型并返回
                return jsonElement.EnumerateObject().ToDictionary<JsonProperty, object, object?>(
                    jsonProperty => jsonProperty.Name,
                    jsonProperty => jsonProperty.Value);
        }

        // 检查类型是否是键值对集合类型
        if (objType.IsKeyValueCollection(out var isKeyValuePairCollection))
        {
            // === 处理 Hashtable 和 NameValueCollection 集合类型 ===
            switch (obj)
            {
                case Hashtable hashtable:
                    return hashtable.Cast<DictionaryEntry>().ToDictionary(entry => entry.Key, entry => entry.Value);
                case NameValueCollection nameValueCollection:
                    return nameValueCollection.AllKeys.ToDictionary(object (key) => key!,
                        object? (key) => nameValueCollection[key]);
            }

            // === 处理非 KeyValuePair<,> 集合类型 ===
            if (!isKeyValuePairCollection)
            {
                // 将对象转化为 IDictionary 接口对象
                var dictionaryObj = (IDictionary)obj;

                // 转换为字典类型并返回
                return dictionaryObj.Count == 0
                    ? new Dictionary<object, object?>()
                    : dictionaryObj.Keys.Cast<object?>().ToDictionary(key => key!, key => dictionaryObj[key!]);
            }

            // === 处理 KeyValuePair<,> 集合类型 ===
            var keyValuePairs = ((IEnumerable)obj).Cast<object?>().ToArray();

            // 空检查
            if (keyValuePairs.Length == 0)
            {
                return new Dictionary<object, object?>();
            }

            // 获取 KeyValuePair<,> 集合中元素类型
            var keyValuePairType = keyValuePairs.First()?.GetType()!;

            // 获取 Key 和 Value 属性值访问器
            var getters = keyValuePairType.GetKeyValuePairOrJPropertyGetters();

            // 转换为字典类型并返回
            return keyValuePairs.GroupBy(keyValuePair => getters.KeyGetter(keyValuePair!)!).ToDictionary(
                group => group.Key,
                group => group.Count() == 1
                    ? getters.ValueGetter(group.First()!)
                    : group.Select(keyValuePair => getters.ValueGetter(keyValuePair!)).ToArray());
        }

        try
        {
            // 初始化反射搜索成员方式
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            // 根据 returnPropertyInfo 参数决定字典的值是 PropertyInfo 还是属性实际值
            if (returnPropertyInfo)
            {
                return objType.GetProperties(bindingFlags).Where(property => property.CanRead).ToDictionary(
                    object (property) => AliasAsUtility.GetPropertyName(property, out _),
                    object? (property) => property);
            }

            return objType.GetProperties(bindingFlags).Where(property => property.CanRead).ToDictionary(
                object (property) => AliasAsUtility.GetPropertyName(property, out _),
                property => property.GetValue(obj));
        }
        catch (Exception e)
        {
            throw new AggregateException(new NotSupportedException(notSupportedExceptionMessage), e);
        }
    }
}