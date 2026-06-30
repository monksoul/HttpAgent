// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpMethodAttributeTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new HttpMethodAttribute((string)null!));
        Assert.Throws<ArgumentException>(() => new HttpMethodAttribute(string.Empty));
        Assert.Throws<ArgumentException>(() => new HttpMethodAttribute(" "));

        Assert.Throws<ArgumentNullException>(() => new HttpMethodAttribute((HttpMethod)null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var attributeUsage = typeof(HttpMethodAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new HttpMethodAttribute("GET");
        Assert.Equal(HttpMethod.Get, attribute.HttpMethod);
        Assert.Null(attribute.RequestUri);

        var attribute2 = new HttpMethodAttribute("DELETE", "http://localhost");
        Assert.Equal(HttpMethod.Delete, attribute2.HttpMethod);
        Assert.Equal("http://localhost", attribute2.RequestUri);

        var attribute3 = new HttpMethodAttribute("UNKNOWN", "http://localhost");
        Assert.Equal("UNKNOWN", attribute3.HttpMethod.ToString());
        Assert.Equal("http://localhost", attribute3.RequestUri);
    }

    [Fact]
    public void DeleteAttribute_ReturnOK()
    {
        var attributeUsage = typeof(DeleteAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new DeleteAttribute();
        Assert.Equal(HttpMethod.Delete, attribute.HttpMethod);
        Assert.Null(attribute.RequestUri);

        var attribute2 = new DeleteAttribute("http://localhost");
        Assert.Equal(HttpMethod.Delete, attribute2.HttpMethod);
        Assert.Equal("http://localhost", attribute2.RequestUri);
    }

    [Fact]
    public void GetAttribute_ReturnOK()
    {
        var attributeUsage = typeof(GetAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new GetAttribute();
        Assert.Equal(HttpMethod.Get, attribute.HttpMethod);
        Assert.Null(attribute.RequestUri);

        var attribute2 = new GetAttribute("http://localhost");
        Assert.Equal(HttpMethod.Get, attribute2.HttpMethod);
        Assert.Equal("http://localhost", attribute2.RequestUri);
    }

    [Fact]
    public void HeadAttribute_ReturnOK()
    {
        var attributeUsage = typeof(HeadAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new HeadAttribute();
        Assert.Equal(HttpMethod.Head, attribute.HttpMethod);
        Assert.Null(attribute.RequestUri);

        var attribute2 = new HeadAttribute("http://localhost");
        Assert.Equal(HttpMethod.Head, attribute2.HttpMethod);
        Assert.Equal("http://localhost", attribute2.RequestUri);
    }

    [Fact]
    public void OptionsAttribute_ReturnOK()
    {
        var attributeUsage = typeof(OptionsAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new OptionsAttribute();
        Assert.Equal(HttpMethod.Options, attribute.HttpMethod);
        Assert.Null(attribute.RequestUri);

        var attribute2 = new OptionsAttribute("http://localhost");
        Assert.Equal(HttpMethod.Options, attribute2.HttpMethod);
        Assert.Equal("http://localhost", attribute2.RequestUri);
    }

    [Fact]
    public void PatchAttribute_ReturnOK()
    {
        var attributeUsage = typeof(PatchAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new PatchAttribute();
        Assert.Equal(HttpMethod.Patch, attribute.HttpMethod);
        Assert.Null(attribute.RequestUri);

        var attribute2 = new PatchAttribute("http://localhost");
        Assert.Equal(HttpMethod.Patch, attribute2.HttpMethod);
        Assert.Equal("http://localhost", attribute2.RequestUri);
    }

    [Fact]
    public void PostAttribute_ReturnOK()
    {
        var attributeUsage = typeof(PostAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new PostAttribute();
        Assert.Equal(HttpMethod.Post, attribute.HttpMethod);
        Assert.Null(attribute.RequestUri);

        var attribute2 = new PostAttribute("http://localhost");
        Assert.Equal(HttpMethod.Post, attribute2.HttpMethod);
        Assert.Equal("http://localhost", attribute2.RequestUri);
    }

    [Fact]
    public void PutAttribute_ReturnOK()
    {
        var attributeUsage = typeof(PutAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new PutAttribute();
        Assert.Equal(HttpMethod.Put, attribute.HttpMethod);
        Assert.Null(attribute.RequestUri);

        var attribute2 = new PutAttribute("http://localhost");
        Assert.Equal(HttpMethod.Put, attribute2.HttpMethod);
        Assert.Equal("http://localhost", attribute2.RequestUri);
    }

    [Fact]
    public void QueryAttribute_ReturnOK()
    {
        var attributeUsage = typeof(QueryAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new QueryAttribute();
        Assert.Equal("QUERY", attribute.HttpMethod.ToString());
        Assert.Null(attribute.RequestUri);

        var attribute2 = new QueryAttribute("http://localhost");
        Assert.Equal("QUERY", attribute2.HttpMethod.ToString());
        Assert.Equal("http://localhost", attribute2.RequestUri);
    }

    [Fact]
    public void TraceAttribute_ReturnOK()
    {
        var attributeUsage = typeof(TraceAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new TraceAttribute();
        Assert.Equal(HttpMethod.Trace, attribute.HttpMethod);
        Assert.Null(attribute.RequestUri);

        var attribute2 = new TraceAttribute("http://localhost");
        Assert.Equal(HttpMethod.Trace, attribute2.HttpMethod);
        Assert.Equal("http://localhost", attribute2.RequestUri);
    }
}