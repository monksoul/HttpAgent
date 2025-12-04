// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpContentProcessorBaseTests
{
    [Fact]
    public void TryProcess_ReturnOK()
    {
        var processor = new StringContentProcessor();
        Assert.True(processor.TryProcess(null, "application/json", null, out var result1));
        Assert.Null(result1);

        Assert.True(processor.TryProcess(new StringContent("Furion"), "application/json", null, out var result2));
        Assert.NotNull(result2);

        Assert.False(processor.TryProcess(new { }, "application/json", null, out var result3));
        Assert.Null(result3);
    }

    [Fact]
    public void GetService_ReturnOK()
    {
        var processor = new StringContentProcessor();
        Assert.Null(processor.GetService<IMyService>());

        using var serviceProvider =
            new ServiceCollection().AddTransient<IMyService, MyService>().BuildServiceProvider();
        processor.ServiceProvider = serviceProvider;
        Assert.NotNull(processor.GetService<IMyService>());
    }

    public class MyService : IMyService;

    public interface IMyService;
}