// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     <see cref="IEnumerable" /> 扩展类
/// </summary>
internal static class IEnumerableExtensions
{
    /// <summary>
    ///     合并两个集合
    /// </summary>
    /// <param name="first">
    ///     <see cref="IEnumerable{T}" />
    /// </param>
    /// <param name="second">
    ///     <see cref="IEnumerable{T}" />
    /// </param>
    /// <typeparam name="TSource">集合元素的类型</typeparam>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    public static IEnumerable<TSource> ConcatIgnoreNull<TSource>(this IEnumerable<TSource>? first,
        IEnumerable<TSource>? second) =>
        (first ?? []).Concat(second ?? []);
}