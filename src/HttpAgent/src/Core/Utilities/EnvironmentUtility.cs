// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Utilities;

/// <summary>
///     提供环境实用方法
/// </summary>
public static class EnvironmentUtility
{
    /// <summary>
    ///     判断是否是开发环境
    /// </summary>
    public static readonly bool IsDevelopment =
        string.Equals(Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT"), "Development",
            StringComparison.OrdinalIgnoreCase) ||
        string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development",
            StringComparison.OrdinalIgnoreCase) ||
        Debugger.IsAttached;
}