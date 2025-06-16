namespace HttpAgent.Tests;

public class ClayContentConverter : HttpContentConverterBase<Clay>
{
    /// <inheritdoc />
    public override Clay? Read(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default)
    {
        var str = httpResponseMessage.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();

        return Clay.Parse(str, ClayOptions.Flexible);
    }

    /// <inheritdoc />
    public override async Task<Clay?> ReadAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default)
    {
        var str = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        return Clay.Parse(str, ClayOptions.Flexible);
    }
}

public class DynamicContentConverter : HttpContentConverterBase<dynamic>
{
    /// <inheritdoc />
    public override dynamic? Read(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default)
    {
        var str = httpResponseMessage.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();

        return Clay.Parse(str, ClayOptions.Flexible);
    }

    /// <inheritdoc />
    public override async Task<dynamic?> ReadAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default)
    {
        var str = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        return Clay.Parse(str, ClayOptions.Flexible);
    }
}