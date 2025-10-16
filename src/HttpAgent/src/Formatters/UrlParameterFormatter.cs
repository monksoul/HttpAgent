// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <inheritdoc cref="IUrlParameterFormatter" />
public class UrlParameterFormatter : IUrlParameterFormatter
{
    /// <inheritdoc />
    public virtual string? Format(object? value, UrlFormattingContext context) => DefaultFormatter(value, context);

    /// <inheritdoc cref="Format" />
    /// <remarks>默认格式化行为。</remarks>
    public static string? DefaultFormatter(object? value, UrlFormattingContext context)
    {
        // 检查值是否为委托，如果是则获取实际值
        var resolvedValue = value switch
        {
            // 处理 () => value; 情况
            Func<object?> valueProvider => valueProvider(),
            // 处理 context => value; 情况
            Func<UrlFormattingContext, object?> valueProvider => valueProvider(context),
            _ => value
        };

        return resolvedValue?.ToCultureString(CultureInfo.InvariantCulture);
    }
}