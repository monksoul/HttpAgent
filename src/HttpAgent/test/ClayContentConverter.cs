// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ClayContentConverter : HttpContentConverterBase<Clay>
{
    /// <inheritdoc />
    public override Clay? Read(HttpContentConverterContext context, CancellationToken cancellationToken = default)
    {
        var str = AsyncUtility.RunSync(() => context.ResponseMessage.Content.ReadAsStringAsync(cancellationToken));

        return Clay.Parse(str, ClayOptions.Flexible);
    }

    /// <inheritdoc />
    public override async Task<Clay?> ReadAsync(HttpContentConverterContext context,
        CancellationToken cancellationToken = default)
    {
        var str = await context.ResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        return Clay.Parse(str, ClayOptions.Flexible);
    }
}

public class DynamicContentConverter : HttpContentConverterBase<dynamic>
{
    /// <inheritdoc />
    public override dynamic? Read(HttpContentConverterContext context, CancellationToken cancellationToken = default)
    {
        var str = AsyncUtility.RunSync(() => context.ResponseMessage.Content.ReadAsStringAsync(cancellationToken));

        return Clay.Parse(str, ClayOptions.Flexible);
    }

    /// <inheritdoc />
    public override async Task<dynamic?> ReadAsync(HttpContentConverterContext context,
        CancellationToken cancellationToken = default)
    {
        var str = await context.ResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        return Clay.Parse(str, ClayOptions.Flexible);
    }
}