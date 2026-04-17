// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpDeclarativeMethodMetadataTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new HttpDeclarativeMethodMetadata(null!, null!));
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method1));
        Assert.Throws<ArgumentNullException>(() => new HttpDeclarativeMethodMetadata(method!, null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method1))!;
        var methodMetadata = new HttpDeclarativeMethodMetadata(method, method.DeclaringType!);
        Assert.NotNull(methodMetadata.Method);
        Assert.NotNull(methodMetadata.InterfaceType);
        Assert.Equal(typeof(IHttpDeclarativeTest), methodMetadata.InterfaceType);
        Assert.NotNull(methodMetadata.InterfaceAttributes);
        Assert.Single(methodMetadata.InterfaceAttributes);
        Assert.NotNull(methodMetadata.MethodAttributes);
        Assert.Single(methodMetadata.MethodAttributes);

        var method2 = typeof(IBaseService).GetMethod(nameof(IBaseService.Method1));
        var methodMetadata2 = new HttpDeclarativeMethodMetadata(method2!, typeof(IChildService));
        Assert.Single(methodMetadata2.InterfaceAttributes?.OfType<HttpClientNameAttribute>() ?? []);

        var methodMetadata3 = new HttpDeclarativeMethodMetadata(method2!, typeof(IChildService2));
        Assert.Equal(2, (methodMetadata3.InterfaceAttributes?.OfType<HttpClientNameAttribute>() ?? []).Count());
        Assert.Equal("Child",
            methodMetadata3.InterfaceAttributes?.OfType<HttpClientNameAttribute>().FirstOrDefault()?.Name);
    }

    [HttpClientName("Base")]
    public interface IBaseService
    {
        void Method1();
    }

    public interface IChildService : IBaseService;

    [HttpClientName("Child")]
    public interface IChildService2 : IBaseService;
}