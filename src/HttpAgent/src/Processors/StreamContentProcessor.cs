// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     流内容处理器
/// </summary>
public class StreamContentProcessor : HttpContentProcessorBase
{
    /// <inheritdoc />
    public override bool CanProcess(HttpContentProcessorContext context) =>
        context.RawContent is StreamContent or Stream;

    /// <inheritdoc />
    public override HttpContent? Process(HttpContentProcessorContext context)
    {
        // 尝试解析 HttpContent 类型
        if (TryProcess(context, out var httpContent))
        {
            return httpContent;
        }

        // 检查是否是流类型
        if (context.RawContent is Stream stream)
        {
            // 初始化 StreamContent 实例
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType =
                new MediaTypeHeaderValue(context.ContentType) { CharSet = context.Encoding?.WebName };

            return streamContent;
        }

        throw new InvalidOperationException(
            $"Expected a stream, but received an object of type `{context.RawContent!.GetType()}`.");
    }
}