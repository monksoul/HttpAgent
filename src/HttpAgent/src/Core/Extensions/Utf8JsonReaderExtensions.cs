// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     <see cref="Utf8JsonReader" /> 扩展类
/// </summary>
internal static class Utf8JsonReaderExtensions
{
    /// <summary>
    ///     从 <see cref="Utf8JsonReader" /> 中提取原始值，并将其转换为字符串
    /// </summary>
    /// <remarks>支持处理各种类型的原始值（例如数字、布尔值等）。</remarks>
    /// <param name="reader">
    ///     <see cref="Utf8JsonReader" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string ConvertRawValueToString(this Utf8JsonReader reader) =>
        Encoding.UTF8.GetString(reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan);
}