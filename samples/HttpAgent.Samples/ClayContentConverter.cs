namespace HttpAgent.Samples;

public class ClayContentConverter : HttpContentConverterBase<Clay>
{
    /// <inheritdoc />
    public override Clay? Read(HttpContentConverterContext context, CancellationToken cancellationToken = default)
    {
        return AsyncUtility.RunSync(() => ReadAsync(context, cancellationToken));
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
        return AsyncUtility.RunSync(() => ReadAsync(context, cancellationToken));
    }

    /// <inheritdoc />
    public override async Task<dynamic?> ReadAsync(HttpContentConverterContext context,
        CancellationToken cancellationToken = default)
    {
        var str = await context.ResponseMessage.Content.ReadAsStringAsync(cancellationToken);
        return Clay.Parse(str, ClayOptions.Flexible);
    }
}