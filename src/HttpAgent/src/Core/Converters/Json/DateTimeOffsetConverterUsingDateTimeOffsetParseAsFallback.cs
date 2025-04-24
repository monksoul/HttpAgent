// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Converters.Json;

/// <summary>
///     <see cref="DateTimeOffset" /> JSON 序列化转换器
/// </summary>
/// <remarks>在不符合 <c>ISO 8601-1:2019</c> 格式的 <see cref="DateTimeOffset" /> 时间使用 <c>DateTimeOffset.Parse</c> 作为回退。</remarks>
public sealed class DateTimeOffsetConverterUsingDateTimeOffsetParseAsFallback : JsonConverter<DateTimeOffset>
{
    /// <inheritdoc />
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 尝试获取 ISO 8601-1:2019 格式时间
        if (!reader.TryGetDateTimeOffset(out var value))
        {
            value = DateTimeOffset.Parse(reader.GetString()!);
        }

        return value;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, value);
}