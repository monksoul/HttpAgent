// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpContentConverterFactoryTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));

        using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpContentConverterFactory1 = new HttpContentConverterFactory(serviceProvider, logger, null!, null!);
        Assert.NotNull(httpContentConverterFactory1.ServiceProvider);
        Assert.NotNull(httpContentConverterFactory1._logger);
        Assert.NotNull(httpContentConverterFactory1._converters);
        Assert.Equal(5, httpContentConverterFactory1._converters.Count);
        Assert.Equal(
            [
                typeof(HttpResponseMessageConverter), typeof(StringContentConverter), typeof(ByteArrayContentConverter),
                typeof(StreamContentConverter), typeof(VoidContentConverter)
            ],
            httpContentConverterFactory1._converters.Select(u => u.Key));
        Assert.NotNull(httpContentConverterFactory1._genericConverters);
        Assert.Equal(2, httpContentConverterFactory1._genericConverters.Count);

        Assert.Equal(typeof(IAsyncEnumerable<>), httpContentConverterFactory1._genericConverters.Keys.First());
        Assert.NotNull(httpContentConverterFactory1._genericConverters[typeof(IAsyncEnumerable<>)].First()
            .Invoke([typeof(ObjectModel)]));

        Assert.Equal(typeof(HttpRemoteResult<>), httpContentConverterFactory1._genericConverters.Keys.Last());
        Assert.NotNull(httpContentConverterFactory1._genericConverters[typeof(HttpRemoteResult<>)].Last()
            .Invoke([typeof(ObjectModel)]));

        Assert.NotNull(httpContentConverterFactory1._genericConverterCache);
        Assert.Empty(httpContentConverterFactory1._genericConverterCache);

        var httpContentConverterFactory2 =
            new HttpContentConverterFactory(serviceProvider, logger, [new CustomStringContentConverter()], null!);
        Assert.NotNull(httpContentConverterFactory2._converters);
        Assert.Equal(6, httpContentConverterFactory2._converters.Count);
        Assert.Equal(
            [
                typeof(HttpResponseMessageConverter), typeof(StringContentConverter), typeof(ByteArrayContentConverter),
                typeof(StreamContentConverter), typeof(VoidContentConverter),
                typeof(CustomStringContentConverter)
            ],
            httpContentConverterFactory2._converters.Select(u => u.Key));

        var httpContentConverterFactory3 =
            new HttpContentConverterFactory(serviceProvider, logger,
                [new StringContentConverter(), new ByteArrayContentConverter()], null!);
        Assert.NotNull(httpContentConverterFactory3._converters);
        Assert.Equal(5, httpContentConverterFactory3._converters.Count);
        Assert.Equal(
            [
                typeof(HttpResponseMessageConverter), typeof(StringContentConverter), typeof(ByteArrayContentConverter),
                typeof(StreamContentConverter), typeof(VoidContentConverter)
            ],
            httpContentConverterFactory3._converters.Select(u => u.Key));

        var httpContentConverterFactory4 =
            new HttpContentConverterFactory(serviceProvider, logger, [new CustomStringContentConverter()], [
                new GenericHttpContentConverter(typeof(IAsyncEnumerable<>), typeArgs =>
                    (IHttpContentConverter)Activator.CreateInstance(
                        typeof(CustomAsyncEnumerableContentConverter<>).MakeGenericType(typeArgs[0]))!)
            ]);
        Assert.NotNull(httpContentConverterFactory4._genericConverters);
        Assert.Equal(2, httpContentConverterFactory4._genericConverters.Count);
        Assert.Equal(2, httpContentConverterFactory4._genericConverters[typeof(IAsyncEnumerable<>)].Count);

        var httpContentConverterFactory5 =
            new HttpContentConverterFactory(serviceProvider, logger, [new CustomStringContentConverter()], [
                new GenericHttpContentConverter(typeof(IEnumerable<>), typeArgs =>
                    (IHttpContentConverter)Activator.CreateInstance(
                        typeof(CustomAsyncEnumerableContentConverter<>).MakeGenericType(typeArgs[0]))!)
            ]);
        Assert.NotNull(httpContentConverterFactory5._genericConverters);
        Assert.Equal(3, httpContentConverterFactory5._genericConverters.Count);
        Assert.Single(httpContentConverterFactory5._genericConverters[typeof(IAsyncEnumerable<>)]);
        Assert.Single(httpContentConverterFactory5._genericConverters[typeof(HttpRemoteResult<>)]);
        Assert.Single(httpContentConverterFactory5._genericConverters[typeof(IEnumerable<>)]);
    }

    [Fact]
    public void GetConverter_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        services.TryAddSingleton<IObjectContentConverterFactory, ObjectContentConverterFactory>();
        using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpContentConverterFactory = new HttpContentConverterFactory(serviceProvider, logger, null!, null!);
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var converterContext = new HttpContentConverterContext(httpResponseMessage);

        Assert.Equal(typeof(HttpResponseMessageConverter),
            httpContentConverterFactory
                .GetConverter<HttpResponseMessage>(converterContext).GetType());

        Assert.Equal(typeof(StringContentConverter),
            httpContentConverterFactory.GetConverter<string>(converterContext).GetType());

        Assert.Equal(typeof(ByteArrayContentConverter),
            httpContentConverterFactory.GetConverter<byte[]>(converterContext).GetType());

        Assert.Equal(typeof(StreamContentConverter),
            httpContentConverterFactory.GetConverter<Stream>(converterContext).GetType());
        Assert.Equal(typeof(VoidContentConverter),
            httpContentConverterFactory.GetConverter<VoidContent>(converterContext).GetType());
        Assert.Equal(typeof(ObjectContentConverter<int>),
            httpContentConverterFactory.GetConverter<int>(converterContext).GetType());
        Assert.Equal(typeof(ObjectContentConverter<ObjectModel>),
            httpContentConverterFactory.GetConverter<ObjectModel>(converterContext).GetType());
        Assert.Equal(typeof(AsyncEnumerableContentConverter<ObjectModel>),
            httpContentConverterFactory.GetConverter<IAsyncEnumerable<ObjectModel>>(converterContext).GetType());

        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Options.AddOrUpdate(Constants.ENABLE_JSON_RESPONSE_WRAPPER_KEY, "TRUE");
        httpResponseMessage.RequestMessage = httpRequestMessage;
        Assert.Equal(typeof(ObjectContentConverter<HttpResponseMessage>),
            httpContentConverterFactory.GetConverter<HttpResponseMessage>(converterContext).GetType());
        Assert.Equal(typeof(ObjectContentConverter<string>),
            httpContentConverterFactory.GetConverter<string>(converterContext).GetType());
        Assert.Equal(typeof(ObjectContentConverter<byte[]>),
            httpContentConverterFactory.GetConverter<byte[]>(converterContext).GetType());
        Assert.Equal(typeof(ObjectContentConverter<Stream>),
            httpContentConverterFactory.GetConverter<Stream>(converterContext).GetType());
        Assert.Equal(typeof(VoidContentConverter),
            httpContentConverterFactory.GetConverter<VoidContent>(converterContext).GetType());
        Assert.Equal(typeof(ObjectContentConverter<int>),
            httpContentConverterFactory.GetConverter<int>(converterContext).GetType());
        Assert.Equal(typeof(ObjectContentConverter<ObjectModel>),
            httpContentConverterFactory.GetConverter<ObjectModel>(converterContext).GetType());
    }

    [Fact]
    public void GetConverter_Of_Customize_ObjectContentConverter_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote(builder =>
        {
            builder.UseObjectContentConverterFactory<CustomObjectContentConverterFactory>();
        });

        using var serviceProvider = services.BuildServiceProvider();
        var httpContentConverterFactory =
            serviceProvider.GetRequiredService<IHttpContentConverterFactory>() as HttpContentConverterFactory;
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        Assert.Equal(typeof(CustomObjectContentConverter<ObjectModel>),
            httpContentConverterFactory!.GetConverter<ObjectModel>(new HttpContentConverterContext(httpResponseMessage))
                .GetType());
    }

    [Fact]
    public void GetConverter_WithCustomize_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, logger, [new CustomByteArrayContentConverter()], null!);
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        Assert.Equal(typeof(CustomStringContentConverter),
            httpContentConverterFactory
                .GetConverter<string>(new HttpContentConverterContext(httpResponseMessage,
                    [new CustomStringContentConverter()]))
                .GetType());
        Assert.Equal(typeof(CustomByteArrayContentConverter),
            httpContentConverterFactory.GetConverter<byte[]>(new HttpContentConverterContext(httpResponseMessage))
                .GetType());
    }

    [Fact]
    public void Read_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, logger, null!, null!);

        var result = httpContentConverterFactory.Read<string>(new HttpContentConverterContext(httpResponseMessage));
        Assert.Equal("furion", result.Result);

        var result2 =
            httpContentConverterFactory.Read<HttpResponseMessage>(new HttpContentConverterContext(httpResponseMessage));
        Assert.Equal(result2.Result, httpResponseMessage);
    }

    [Fact]
    public async Task ReadAsync_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        await using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, logger, null!, null!);

        var result =
            await httpContentConverterFactory.ReadAsync<string>(new HttpContentConverterContext(httpResponseMessage));
        Assert.Equal("furion", result.Result);

        var result2 =
            await httpContentConverterFactory.ReadAsync<HttpResponseMessage>(
                new HttpContentConverterContext(httpResponseMessage));
        Assert.Equal(result2.Result, httpResponseMessage);
    }

    [Fact]
    public async Task ReadAsync_WithCancellationToken_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        await using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, logger, null!, null!);

        using var cancellationTokenSource = new CancellationTokenSource();

        var result = await httpContentConverterFactory.ReadAsync<string>(
            new HttpContentConverterContext(httpResponseMessage),
            cancellationTokenSource.Token);
        Assert.Equal("furion", result.Result);
    }

    [Fact]
    public void GetConverter_WithType_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        services.TryAddSingleton<IObjectContentConverterFactory, ObjectContentConverterFactory>();
        using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpContentConverterFactory = new HttpContentConverterFactory(serviceProvider, logger, null!, null!);
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var converterContext = new HttpContentConverterContext(httpResponseMessage);

        Assert.Equal(typeof(StringContentConverter),
            httpContentConverterFactory.GetConverter(typeof(string), converterContext).GetType());
        Assert.Equal(typeof(ByteArrayContentConverter),
            httpContentConverterFactory.GetConverter(typeof(byte[]), converterContext).GetType());
        Assert.Equal(typeof(StreamContentConverter),
            httpContentConverterFactory.GetConverter(typeof(Stream), converterContext).GetType());
        Assert.Equal(typeof(ObjectContentConverter),
            httpContentConverterFactory.GetConverter(typeof(int), converterContext).GetType());
        Assert.Equal(typeof(ObjectContentConverter),
            httpContentConverterFactory.GetConverter(typeof(ObjectModel), converterContext).GetType());
        Assert.Equal(typeof(AsyncEnumerableContentConverter<ObjectModel>),
            httpContentConverterFactory.GetConverter(typeof(IAsyncEnumerable<ObjectModel>), converterContext)
                .GetType());

        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Options.AddOrUpdate(Constants.ENABLE_JSON_RESPONSE_WRAPPER_KEY, "TRUE");
        httpResponseMessage.RequestMessage = httpRequestMessage;
        Assert.Equal(typeof(ObjectContentConverter),
            httpContentConverterFactory.GetConverter(typeof(string), converterContext).GetType());
        Assert.Equal(typeof(ObjectContentConverter),
            httpContentConverterFactory.GetConverter(typeof(byte[]), converterContext).GetType());
        Assert.Equal(typeof(ObjectContentConverter),
            httpContentConverterFactory.GetConverter(typeof(Stream), converterContext).GetType());
        Assert.Equal(typeof(VoidContentConverter),
            httpContentConverterFactory.GetConverter(typeof(VoidContent), converterContext).GetType());
        Assert.Equal(typeof(ObjectContentConverter),
            httpContentConverterFactory.GetConverter(typeof(int), converterContext).GetType());
        Assert.Equal(typeof(ObjectContentConverter),
            httpContentConverterFactory.GetConverter(typeof(ObjectModel), converterContext).GetType());
    }

    [Fact]
    public void GetConverter_WithType_Of_Customize_ObjectContentConverter_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote(builder =>
        {
            builder.UseObjectContentConverterFactory<CustomObjectContentConverterFactory>();
        });

        using var serviceProvider = services.BuildServiceProvider();
        var httpContentConverterFactory =
            serviceProvider.GetRequiredService<IHttpContentConverterFactory>() as HttpContentConverterFactory;
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        Assert.Equal(typeof(CustomObjectContentConverter),
            httpContentConverterFactory!
                .GetConverter(typeof(ObjectModel), new HttpContentConverterContext(httpResponseMessage)).GetType());
    }

    [Fact]
    public void GetConverter_WithType_WithCustomize_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, logger, [new CustomByteArrayContentConverter()], null!);
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        Assert.Equal(typeof(CustomStringContentConverter),
            httpContentConverterFactory
                .GetConverter(typeof(string),
                    new HttpContentConverterContext(httpResponseMessage, [new CustomStringContentConverter()]))
                .GetType());
        Assert.Equal(typeof(CustomByteArrayContentConverter),
            httpContentConverterFactory
                .GetConverter(typeof(byte[]), new HttpContentConverterContext(httpResponseMessage)).GetType());
    }

    [Fact]
    public void Read_WithType_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, logger, null!, null!);

        var result =
            httpContentConverterFactory.Read(typeof(string), new HttpContentConverterContext(httpResponseMessage));
        Assert.Equal("furion", result.Result);

        var result2 = httpContentConverterFactory.Read(typeof(HttpResponseMessage),
            new HttpContentConverterContext(httpResponseMessage));
        Assert.Equal(result2.Result, httpResponseMessage);
    }

    [Fact]
    public async Task ReadAsync_WithType_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        await using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, logger, null!, null!);

        var result =
            await httpContentConverterFactory.ReadAsync(typeof(string),
                new HttpContentConverterContext(httpResponseMessage));
        Assert.Equal("furion", result.Result);

        var result2 = await httpContentConverterFactory.ReadAsync(typeof(HttpResponseMessage),
            new HttpContentConverterContext(httpResponseMessage));
        Assert.Equal(result2.Result, httpResponseMessage);
    }

    [Fact]
    public async Task ReadAsync_WithType_WithCancellationToken_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        await using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, logger, null!, null!);

        using var cancellationTokenSource = new CancellationTokenSource();

        var result = await httpContentConverterFactory.ReadAsync(typeof(string),
            new HttpContentConverterContext(httpResponseMessage),
            cancellationTokenSource.Token);
        Assert.Equal("furion", result.Result);
    }

    [Fact]
    public void LogContentConversionError_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, logger, null!, null!);

        httpContentConverterFactory.LogContentConversionError(typeof(string), httpResponseMessage,
            new Exception("出错了"));
    }

    [Fact]
    public void TryResolveGenericConverter_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        services.TryAddSingleton<IObjectContentConverterFactory, ObjectContentConverterFactory>();
        using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpContentConverterFactory = new HttpContentConverterFactory(serviceProvider, logger, null!, null!);

        Assert.Null(httpContentConverterFactory.TryResolveGenericConverter(typeof(ObjectModel)));
        Assert.Empty(httpContentConverterFactory._genericConverterCache);

        var genericConverter =
            httpContentConverterFactory.TryResolveGenericConverter(typeof(IAsyncEnumerable<ObjectModel>));
        Assert.NotNull(genericConverter);
        Assert.Single(httpContentConverterFactory._genericConverterCache);

        var httpContentConverterFactory2 = new HttpContentConverterFactory(serviceProvider, logger, null!, [
            new GenericHttpContentConverter(typeof(IAsyncEnumerable<>), typeArgs =>
                (IHttpContentConverter)Activator.CreateInstance(
                    typeof(CustomAsyncEnumerableContentConverter<>).MakeGenericType(typeArgs[0]))!)
        ]);

        Assert.Null(httpContentConverterFactory2.TryResolveGenericConverter(typeof(ObjectModel)));
        Assert.Single(httpContentConverterFactory._genericConverterCache);

        var genericConverter2 =
            httpContentConverterFactory2.TryResolveGenericConverter(typeof(IAsyncEnumerable<ObjectModel>));
        Assert.NotNull(genericConverter2);
        Assert.True(genericConverter2 is CustomAsyncEnumerableContentConverter<ObjectModel>);
    }
}