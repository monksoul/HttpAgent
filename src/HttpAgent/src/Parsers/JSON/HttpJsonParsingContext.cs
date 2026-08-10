// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON 解析上下文
/// </summary>
public sealed class HttpJsonParsingContext
{
    /// <summary>
    ///     <inheritdoc cref="HttpJsonParsingContext" />
    /// </summary>
    /// <param name="rootObject">根 JSON 对象</param>
    /// <exception cref="ArgumentNullException"></exception>
    internal HttpJsonParsingContext(JsonObject rootObject)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(rootObject);

        RootObject = rootObject;
    }

    /// <summary>
    ///     根 JSON 对象
    /// </summary>
    public JsonObject RootObject { get; }

    /// <summary>
    ///     尝试获取指定属性名的 <see cref="JsonNode" /> 节点
    /// </summary>
    /// <param name="propertyName">属性名</param>
    /// <param name="node">
    ///     <see cref="JsonNode" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool TryGetNode(string propertyName, out JsonNode? node)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(propertyName);

        return RootObject.TryGetPropertyValue(propertyName, out node) && node is not null;
    }

    /// <summary>
    ///     获取指定属性名的 <see cref="JsonNode" /> 节点
    /// </summary>
    /// <param name="propertyName">属性名</param>
    /// <returns>
    ///     <see cref="JsonNode" /> 或 <c>null</c>
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public JsonNode? GetNode(string propertyName)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(propertyName);

        RootObject.TryGetPropertyValue(propertyName, out var node);

        return node;
    }

    /// <summary>
    ///     检查 JSON 对象中是否包含指定属性
    /// </summary>
    /// <param name="propertyName">属性名</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool ContainsProperty(string propertyName)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(propertyName);

        return RootObject.ContainsKey(propertyName);
    }
}