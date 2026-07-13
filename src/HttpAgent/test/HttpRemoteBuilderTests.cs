// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteBuilderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Null(builder._genericHttpContentConverterProviders);
        Assert.Null(builder._httpContentConverterProviders);
        Assert.Null(builder._httpContentProcessorProviders);
        Assert.Null(builder._httpDeclarativeExtractors);
        Assert.Null(builder._objectContentConverterFactoryType);
        Assert.Null(builder._httpDeclarativeTypes);

        Assert.NotNull(builder._httpRequestPipelineHandlerTypes);
        Assert.Equal(13, builder._httpRequestPipelineHandlerTypes.Count);
        Assert.Equal([
            typeof(SuppressExceptionPipelineHandler),
            typeof(ResponseAssertionPipelineHandler),
            typeof(ResponseProfilerPipelineHandler),
            typeof(RequestEventPipelineHandler),
            typeof(TimeoutPipelineHandler),
            typeof(RetryPipelineHandler),
            typeof(TokenManagementPipelineHandler),
            typeof(AutoRedirectPipelineHandler),
            typeof(StatusCodePipelineHandler),
            typeof(ContentLengthValidationPipelineHandler),
            typeof(RequestBuilderPipelineHandler),
            typeof(RequestProfilerPipelineHandler),
            typeof(SendCorePipelineHandler)
        ], builder._httpRequestPipelineHandlerTypes);
    }

    [Fact]
    public void AddHttpContentProcessors_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Throws<ArgumentNullException>(() => { builder.AddHttpContentProcessors(null!); });
    }

    [Fact]
    public void AddHttpContentProcessors_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();

        builder.AddHttpContentProcessors(() => []);
        Assert.NotNull(builder._httpContentProcessorProviders);

        builder.AddHttpContentProcessors(() => [new StringContentProcessor()]);
        Assert.NotNull(builder._httpContentProcessorProviders);

        // 运行时将抛异常
        builder.AddHttpContentProcessors(() => [null!, new StringContentProcessor()]);
        Assert.NotNull(builder._httpContentProcessorProviders);

        var builder2 = new HttpRemoteBuilder();
        builder2.AddHttpContentProcessors(() => [new StringContentProcessor()]);
        builder2.AddHttpContentProcessors(() => [new StringContentProcessor()]);
        Assert.NotNull(builder2._httpContentProcessorProviders);
        Assert.Equal(2, builder2._httpContentProcessorProviders.Count);
    }

    [Fact]
    public void AddHttpContentConverters_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Throws<ArgumentNullException>(() => { builder.AddHttpContentConverters(null!); });
    }

    [Fact]
    public void AddHttpContentConverters_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();

        builder.AddHttpContentConverters(() => []);
        Assert.NotNull(builder._httpContentConverterProviders);

        builder.AddHttpContentConverters(() =>
            [new StringContentConverter(), new ObjectContentConverter<int>()]);
        Assert.NotNull(builder._httpContentConverterProviders);

        // 运行时将抛异常
        builder.AddHttpContentConverters(() => [null!, new StringContentConverter()]);
        Assert.NotNull(builder._httpContentConverterProviders);

        var builder2 = new HttpRemoteBuilder();
        builder2.AddHttpContentConverters(() => [new StringContentConverter()]);
        builder2.AddHttpContentConverters(() => [new StringContentConverter()]);
        Assert.NotNull(builder2._httpContentConverterProviders);
        Assert.Equal(2, builder2._httpContentConverterProviders.Count);
    }

    [Fact]
    public void AddGenericHttpContentConverters_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Throws<ArgumentNullException>(() => { builder.AddGenericHttpContentConverters(null!); });
    }

    [Fact]
    public void AddGenericHttpContentConverters_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();

        builder.AddGenericHttpContentConverters(() => []);
        Assert.NotNull(builder._genericHttpContentConverterProviders);

        var genericHttpContentConverter = new GenericHttpContentConverter(typeof(IAsyncEnumerable<>), typeArgs =>
            (IHttpContentConverter)Activator.CreateInstance(
                typeof(AsyncEnumerableContentConverter<>).MakeGenericType(typeArgs[0]))!);

        builder.AddGenericHttpContentConverters(() => [genericHttpContentConverter]);
        Assert.NotNull(builder._genericHttpContentConverterProviders);

        // 运行时将抛异常
        builder.AddGenericHttpContentConverters(() => [null!, genericHttpContentConverter]);
        Assert.NotNull(builder._genericHttpContentConverterProviders);

        var builder2 = new HttpRemoteBuilder();
        builder2.AddGenericHttpContentConverters(() => [genericHttpContentConverter]);
        builder2.AddGenericHttpContentConverters(() => [genericHttpContentConverter]);
        Assert.NotNull(builder2._genericHttpContentConverterProviders);
        Assert.Equal(2, builder2._genericHttpContentConverterProviders.Count);
    }

    [Fact]
    public void UseObjectContentConverterFactory_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.UseObjectContentConverterFactory(null!));

        var exception = Assert.Throws<ArgumentException>(() =>
            builder.UseObjectContentConverterFactory(typeof(NotImplementObjectContentConverterFactory)));
        Assert.Equal(
            $"`{typeof(NotImplementObjectContentConverterFactory)}` type is not assignable from `{typeof(IObjectContentConverterFactory)}`. (Parameter 'factoryType')",
            exception.Message);
    }

    [Fact]
    public void UseObjectContentConverterFactory_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();
        builder.UseObjectContentConverterFactory(typeof(CustomObjectContentConverterFactory));

        Assert.NotNull(builder._objectContentConverterFactoryType);
        Assert.Equal(typeof(CustomObjectContentConverterFactory), builder._objectContentConverterFactoryType);

        var builder2 = new HttpRemoteBuilder();
        builder2.UseObjectContentConverterFactory<CustomObjectContentConverterFactory>();

        Assert.NotNull(builder2._objectContentConverterFactoryType);
        Assert.Equal(typeof(CustomObjectContentConverterFactory), builder2._objectContentConverterFactoryType);
    }

    [Fact]
    public void AddHttpDeclarative_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.AddHttpDeclarative(null!));

        var exception = Assert.Throws<ArgumentException>(() =>
            builder.AddHttpDeclarative(typeof(INonHttpTest)));
        Assert.Equal(
            $"The type `{typeof(INonHttpTest)}` must be a closed or non-generic interface that implements `{typeof(IHttpDeclarative)}`. (Parameter 'declarativeType')",
            exception.Message);

        Assert.Throws<ArgumentException>(() =>
            builder.AddHttpDeclarative(typeof(HttpTest)));
        Assert.Throws<ArgumentException>(() =>
            builder.AddHttpDeclarative(typeof(IHttpTestBase<>)));
    }

    [Fact]
    public void AddHttpDeclarative_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();
        builder.AddHttpDeclarative(typeof(IHttpTest));

        Assert.NotNull(builder._httpDeclarativeTypes);
        Assert.Single(builder._httpDeclarativeTypes);
        Assert.Equal(typeof(IHttpTest), builder._httpDeclarativeTypes.First());

        builder.AddHttpDeclarative<IHttpTest>();
        Assert.Single(builder._httpDeclarativeTypes);
        Assert.Equal(typeof(IHttpTest), builder._httpDeclarativeTypes.First());

        builder.AddHttpDeclarative<IHttpTestBase<string>>();
        Assert.Equal(2, builder._httpDeclarativeTypes.Count);
        Assert.Equal(typeof(IHttpTestBase<string>), builder._httpDeclarativeTypes.Last());
    }

    [Fact]
    public void AddHttpDeclaratives_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.AddHttpDeclaratives(null!));
    }

    [Fact]
    public void AddHttpDeclaratives_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();
        // ReSharper disable once RedundantExplicitParamsArrayCreation
        builder.AddHttpDeclaratives([typeof(IHttpTest), typeof(IHttpTest), typeof(IHttpTest2)]);

        Assert.NotNull(builder._httpDeclarativeTypes);
        Assert.Equal(2, builder._httpDeclarativeTypes.Count);
        Assert.Equal(typeof(IHttpTest), builder._httpDeclarativeTypes.First());
        Assert.Equal(typeof(IHttpTest2), builder._httpDeclarativeTypes.Last());
    }

    [Fact]
    public void AddHttpDeclarativesFromAssemblies_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.AddHttpDeclarativesFromAssemblies(null!));
    }

    [Fact]
    public void AddHttpDeclarativesFromAssemblies_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();
        // ReSharper disable once RedundantExplicitParamsArrayCreation
        builder.AddHttpDeclarativesFromAssemblies([typeof(HttpRemoteBuilderTests).Assembly, null]);

        Assert.NotNull(builder._httpDeclarativeTypes);
        Assert.Equal(55, builder._httpDeclarativeTypes.Count);
    }

    [Fact]
    public void AddHttpDeclarativeExtractors_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Throws<ArgumentNullException>(() => { builder.AddHttpDeclarativeExtractors(null!); });
    }

    [Fact]
    public void AddHttpDeclarativeExtractors_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();

        builder.AddHttpDeclarativeExtractors(() => []);
        Assert.NotNull(builder._httpDeclarativeExtractors);

        builder.AddHttpDeclarativeExtractors(() => [new HttpClientNameDeclarativeExtractor()]);
        Assert.NotNull(builder._httpDeclarativeExtractors);

        // 运行时将抛异常
        builder.AddHttpDeclarativeExtractors(() => [null!, new HttpClientNameDeclarativeExtractor()]);
        Assert.NotNull(builder._httpDeclarativeExtractors);

        var builder2 = new HttpRemoteBuilder();
        builder2.AddHttpDeclarativeExtractors(() => [new HttpClientNameDeclarativeExtractor()]);
        builder2.AddHttpDeclarativeExtractors(() => [new HttpClientNameDeclarativeExtractor()]);
        Assert.NotNull(builder2._httpDeclarativeExtractors);
        Assert.Equal(2, builder2._httpDeclarativeExtractors.Count);
    }

    [Fact]
    public void AddHttpDeclarativeExtractorsFromAssemblies_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Throws<ArgumentNullException>(() => { builder.AddHttpDeclarativeExtractorsFromAssemblies(null!); });
    }

    [Fact]
    public void AddHttpDeclarativeExtractorsFromAssemblies_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();

        builder.AddHttpDeclarativeExtractorsFromAssemblies(typeof(HttpRemoteBuilderTests).Assembly, null);
        Assert.NotNull(builder._httpDeclarativeExtractors);
        Assert.Single(builder._httpDeclarativeExtractors);
    }

    [Fact]
    public void AddPipelineHandler_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.AddPipelineHandler(null!));

        var exception = Assert.Throws<ArgumentException>(() =>
            builder.AddPipelineHandler(typeof(NotImplementHttpRequestPipelineHandler)));
        Assert.Equal(
            $"`{typeof(NotImplementHttpRequestPipelineHandler)}` type is not assignable from `{typeof(IHttpRequestPipelineHandler)}`. (Parameter 'handlerType')",
            exception.Message);

        var exception2 = Assert.Throws<ArgumentOutOfRangeException>(() =>
            builder.AddPipelineHandler(typeof(CustomPipelineHandler), -1));
        Assert.Equal("Index must be between 0 and 13. (Parameter 'index')", exception2.Message);
    }

    [Fact]
    public void AddPipelineHandler_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();
        builder.AddPipelineHandler(typeof(CustomPipelineHandler)).AddPipelineHandler<CustomPipelineHandler>();

        Assert.Equal(15, builder._httpRequestPipelineHandlerTypes.Count);
        Assert.Equal(typeof(CustomPipelineHandler), builder._httpRequestPipelineHandlerTypes.First());
        Assert.Equal(typeof(CustomPipelineHandler), builder._httpRequestPipelineHandlerTypes.Skip(1).First());

        builder.AddPipelineHandler<CustomPipelineHandler>(builder._httpRequestPipelineHandlerTypes.Count);
        Assert.Equal(typeof(CustomPipelineHandler), builder._httpRequestPipelineHandlerTypes.Last());
    }

    [Fact]
    public void Build_Default_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new HttpRemoteBuilder();

        builder.Build(services);
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpRemoteLogger));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpClientFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentProcessorFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentConverterFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpRemoteService));
        Assert.True(services.First(u => u.ServiceType == typeof(IObjectContentConverterFactory)).ImplementationType ==
                    typeof(ObjectContentConverterFactory));

        Assert.Contains(services, u => u.ServiceType == typeof(SuppressExceptionPipelineHandler));
        Assert.Contains(services, u => u.ServiceType == typeof(ResponseAssertionPipelineHandler));
        Assert.Contains(services, u => u.ServiceType == typeof(ResponseProfilerPipelineHandler));
        Assert.Contains(services, u => u.ServiceType == typeof(RequestEventPipelineHandler));
        Assert.Contains(services, u => u.ServiceType == typeof(TimeoutPipelineHandler));
        Assert.Contains(services, u => u.ServiceType == typeof(RetryPipelineHandler));
        Assert.Contains(services, u => u.ServiceType == typeof(TokenManagementPipelineHandler));
        Assert.Contains(services, u => u.ServiceType == typeof(AutoRedirectPipelineHandler));
        Assert.Contains(services, u => u.ServiceType == typeof(StatusCodePipelineHandler));
        Assert.Contains(services, u => u.ServiceType == typeof(ContentLengthValidationPipelineHandler));
        Assert.Contains(services, u => u.ServiceType == typeof(RequestBuilderPipelineHandler));
        Assert.Contains(services, u => u.ServiceType == typeof(RequestProfilerPipelineHandler));
        Assert.Contains(services, u => u.ServiceType == typeof(SendCorePipelineHandler));

        Assert.Contains(services, u => u.ServiceType == typeof(HttpAccessTokenManager));

        Assert.Equal(46, services.Count);
    }

    [Fact]
    public void Build_Duplicate_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new HttpRemoteBuilder();

        builder.Build(services);
        builder.Build(services);
        builder.Build(services);

        Assert.Contains(services, u => u.ServiceType == typeof(IHttpRemoteLogger));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpClientFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentProcessorFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentConverterFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpRemoteService));
        Assert.True(services.First(u => u.ServiceType == typeof(IObjectContentConverterFactory)).ImplementationType ==
                    typeof(ObjectContentConverterFactory));
        Assert.Equal(48, services.Count);
    }

    [Fact]
    public void Build_UseObjectContentConverterFactory_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new HttpRemoteBuilder().UseObjectContentConverterFactory<CustomObjectContentConverterFactory>();

        builder.Build(services);
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpRemoteLogger));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpClientFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentProcessorFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentConverterFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpRemoteService));
        Assert.Contains(services, u => u.ServiceType == typeof(IObjectContentConverterFactory));
        Assert.True(services.First(u => u.ServiceType == typeof(IObjectContentConverterFactory)).ImplementationType ==
                    typeof(CustomObjectContentConverterFactory));
        Assert.Equal(46, services.Count);
    }

    [Fact]
    public void Build_UseObjectContentConverterFactory_Duplicate_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new HttpRemoteBuilder().UseObjectContentConverterFactory<CustomObjectContentConverterFactory>();

        builder.Build(services);
        builder.Build(services);
        builder.Build(services);
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpRemoteLogger));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpClientFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentProcessorFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentConverterFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpRemoteService));
        Assert.Contains(services, u => u.ServiceType == typeof(IObjectContentConverterFactory));
        Assert.True(services.First(u => u.ServiceType == typeof(IObjectContentConverterFactory)).ImplementationType ==
                    typeof(CustomObjectContentConverterFactory));
        Assert.Equal(48, services.Count);
    }

    [Fact]
    public void BuildHttpDeclarativeServices_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new HttpRemoteBuilder().AddHttpDeclarative<IHttpTest>();

        builder.BuildHttpDeclarativeServices(services);
        builder.BuildHttpDeclarativeServices(services);

        Assert.Contains(services, u => u.ServiceType == typeof(IHttpTest));
        Assert.Single(services);
    }

    [Fact]
    public void Build_Resolve_ReturnOK()
    {
        var services = new ServiceCollection();

        services.Configure<HttpRemoteOptions>(options =>
        {
            options.DefaultContentType = "application/json";
            options.DefaultFileDownloadDirectory = @"C:\Workspaces";
        });

        var builder = new HttpRemoteBuilder()
            .AddHttpContentProcessors(() => [new CustomStringContentProcessor()])
            .AddHttpContentConverters(() => [new CustomStringContentConverter()])
            .AddHttpDeclarativeExtractors(() => [new BodyDeclarativeExtractor()])
            .AddHttpDeclarative<IHttpTest>();

        builder.Build(services);

        using var serviceProvider = services.BuildServiceProvider();
        var remoteOptions = serviceProvider.GetRequiredService<IOptions<HttpRemoteOptions>>().Value;

        Assert.Equal("application/json", remoteOptions.DefaultContentType);
        Assert.Equal(@"C:\Workspaces", remoteOptions.DefaultFileDownloadDirectory);

        var httpContentProcessorFactory =
            (HttpContentProcessorFactory)serviceProvider.GetRequiredService(typeof(IHttpContentProcessorFactory));
        Assert.NotNull(httpContentProcessorFactory._processors);
        Assert.Equal(9, httpContentProcessorFactory._processors.Count);
        Assert.Equal(
            [
                typeof(StringContentProcessor), typeof(FormUrlEncodedContentProcessor),
                typeof(ByteArrayContentProcessor), typeof(StreamContentProcessor),
                typeof(MultipartFormDataContentProcessor), typeof(ReadOnlyMemoryContentProcessor),
                typeof(JsonLinesContentProcessor), typeof(FileInfoContentProcessor),
                typeof(CustomStringContentProcessor)
            ],
            httpContentProcessorFactory._processors.Select(u => u.Key));

        var httpContentConverterFactory =
            (HttpContentConverterFactory)serviceProvider.GetRequiredService(typeof(IHttpContentConverterFactory));
        Assert.NotNull(httpContentConverterFactory._converters);
        Assert.Equal(6, httpContentConverterFactory._converters.Count);
        Assert.Equal(
            [
                typeof(HttpResponseMessageConverter), typeof(StringContentConverter), typeof(ByteArrayContentConverter),
                typeof(StreamContentConverter), typeof(VoidContentConverter), typeof(CustomStringContentConverter)
            ],
            httpContentConverterFactory._converters.Select(u => u.Key));

        var httpTest = serviceProvider.GetRequiredService<IHttpTest>();
        Assert.NotNull(httpTest);

        dynamic httpTestProxy = httpTest;
        Assert.NotNull(httpTestProxy.RemoteService);
        Assert.NotNull(httpTestProxy.InterfaceType);
        Assert.Equal(typeof(IHttpTest), httpTestProxy.InterfaceType);

        Assert.NotNull(remoteOptions.HttpDeclarativeExtractors);
        Assert.Single(remoteOptions.HttpDeclarativeExtractors);
        Assert.NotNull(remoteOptions.PipelineHandlerTypes);
        Assert.Equal(13, remoteOptions.PipelineHandlerTypes.Count);
        Assert.Equal([
            typeof(SuppressExceptionPipelineHandler),
            typeof(ResponseAssertionPipelineHandler),
            typeof(ResponseProfilerPipelineHandler),
            typeof(RequestEventPipelineHandler),
            typeof(TimeoutPipelineHandler),
            typeof(RetryPipelineHandler),
            typeof(TokenManagementPipelineHandler),
            typeof(AutoRedirectPipelineHandler),
            typeof(StatusCodePipelineHandler),
            typeof(ContentLengthValidationPipelineHandler),
            typeof(RequestBuilderPipelineHandler),
            typeof(RequestProfilerPipelineHandler),
            typeof(SendCorePipelineHandler)
        ], remoteOptions.PipelineHandlerTypes);
    }

    [Fact]
    public void RegisterContentProviders_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new HttpRemoteBuilder();

        builder.RegisterContentProviders(services);
        Assert.Empty(services);

        builder.AddHttpContentProcessors(() => [new CustomStringContentProcessor()])
            .AddHttpContentConverters(() => [new CustomStringContentConverter()])
            .AddGenericHttpContentConverters(() =>
            [
                new GenericHttpContentConverter(typeof(IAsyncEnumerable<>), typeArgs =>
                    (IHttpContentConverter)Activator.CreateInstance(
                        typeof(AsyncEnumerableContentConverter<>).MakeGenericType(typeArgs[0]))!)
            ]);

        builder.RegisterContentProviders(services);
        Assert.Equal(3, services.Count);
    }

    [Fact]
    public void EnsureSuppressExceptionHandlerFirst_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpRemoteBuilder.EnsureSuppressExceptionHandlerFirst(null!));

    [Fact]
    public void EnsureSuppressExceptionHandlerFirst_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();
        builder.AddPipelineHandler<CustomPipelineHandler>();
        Assert.Equal(typeof(CustomPipelineHandler), builder._httpRequestPipelineHandlerTypes.First());

        var pipelineHandlerTypes =
            HttpRemoteBuilder.EnsureSuppressExceptionHandlerFirst(builder._httpRequestPipelineHandlerTypes);
        Assert.NotNull(pipelineHandlerTypes);
        Assert.Equal(typeof(SuppressExceptionPipelineHandler), pipelineHandlerTypes.First());
    }

    [Fact]
    public void Build_EnsureSuppressExceptionHandlerFirst_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new HttpRemoteBuilder().AddPipelineHandler<CustomPipelineHandler>();

        builder.Build(services);

        using var serviceProvider = services.BuildServiceProvider();
        var httpRemoteOptions = serviceProvider.GetRequiredService<IOptions<HttpRemoteOptions>>();

        Assert.NotNull(httpRemoteOptions.Value.PipelineHandlerTypes);
        Assert.Equal(typeof(SuppressExceptionPipelineHandler), httpRemoteOptions.Value.PipelineHandlerTypes[0]);
        Assert.Equal(typeof(CustomPipelineHandler), httpRemoteOptions.Value.PipelineHandlerTypes.Skip(1).First());
    }
}