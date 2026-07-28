// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     配额管理器接口
/// </summary>
public interface IHttpQuotaManager
{
    /// <summary>
    ///     尝试递增调用计数，并检查是否超过配额
    /// </summary>
    /// <param name="httpClientName"><see cref="HttpClient" /> 实例的配置名称</param>
    /// <param name="quotaKey">配额键</param>
    /// <param name="quotaLimit">
    ///     <see cref="HttpQuotaLimit" />
    /// </param>
    /// <param name="current">递增后的当前调用次数</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    bool TryIncrement(string? httpClientName, string quotaKey, HttpQuotaLimit quotaLimit, out int current);
}