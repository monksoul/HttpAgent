// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     表单名称命名策略（转换器）
/// </summary>
public enum FormNamingPolicy
{
    /// <summary>
    ///     缺省值
    /// </summary>
    None = 0,

    /// <summary>
    ///     小驼峰命名法
    /// </summary>
    CamelCase,

    /// <summary>
    ///     小写蛇形命名法
    /// </summary>
    SnakeCaseLower,

    /// <summary>
    ///     大写蛇形命名法
    /// </summary>
    SnakeCaseUpper,

    /// <summary>
    ///     小写短横线命名法
    /// </summary>
    KebabCaseLower,

    /// <summary>
    ///     写短横线命名法
    /// </summary>
    KebabCaseUpper
}