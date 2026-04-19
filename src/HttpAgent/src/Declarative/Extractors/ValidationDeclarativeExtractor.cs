// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="ValidationAttribute" /> 特性提取器
/// </summary>
internal sealed class ValidationDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 检查方法或接口是否贴有 [SuppressValidation] 特性
        if (context.IsMethodDefined<SuppressValidationAttribute>(out _, true))
        {
            return;
        }

        // 遍历所有非冻结类型参数并进行验证操作
        foreach (var (parameter, value) in context.UnFrozenParameters)
        {
            ValidateParameter(parameter, value);
        }
    }

    /// <summary>
    ///     验证参数
    /// </summary>
    /// <param name="parameter">
    ///     <see cref="ParameterInfo" />
    /// </param>
    /// <param name="value">参数的值</param>
    /// <exception cref="ValidationException"></exception>
    internal static void ValidateParameter(ParameterInfo parameter, object? value)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(parameter);

        // 获取参数名和参数类型
        var parameterName = parameter.Name!;
        var parameterType = parameter.ParameterType;

        // 空检查
        if (value is null)
        {
            // 优先验证 [Required] 特性
            if (parameter.IsDefined(typeof(RequiredAttribute), true))
            {
                throw new ValidationException(parameter.GetCustomAttribute<RequiredAttribute>(true)
                    ?.FormatErrorMessage(parameterName));
            }

            return;
        }

        // 检查参数是否贴有验证特性
        if (parameter.IsDefined(typeof(ValidationAttribute), true))
        {
            // 验证单个值类型
            Validator.ValidateValue(value, new ValidationContext(value) { MemberName = parameterName },
                parameter.GetCustomAttributes<ValidationAttribute>(true));
        }

        // 检查类型是否是基本类型或枚举类型或由它们组成的数组或集合类型
        if (!parameterType.IsBaseTypeOrEnumOrCollection())
        {
            // 验证复杂对象类型
            Validator.ValidateObject(value, new ValidationContext(value), true);
        }
    }
}