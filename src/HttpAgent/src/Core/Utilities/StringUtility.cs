// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Utilities;

/// <summary>
///     提供字符串实用方法
/// </summary>
public static class StringUtility
{
    /// <summary>
    ///     格式化键值集合摘要
    /// </summary>
    /// <param name="keyValues">键值集合</param>
    /// <param name="summary">摘要</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string? FormatKeyValuesSummary(IEnumerable<KeyValuePair<string, IEnumerable<string>>> keyValues,
        string? summary = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(keyValues);

        // 获取键值集合数量
        var keyValuePairs = keyValues as KeyValuePair<string, IEnumerable<string>>[] ?? keyValues.ToArray();
        var count = keyValuePairs.Length;

        // 空检查
        if (count == 0)
        {
            return null;
        }

        // 注册 CodePagesEncodingProvider，使得程序能够识别并使用 Windows 代码页中的各种编码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // 获取最长键名长度用于对齐键名字符串
        var totalByteCount = keyValuePairs.Max(h => h.Key.Length) + 5;

        // 初始化 StringBuilder 实例
        var stringBuilder = new StringBuilder();

        // 检查是否设置了摘要
        var hasSummary = !string.IsNullOrWhiteSpace(summary);

        // 逐条构建摘要信息
        var index = 0;
        foreach (var (key, value) in keyValuePairs)
        {
            // 检查是否包含摘要，如果有则添加制表符（两个空白）
            if (hasSummary)
            {
                stringBuilder.Append("  ");
            }

            // 获取格式化后的值
            var formatValue = AddTabToEachLine(string.Join(", ", value), true);

            // 处理空 Key 问题
            if (!string.IsNullOrWhiteSpace(key))
            {
                stringBuilder.Append($"{(key + ':').PadStringToByteLength(totalByteCount)} {formatValue}");
            }
            else
            {
                stringBuilder.Append($"{string.Join(", ", formatValue)}");
            }

            // 处理最后一行空行问题
            if (index < count - 1)
            {
                stringBuilder.Append("\r\n");
            }

            index++;
        }

        // 获取字符串
        var formatString = stringBuilder.ToString();

        return hasSummary ? $"\e[34m\e[1m{summary}:\e[0m \r\n{formatString}" : formatString;
    }

    /// <summary>
    ///     在字符串每一行添加制表符（两个空白）
    /// </summary>
    /// <param name="input">文本</param>
    /// <param name="skipFirstLine">是否跳过第一行</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string? AddTabToEachLine(string? input, bool skipFirstLine = false)
    {
        // 空检查
        if (input is null)
        {
            return input;
        }

        // 使用 Environment.NewLine 以确保跨平台兼容性
        return string.Join(Environment.NewLine, input.Split([Environment.NewLine, "\n"], StringSplitOptions.None)
            .Select((line, i) => (skipFirstLine && i == 0 ? string.Empty : "  ") + line));
    }
}