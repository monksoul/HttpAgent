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
    public override bool CanProcess(HttpContentProcessorContext context) =>
        context.ContentType.IsIn(
            ["application/x-ndjson", "application/x-jsonlines", "application/jsonlines", "application/jsonl"],
            StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override HttpContent? Process(HttpContentProcessorContext context)
    {
        // 尝试解析 HttpContent 类型
        if (TryProcess(context, out var httpContent))
        {
            return httpContent;
        }

        // 检查原始请求内容是否为枚举类型，且其元素类型为引用类型（非字符串）
        if (!context.RawContent!.GetType().IsArrayOrCollection(out var underlyingType) || !underlyingType.IsClass ||
            underlyingType == typeof(string))
        {
            throw new InvalidOperationException(
                $"Expected IEnumerable<T> where T is a class type other than string, but received type `{context.RawContent.GetType()}`.");
        }

        // 将原始请求内容转换为 IEnumerable 类型
        var enumerable = (IEnumerable)context.RawContent;

        // 初始化 StringBuilder 实例
        var stringBuilder = new StringBuilder();

        // 解析 JSON 序列化配置
        var jsonSerializerOptions = ResolveJsonSerializerOptions(context.HttpClientName);

        // 构建 JSON Lines 格式内容
        foreach (var item in enumerable)
        {
            stringBuilder.Append(item.ToJsonString(jsonSerializerOptions)).Append('\n');
        }

        // 移除末尾多余换行
        var content = stringBuilder.ToString().TrimEnd('\n');

        // 初始化 StringContent 实例
        var stringContent = new StringContent(content, context.Encoding,
            new MediaTypeHeaderValue(context.ContentType) { CharSet = context.Encoding?.WebName });

        return stringContent;
    }
}