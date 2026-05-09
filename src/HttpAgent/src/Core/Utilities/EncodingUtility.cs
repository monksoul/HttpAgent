// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Utilities;

/// <summary>
///     提供编码实用方法
/// </summary>
public static class EncodingUtility
{
    /// <summary>
    ///     <inheritdoc cref="EncodingUtility" />
    /// </summary>
    static EncodingUtility()
    {
        // 注册 CodePagesEncodingProvider，使得程序能够识别并使用 Windows 代码页中的各种编码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    /// <summary>
    ///     显式初始化方法
    /// </summary>
    /// <remarks>仅用于触发静态构造函数。</remarks>
    public static void Initialize()
    {
    }
}