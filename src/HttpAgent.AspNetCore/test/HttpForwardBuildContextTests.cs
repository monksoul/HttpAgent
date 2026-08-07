// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.AspNetCore.Tests;

public class HttpForwardBuildContextTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var buildContext = new HttpForwardBuildContext(new DefaultHttpContext(), HttpRequestBuilder.Get(""),
            new HttpContextForwardOptions());
        Assert.NotNull(buildContext.HttpContext);
        Assert.NotNull(buildContext.Builder);
        Assert.NotNull(buildContext.ForwardOptions);
    }
}