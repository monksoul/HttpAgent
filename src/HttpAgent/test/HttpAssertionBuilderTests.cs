// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpAssertionBuilderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.NotNull(httpAssertionBuilder);
        Assert.NotNull(httpAssertionBuilder._assertions);
        Assert.Empty(httpAssertionBuilder._assertions);
    }

    [Fact]
    public void AddAssertion_Invalid_Parameters()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Throws<ArgumentNullException>(() => httpAssertionBuilder.AddAssertion(null!));
    }

    [Fact]
    public void AddAssertion_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();

        httpAssertionBuilder.AddAssertion(_ => Task.CompletedTask);
        Assert.Single(httpAssertionBuilder._assertions);

        httpAssertionBuilder.AddAssertion(_ => Task.CompletedTask);
        Assert.Equal(2, httpAssertionBuilder._assertions.Count);
    }

    [Fact]
    public void GetAssertions_ReturnOK()
    {
        var httpAssertionBuilder = new HttpAssertionBuilder();
        Assert.Empty(httpAssertionBuilder.GetAssertions());

        httpAssertionBuilder.AddAssertion(_ => Task.CompletedTask);
        Assert.Single(httpAssertionBuilder.GetAssertions());

        httpAssertionBuilder.AddAssertion(_ => Task.CompletedTask);
        Assert.Equal(2, httpAssertionBuilder.GetAssertions().Count);
    }
}