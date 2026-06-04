// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     字节数组内容处理器
/// </summary>
public class ByteArrayContentProcessor : HttpContentProcessorBase
{
    /// <inheritdoc />
    public override bool CanProcess(HttpContentProcessorContext context) =>
        context.RawContent is (ByteArrayContent or byte[]) and not (FormUrlEncodedContent or StringContent);

    /// <inheritdoc />
    public override HttpContent? Process(HttpContentProcessorContext context)
    {
        // 尝试解析 HttpContent 类型
        if (TryProcess(context, out var httpContent))
        {
            return httpContent;
        }

        // 检查是否是字节数组类型
        if (context.RawContent is byte[] bytes)
        {
            // 初始化 ByteArrayContent 实例
            var byteArrayContent = new ByteArrayContent(bytes);
            byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(context.ContentType)
            {
                CharSet = context.Encoding?.WebName
            };

            return byteArrayContent;
        }

        throw new InvalidOperationException(
            $"Expected a byte array, but received an object of type `{context.RawContent!.GetType()}`.");
    }
}