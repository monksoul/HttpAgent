// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     <see cref="ICollection{T}" /> 扩展类
/// </summary>
internal static class ICollectionExtensions
{
    /// <summary>
    ///     判断集合是否为空
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="collection">
    ///     <see cref="ICollection{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsNullOrEmpty<T>([NotNullWhen(false)] this ICollection<T>? collection) =>
        collection is null
        || collection.Count == 0;
}