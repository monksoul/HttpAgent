// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     数值类型拓展类
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
    public static double ToSizeUnits(this long byteSize, string unit) => ToSizeUnits((double)byteSize, unit);

    /// <summary>
    ///     将毫秒数格式化为更直观的时间单位字符串（如 ms, s, min, h, d, y）
    /// </summary>
    /// <param name="millisecond">时间间隔的毫秒数</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string FormatDuration(this long millisecond)
    {
        while (true)
        {
            // 如果时间小于 1000 毫秒（即 1 秒），直接以毫秒为单位返回
            if (millisecond < 1000)
            {
                return $"{millisecond}ms";
            }

            // 将毫秒转换为秒
            var seconds = millisecond / 1000.0;
            if (seconds < 60)
            {
                var val = Math.Round(seconds * 10) / 10;
                if (!(val >= 60))
                {
                    return val % 1 == 0 ? $"{val:F0}s" : $"{val:F1}s";
                }

                millisecond = (long)(val * 1000);
                continue;
            }

            // 将秒转换为分钟
            var minutes = seconds / 60;
            if (minutes < 60)
            {
                var val = Math.Round(minutes * 10) / 10;
                if (!(val >= 60))
                {
                    return val % 1 == 0 ? $"{val:F0}min" : $"{val:F1}min";
                }

                millisecond = (long)(val * 60 * 1000);
                continue;
            }

            // 将分钟转换为小时
            var hours = minutes / 60;
            if (hours < 24)
            {
                var val = Math.Round(hours * 10) / 10;
                if (!(val >= 24))
                {
                    return val % 1 == 0 ? $"{val:F0}h" : $"{val:F1}h";
                }

                millisecond = (long)(val * 60 * 60 * 1000);
                continue;
            }

            // 将小时转换为天
            var days = hours / 24;
            if (days < 365)
            {
                var val = Math.Round(days * 10) / 10;
                if (!(val >= 365))
                {
                    return val % 1 == 0 ? $"{val:F0}d" : $"{val:F1}d";
                }

                millisecond = (long)(val * 24 * 60 * 60 * 1000);
                continue;
            }

            // 将天数转换为年
            var years = days / 365;
            var finalVal = Math.Round(years * 10) / 10;
            return finalVal % 1 == 0 ? $"{finalVal:F0}y" : $"{finalVal:F1}y";
        }
    }

    /// <summary>
    ///     将毫秒数格式化为更直观的时间单位字符串（如 ms, s, min, h, d, y）
    /// </summary>
    /// <param name="millisecond">时间间隔的毫秒数</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string FormatDuration(this double millisecond) => FormatDuration((long)millisecond);
}