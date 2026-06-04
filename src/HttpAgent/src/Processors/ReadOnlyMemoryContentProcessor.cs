// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="ReadOnlyMemory{T}" /> 内容处理器
/// </summary>
public class ReadOnlyMemoryContentProcessor : HttpContentProcessorBase
{
    /// <inheritdoc />
    public override bool CanProcess(HttpContentProcessorContext context) =>
        context.RawContent is ReadOnlyMemoryContent or ReadOnlyMemory<byte>;

    /// <inheritdoc />
    public override HttpContent? Process(HttpContentProcessorContext context)
    {
        // 尝试解析 HttpContent 类型
        if (TryProcess(context, out var httpContent))
        {
            return httpContent;
        }

        // 检查是否是 ReadOnlyMemory<byte> 类型
        if (context.RawContent is ReadOnlyMemory<byte> readOnlyMemory)
        {
            // 初始化 ReadOnlyMemoryContent 实例
            var readOnlyMemoryContent = new ReadOnlyMemoryContent(readOnlyMemory);
            readOnlyMemoryContent.Headers.ContentType = new MediaTypeHeaderValue(context.ContentType)
            {
                CharSet = context.Encoding?.WebName
            };

            return readOnlyMemoryContent;
        }

        throw new InvalidOperationException(
            $"Expected a ReadOnlyMemory<byte>, but received an object of type `{context.RawContent!.GetType()}`.");
    }
}