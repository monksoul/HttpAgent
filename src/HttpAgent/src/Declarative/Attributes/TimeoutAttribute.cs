// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式超时时间特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class TimeoutAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="TimeoutAttribute" />
    /// </summary>
    /// <param name="milliseconds">
    ///     <para>超时时间（毫秒）</para>
    ///     <para>
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>设置为 <c>-1</c> 表示无超时</description>
    ///             </item>
    ///             <item>
    ///                 <description>设置为 <c>0</c> 将立即取消请求</description>
    ///             </item>
    ///             <item>
    ///                 <description>负值（除 <c>-1</c> 毫秒外）将引发 <see cref="ArgumentOutOfRangeException" /></description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </param>
    public TimeoutAttribute(double milliseconds) => Timeout = milliseconds;

    /// <summary>
    ///     超时时间（毫秒）
    /// </summary>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>
    ///             <description>设置为 <c>-1</c> 表示无超时</description>
    ///         </item>
    ///         <item>
    ///             <description>设置为 <c>0</c> 将立即取消请求</description>
    ///         </item>
    ///         <item>
    ///             <description>负值（除 <c>-1</c> 毫秒外）将引发 <see cref="ArgumentOutOfRangeException" /></description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public double Timeout { get; set; }
}