// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteServiceCollectionExtensionsTests
{
    [Fact]
    public void AddHttpRemote_Invalid_Parameters()
    {
        var services = new ServiceCollection();

        Assert.Throws<ArgumentNullException>(() =>
        {
            services.AddHttpRemote((HttpRemoteBuilder)null!);
        });
    }

    [Fact]
    public void AddHttpRemote_ReturnOK()
    {
        var services = new ServiceCollection();

        var httpRemoteBuilder = new HttpRemoteBuilder();
        services.AddHttpRemote(httpRemoteBuilder);

        Assert.NotEmpty(services);
        _ = services.BuildServiceProvider();
    }

    [Fact]
    public void AddHttpRemote_Action_Empty_Parameters()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote();

        Assert.NotEmpty(services);
        _ = services.BuildServiceProvider();
    }

    [Fact]
    public void AddHttpRemote_Action_ReturnOK()
    {
        var services = new ServiceCollection();
        services.Configure<HttpRemoteOptions>(options =>
        {
            options.DefaultContentType = "application/json";
        });

        services.AddHttpRemote();

        Assert.NotEmpty(services);
        _ = services.BuildServiceProvider();
    }

    [Fact]
    public void AddHttpRemote_Duplicate_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote(s => s.AddHttpDeclarative<IHttpTest>());
        services.AddHttpRemote(s => s.AddHttpDeclarative<IHttpTest>());
        services.AddHttpRemote(s => s.AddHttpDeclarative<IHttpTest>());

        Assert.Contains(services, u => u.ServiceType == typeof(IHttpClientFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentProcessorFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentConverterFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpRemoteService));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpTest));
        Assert.Equal(35, services.Count);
    }

    [Fact]
    public void AddHttpRemote_WithConfigureOptions_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote().ConfigureOptions(options =>
        {
            options.DefaultContentType = "application/json";
        });
    }
}