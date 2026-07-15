// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.AspNetCore.Tests;

public class ForwardAttributeTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var attribute = new ForwardAttribute(null);
        Assert.Null(attribute.RequestUri);
        Assert.Null(attribute.Method);
        Assert.Null(attribute.HttpClientName);
        Assert.Equal(HttpCompletionOption.ResponseContentRead, attribute.CompletionOption);
        Assert.True(attribute.WithQueryParameters);
        Assert.True(attribute.WithRequestHeaders);
        Assert.True(attribute.WithResponseStatusCode);
        Assert.True(attribute.WithResponseHeaders);
        Assert.True(attribute.WithResponseContentHeaders);
        Assert.False(attribute.ResetHostRequestHeader);
        Assert.Null(attribute.IgnoreQueryParameters);
        Assert.Null(attribute.IgnoreRequestHeaders);
        Assert.Null(attribute.IgnoreResponseHeaders);
        Assert.Null(attribute.AllowedHosts);

        var attribute2 = new ForwardAttribute("https://furion.net");
        Assert.Equal("https://furion.net", attribute2.RequestUri);

        var attribute3 = new ForwardAttribute("https://furion.net/", HttpMethod.Get);
        Assert.Equal("https://furion.net/", attribute3.RequestUri);
        Assert.Equal(HttpMethod.Get, attribute3.Method);

        var attribute4 = new ForwardAttribute("https://furion.net/", HttpMethod.Get) { HttpClientName = "furion" };
        Assert.Equal("https://furion.net/", attribute4.RequestUri);
        Assert.Equal(HttpMethod.Get, attribute4.Method);
        Assert.Equal("furion", attribute4.HttpClientName);

        Assert.True(typeof(ActionFilterAttribute).IsAssignableFrom(typeof(ForwardAttribute)));

        var attributeUsage = typeof(ForwardAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method, attributeUsage.ValidOn);
    }

    [Theory]
    [InlineData(typeof(void), typeof(VoidContent))]
    [InlineData(typeof(string), typeof(string))]
    [InlineData(typeof(Task), typeof(VoidContent))]
    [InlineData(typeof(Task<string>), typeof(string))]
    [InlineData(typeof(Task<IActionResult>), typeof(IActionResult))]
    [InlineData(typeof(Task<VoidContent>), typeof(VoidContent))]
    [InlineData(typeof(VoidContent), typeof(VoidContent))]
    public void ParseResultType_ReturnOK(Type returnType, Type resultType) =>
        Assert.Equal(resultType, ForwardAttribute.ParseResultType(returnType));
}