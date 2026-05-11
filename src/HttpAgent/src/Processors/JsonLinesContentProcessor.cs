// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON Lines 内容处理器
/// </summary>
/// <remarks>参考文献：https://jsonlines.org/</remarks>
public class JsonLinesContentProcessor : HttpContentProcessorBase
{
    /// <inheritdoc />
    public override bool CanProcess(object? rawContent, string contentType) =>
        contentType.IsIn(
            ["application/x-ndjson", "application/x-jsonlines", "application/jsonlines", "application/jsonl"],
            StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override HttpContent? Process(object? rawContent, string contentType, Encoding? encoding)
    {
        // 尝试解析 HttpContent 类型
        if (TryProcess(rawContent, contentType, encoding, out var httpContent))
        {
            return httpContent;
        }

        // 检查原始请求内容是否为枚举类型，且其元素类型为引用类型（非字符串）
        if (!rawContent.GetType().IsArrayOrCollection(out var underlyingType) || !underlyingType.IsClass ||
            underlyingType == typeof(string))
        {
            throw new InvalidOperationException(
                $"Expected IEnumerable<T> where T is a class type other than string, but received type `{rawContent.GetType()}`.");
        }

        // 将原始请求内容转换为 IEnumerable 类型
        var enumerable = (IEnumerable)rawContent;

        // 初始化 StringBuilder 实例
        var stringBuilder = new StringBuilder();

        // 解析 JSON 序列化配置
        var jsonSerializerOptions = ResolveJsonSerializerOptions();

        // 构建 JSON Lines 格式内容
        foreach (var item in enumerable)
        {
            stringBuilder.Append(item.ToJsonString(jsonSerializerOptions)).Append('\n');
        }

        // 移除末尾多余换行
        var content = stringBuilder.ToString().TrimEnd('\n');

        // 初始化 StringContent 实例
        var stringContent = new StringContent(content, encoding,
            new MediaTypeHeaderValue(contentType) { CharSet = encoding?.WebName });

        return stringContent;
    }
}