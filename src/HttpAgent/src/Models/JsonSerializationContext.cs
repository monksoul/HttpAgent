// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON 序列化上下文
/// </summary>
/// <param name="ResultType">目标类型</param>
/// <param name="JsonSerializerOptions">
///     <see cref="JsonSerializerOptions" />
/// </param>
/// <param name="GetResultValue">获取目标类型值的委托</param>
public sealed record JsonSerializationContext(
    Type ResultType,
    JsonSerializerOptions JsonSerializerOptions,
    Func<object?, HttpResponseMessage, object?> GetResultValue);