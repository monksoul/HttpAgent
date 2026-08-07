// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.AspNetCore.Tests;

public class HttpRemoteBuilderExtensionsTest
{
    [Fact]
    public void ConfigureForwardOptions_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        var builder = new DefaultHttpRemoteBuilder(services);

        Assert.Throws<ArgumentNullException>(() =>
            builder.ConfigureForwardOptions((Action<HttpContextForwardOptions>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            builder.ConfigureForwardOptions((Action<HttpContextForwardOptions, IServiceProvider>)null!));
    }

    [Fact]
    public void ConfigureForwardOptions_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new DefaultHttpRemoteBuilder(services);

        builder.ConfigureForwardOptions(options =>
        {
            options.WithResponseHeaders = false;
        });
        builder.ConfigureForwardOptions((options, serviceProvider) =>
        {
            Assert.NotNull(serviceProvider);
            options.WithResponseHeaders = false;
        });
    }
}