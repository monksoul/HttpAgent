// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <inheritdoc cref="IUrlParameterFormatter" />
public class UrlParameterFormatter : IUrlParameterFormatter
{
    /// <inheritdoc />
    public virtual IEnumerable<KeyValuePair<string, string?>>? Format(UrlFormattingContext context, string key,
        IEnumerable<object?> values) =>
        values.Select(value => FormatValue(context, value)).OfType<string>()
            .Select(formattedValue => new KeyValuePair<string, string?>(key, formattedValue));

    /// <summary>
    ///     格式化单个参数值
    /// </summary>
    /// <param name="context">
    ///     <see cref="UrlFormattingContext" />
    /// </param>
    /// <param name="value">参数值</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string? FormatValue(UrlFormattingContext context, object? value) =>
        (value switch
        {
            Func<object?> valueProvider => valueProvider(),
            Func<UrlFormattingContext, object?> valueProvider => valueProvider(context),
            _ => value
        })?.ToInvariantCultureString();
}