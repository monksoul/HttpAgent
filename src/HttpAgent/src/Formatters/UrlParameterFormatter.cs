// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <inheritdoc cref="IUrlParameterFormatter" />
public class UrlParameterFormatter : IUrlParameterFormatter
{
    /// <inheritdoc />
    public virtual string? Format(object? value, UrlFormattingContext context) =>
        value?.ToCultureString(CultureInfo.InvariantCulture);
}