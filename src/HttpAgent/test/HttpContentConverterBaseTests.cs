// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpContentConverterBaseTests
{
    [Fact]
    public void GetService_ReturnOK()
    {
        var converter = new StringContentConverter();
        Assert.Null(converter.GetService<IMyService>());

        using var serviceProvider =
            new ServiceCollection().AddTransient<IMyService, MyService>().BuildServiceProvider();
        converter.ServiceProvider = serviceProvider;
        Assert.NotNull(converter.GetService<IMyService>());
    }

    public class MyService : IMyService;

    public interface IMyService;
}