// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     泛型 <see cref="IHttpContentConverter" /> 响应内容转换器
/// </summary>
/// <param name="GenericType">转换的目标类型的泛型类型</param>
/// <param name="Factory">泛型 <see cref="IHttpContentConverter" /> 工厂委托</param>
public sealed record GenericHttpContentConverter(Type GenericType, Func<Type[], IHttpContentConverter> Factory);