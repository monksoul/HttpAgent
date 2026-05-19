// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     数值类型扩展类
/// </summary>
public static class NumberExtensions
{
    /// <summary>
    ///     根据指定的单位将字节数进行转换
    /// </summary>
    /// <param name="byteSize">字节数</param>
    /// <param name="unit">单位。可选值为：<c>B</c>, <c>KB</c>, <c>MB</c>, <c>GB</c>, <c>TB</c>, <c>PB</c>, <c>EB</c>。</param>
    /// <returns>
    ///     <see cref="double" />
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static double ToSizeUnits(this double byteSize, string unit)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(unit);

        // 非负检查
        if (byteSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(byteSize),
                $"The `{nameof(byteSize)}` must be non-negative.");
        }

        return unit.ToUpperInvariant() switch
        {
            "B" => byteSize,
            "KB" => byteSize / 1024.0,
            "MB" => byteSize / (1024.0 * 1024),
            "GB" => byteSize / (1024.0 * 1024 * 1024),
            "TB" => byteSize / (1024.0 * 1024 * 1024 * 1024),
            "PB" => byteSize / (1024.0 * 1024 * 1024 * 1024 * 1024),
            "EB" => byteSize / (1024.0 * 1024 * 1024 * 1024 * 1024 * 1024),
            _ => throw new ArgumentOutOfRangeException(nameof(unit), $"Unsupported `{unit}` unit.")
        };
    }

    /// <summary>
    ///     根据指定的单位将字节数进行转换
    /// </summary>
    /// <param name="byteSize">字节数</param>
    /// <param name="unit">单位。可选值为：<c>B</c>, <c>KB</c>, <c>MB</c>, <c>GB</c>, <c>TB</c>, <c>PB</c>, <c>EB</c>。</param>
    /// <returns>
    ///     <see cref="double" />
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static double ToSizeUnits(this long byteSize, string unit) => ((double)byteSize).ToSizeUnits(unit);

    /// <summary>
    ///     将毫秒格式化为更直观的时间单位字符串（如 ms, s, m, h, d, y）
    /// </summary>
    /// <param name="millisecond">毫秒</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string FormatDuration(this long millisecond)
    {
        switch (millisecond)
        {
            case < 0:
                return "-" + (-millisecond).FormatDuration();
            case < 1000:
                return $"{millisecond}ms";
            default:
                var (value, unit) = millisecond switch
                {
                    < 60_000 => (millisecond / 1000.0, "s"),
                    < 3_600_000 => (millisecond / 60_000.0, "m"),
                    < 86_400_000 => (millisecond / 3_600_000.0, "h"),
                    < 31_536_000_000L => (millisecond / 86_400_000.0, "d"),
                    _ => (millisecond / 31_536_000_000.0, "y")
                };

                return FormatValue(value, unit);
        }
    }

    /// <summary>
    ///     将毫秒格式化为更直观的时间单位字符串（如 ms, s, m, h, d, y）
    /// </summary>
    /// <param name="millisecond">毫秒</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string FormatDuration(this double millisecond) => ((long)millisecond).FormatDuration();

    /// <summary>
    ///     格式化数值为指定单位的字符串表示
    /// </summary>
    /// <param name="value">数值</param>
    /// <param name="unit">单位</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string FormatValue(double value, string unit)
    {
        var rounded = Math.Round(value, 1);
        var isInteger = Math.Abs(rounded % 1) < 0.0001;

        return isInteger ? $"{rounded:F0}{unit}" : $"{rounded:F1}{unit}";
    }
}