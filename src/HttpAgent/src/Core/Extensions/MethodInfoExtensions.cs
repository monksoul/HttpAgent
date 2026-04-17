// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     <see cref="MethodInfo" /> 扩展类
/// </summary>
internal static class MethodInfoExtensions
{
    /// <summary>
    ///     输出方法签名的友好字符串
    /// </summary>
    /// <param name="method">
    ///     <see cref="MethodInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string? ToFriendlyString(this MethodInfo? method)
    {
        // 空检查
        if (method is null)
        {
            return null;
        }

        // 获取方法的基本信息
        var methodName = method.Name;
        var returnType = method.ReturnType.ToFriendlyString();

        // 处理泛型方法
        var genericArguments = method.IsGenericMethod
            ? method.GetGenericArguments().Select(t => t.ToFriendlyString()).ToArray()
            : [];

        // 获取参数列表
        var parameters = method.GetParameters().Select(p => p.ParameterType.ToFriendlyString());

        // 组合字符串
        var genericPart = genericArguments.Length != 0 ? $"<{string.Join(',', genericArguments)}>" : string.Empty;

        return $"{returnType} {methodName}{genericPart}({string.Join(", ", parameters)})";
    }
}