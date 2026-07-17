// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="ObjectContentConverter{TResult}" /> 工厂
/// </summary>
public interface IObjectContentConverterFactory
{
    /// <summary>
    ///     根据目标类型获取对应的 <see cref="IHttpContentConverter{TResult}" /> 实例
    /// </summary>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <param name="context">
    ///     <see cref="HttpContentConverterContext" />
    /// </param>
    /// <returns>
    ///     <see cref="IHttpContentConverter{TResult}" />
    /// </returns>
    IHttpContentConverter<TResult> GetConverter<TResult>(HttpContentConverterContext context);

    /// <summary>
    ///     根据目标类型获取对应的 <see cref="IHttpContentConverter" /> 实例
    /// </summary>
    /// <param name="resultType">转换的目标类型</param>
    /// <param name="context">
    ///     <see cref="HttpContentConverterContext" />
    /// </param>
    /// <returns>
    ///     <see cref="IHttpContentConverter" />
    /// </returns>
    IHttpContentConverter GetConverter(Type resultType, HttpContentConverterContext context);
}