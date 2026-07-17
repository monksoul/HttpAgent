// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteResultContentConverterTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var converter = new HttpRemoteResultContentConverter<ObjectModel>();
        Assert.NotNull(converter);
        Assert.True(
            typeof(IHttpContentConverter<HttpRemoteResult<ObjectModel>>).IsAssignableFrom(
                typeof(HttpRemoteResultContentConverter<ObjectModel>)));
        Assert.True(converter.KeepsResponseAlive);
    }

    [Fact]
    public void Read_ReturnOK()
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

        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new HttpRemoteResultContentConverter<ObjectModel>();
        var httpRemoteResult = converter.Read(typeof(HttpRemoteResult<ObjectModel>),
                new HttpContentConverterContext(httpResponseMessage) { Factory = httpContentConverterFactory }) as
            HttpRemoteResult<ObjectModel>;
        Assert.NotNull(httpRemoteResult);
        Assert.Equal(10, httpRemoteResult.Result?.Id);
        Assert.Equal("furion", httpRemoteResult.Result?.Name);

        using var stringContent2 = new StringContent("""{"Id":10, "Name":"furion"}""");
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        var converter2 = new HttpRemoteResultContentConverter<ObjectModel>();
        var httpRemoteResult2 = converter2.Read(typeof(HttpRemoteResult<ObjectModel>),
                new HttpContentConverterContext(httpResponseMessage2) { Factory = httpContentConverterFactory }) as
            HttpRemoteResult<ObjectModel>;
        Assert.NotNull(httpRemoteResult2);
        Assert.Equal(10, httpRemoteResult2.Result?.Id);
        Assert.Equal("furion", httpRemoteResult2.Result?.Name);
    }

    [Fact]
    public async Task ReadAsync_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        services.TryAddSingleton<IObjectContentConverterFactory, ObjectContentConverterFactory>();

        await using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpContentConverterFactory = new HttpContentConverterFactory(serviceProvider, logger, null!, null!);

        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new HttpRemoteResultContentConverter<ObjectModel>();
        var httpRemoteResult = await converter.ReadAsync(typeof(HttpRemoteResult<ObjectModel>),
                new HttpContentConverterContext(httpResponseMessage) { Factory = httpContentConverterFactory }) as
            HttpRemoteResult<ObjectModel>;
        Assert.NotNull(httpRemoteResult);
        Assert.Equal(10, httpRemoteResult.Result?.Id);
        Assert.Equal("furion", httpRemoteResult.Result?.Name);

        using var stringContent2 = new StringContent("""{"Id":10, "Name":"furion"}""");
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        var converter2 = new HttpRemoteResultContentConverter<ObjectModel>();
        var httpRemoteResult2 = await converter2.ReadAsync(typeof(HttpRemoteResult<ObjectModel>),
                new HttpContentConverterContext(httpResponseMessage2) { Factory = httpContentConverterFactory }) as
            HttpRemoteResult<ObjectModel>;
        Assert.NotNull(httpRemoteResult2);
        Assert.Equal(10, httpRemoteResult2.Result?.Id);
        Assert.Equal("furion", httpRemoteResult2.Result?.Name);
    }

    [Fact]
    public void Read_WithType_ReturnOK()
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

        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new HttpRemoteResultContentConverter<ObjectModel>();
        var httpRemoteResult =
            converter.Read(
                new HttpContentConverterContext(httpResponseMessage) { Factory = httpContentConverterFactory });
        Assert.NotNull(httpRemoteResult);
        Assert.Equal(10, httpRemoteResult.Result?.Id);
        Assert.Equal("furion", httpRemoteResult.Result?.Name);

        using var stringContent2 = new StringContent("""{"Id":10, "Name":"furion"}""");
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        var converter2 = new HttpRemoteResultContentConverter<ObjectModel>();
        var httpRemoteResult2 =
            converter2.Read(
                new HttpContentConverterContext(httpResponseMessage2) { Factory = httpContentConverterFactory });
        Assert.NotNull(httpRemoteResult2);
        Assert.Equal(10, httpRemoteResult2.Result?.Id);
        Assert.Equal("furion", httpRemoteResult2.Result?.Name);
    }

    [Fact]
    public async Task ReadAsync_WithType_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        services.TryAddSingleton<IObjectContentConverterFactory, ObjectContentConverterFactory>();

        await using var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<IHttpRemoteLogger>();
        var httpContentConverterFactory = new HttpContentConverterFactory(serviceProvider, logger, null!, null!);

        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new HttpRemoteResultContentConverter<ObjectModel>();
        var httpRemoteResult =
            await converter.ReadAsync(
                new HttpContentConverterContext(httpResponseMessage) { Factory = httpContentConverterFactory });
        Assert.NotNull(httpRemoteResult);
        Assert.Equal(10, httpRemoteResult.Result?.Id);
        Assert.Equal("furion", httpRemoteResult.Result?.Name);

        using var stringContent2 = new StringContent("""{"Id":10, "Name":"furion"}""");
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        var converter2 = new HttpRemoteResultContentConverter<ObjectModel>();
        var httpRemoteResult2 = await converter2.ReadAsync(new HttpContentConverterContext(httpResponseMessage2)
        {
            Factory = httpContentConverterFactory
        });
        Assert.NotNull(httpRemoteResult2);
        Assert.Equal(10, httpRemoteResult2.Result?.Id);
        Assert.Equal("furion", httpRemoteResult2.Result?.Name);
    }

    public class ObjectModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}