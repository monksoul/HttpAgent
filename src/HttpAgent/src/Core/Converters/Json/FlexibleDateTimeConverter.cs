// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Converters.Json;

/// <summary>
///     <see cref="DateTime" /> JSON 序列化转换器
/// </summary>
public partial class FlexibleDateTimeConverter : JsonConverter<DateTime>
{
    private static readonly DateTime s_epoch = new(1970, 1, 1, 0, 0, 0);
    private static readonly Regex s_regex = Regex();

    /// <inheritdoc />
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var formatted = reader.GetString()!;
        var match = s_regex.Match(formatted);

        // 尝试获取 Unix epoch 日期格式
        // 参考文献：https://learn.microsoft.com/zh-cn/dotnet/standard/datetime/system-text-json-support#use-unix-epoch-date-format
        if (match.Success && long.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture,
                out var unixTime))
        {
            return s_epoch.AddMilliseconds(unixTime);
        }

        // 尝试获取日期
        if (reader.TryGetDateTime(out var value))
        {
            return value;
        }

        // 尝试获取 ISO 8601-1:2019 日期格式
        if (!DateTime.TryParse(formatted, out value))
        {
            throw new JsonException();
        }

        return value;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, value);

    [GeneratedRegex(@"^/Date\(([+-]*\d+)\)/$", RegexOptions.CultureInvariant)]
    private static partial Regex Regex();
}