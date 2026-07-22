// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     URL 参数格式化程序
/// </summary>
public interface IUrlParameterFormatter
{
    /// <summary>
    ///     格式化
    /// </summary>
    /// <param name="context">
    ///     <see cref="UrlFormattingContext" />
    /// </param>
    /// <param name="key">参数名</param>
    /// <param name="values">参数值集合</param>
    /// <returns><see cref="KeyValuePair{TKey,TValue}" /> 集合</returns>
    IEnumerable<KeyValuePair<string, string?>>? Format(UrlFormattingContext context, string key,
        IEnumerable<object?> values);
}