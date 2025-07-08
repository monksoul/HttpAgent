// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式多部分表单对象内容特性
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class MultipartObjectAttribute : MultipartAttribute
{
    /// <summary>
    ///     <inheritdoc cref="MultipartObjectAttribute" />
    /// </summary>
    public MultipartObjectAttribute() => AsFormItem = false;
}