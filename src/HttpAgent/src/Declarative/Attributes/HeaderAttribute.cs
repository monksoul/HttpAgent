// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式请求标头特性
/// </summary>
/// <remarks>支持多次指定。</remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Parameter,
    AllowMultiple = true)]
public sealed class HeaderAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="HeaderAttribute" />
    /// </summary>
    /// <remarks>特性作用于参数时有效。</remarks>
    public HeaderAttribute()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="HeaderAttribute" />
    /// </summary>
    /// <remarks>
    ///     <para>当特性作用于方法或接口时，若配置字符串不含冒号（:），则表示移除指定请求标头；若含冒号，则以第一个冒号为分隔，左侧为键，右侧为值。</para>
    ///     <para>当特性作用于参数时，则表示添加请求标头，同时设置请求标头键为 <c>name</c> 的值。</para>
    /// </remarks>
    /// <param name="name">请求标头键</param>
    public HeaderAttribute(string name)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    /// <summary>
    ///     <inheritdoc cref="HeaderAttribute" />
    /// </summary>
    /// <param name="name">请求标头键</param>
    /// <param name="value">请求标头的值</param>
    public HeaderAttribute(string name, object? value)
        : this(name) =>
        Value = value;

    /// <summary>
    ///     请求标头键
    /// </summary>
    /// <remarks>该属性优先级低于 <see cref="AliasAs" /> 属性设置的值。</remarks>
    public string? Name { get; set; }

    /// <summary>
    ///     请求标头的值
    /// </summary>
    /// <remarks>当特性作用于参数时，表示默认值。</remarks>
    public object? Value
    {
        get;
        set
        {
            field = value;
            HasSetValue = true;
        }
    }

    /// <summary>
    ///     别名
    /// </summary>
    /// <remarks>
    ///     <para>特性用于参数时有效。</para>
    ///     <para>该属性优先级高于 <see cref="Name" /> 属性设置的值。</para>
    /// </remarks>
    public string? AliasAs { get; set; }

    /// <summary>
    ///     是否转义
    /// </summary>
    public bool Escape { get; set; }

    /// <summary>
    ///     是否替换已存在的请求标头。默认值为 <c>false</c>
    /// </summary>
    public bool Replace { get; set; }

    /// <summary>
    ///     要使用的格式
    /// </summary>
    /// <remarks>仅当 <see cref="Value" /> 实现 <see cref="IFormattable" /> 时有效。</remarks>
    public string? Format { get; set; }

    /// <summary>
    ///     是否设置了值
    /// </summary>
    internal bool HasSetValue { get; private set; }
}