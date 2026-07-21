// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 请求的响应结果
/// </summary>
/// <param name="ResponseMessage">
///     <see cref="HttpResponseMessage" />
/// </param>
/// <param name="RequestDuration">请求耗时（毫秒）</param>
public sealed record HttpResponseResult(HttpResponseMessage? ResponseMessage, long RequestDuration);