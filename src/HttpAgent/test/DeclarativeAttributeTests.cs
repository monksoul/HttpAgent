// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class DeclarativeAttributeTests
{
    [Fact]
    public void BodyAttribute_ReturnOK()
    {
        var attributeUsage = typeof(BodyAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Parameter, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new BodyAttribute();
        Assert.Null(attribute.ContentType);
        Assert.Null(attribute.ContentEncoding);
        Assert.False(attribute.UseStringContent);
        Assert.False(attribute.RawString);

        var attribute2 = new BodyAttribute("application/json");
        Assert.Equal("application/json", attribute2.ContentType);
        Assert.Null(attribute2.ContentEncoding);

        var attribute3 = new BodyAttribute("application/json", "utf-32");
        Assert.Equal("application/json", attribute3.ContentType);
        Assert.Equal("utf-32", attribute3.ContentEncoding);
    }

    [Fact]
    public void MultipartFormAttribute_ReturnOK()
    {
        var attributeUsage = typeof(MultipartFormAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new MultipartFormAttribute();
        Assert.NotNull(attribute.Boundary);
        Assert.StartsWith("--------------------------", attribute.Boundary);
        Assert.True(attribute.OmitContentType);

        var attribute2 = new MultipartFormAttribute("--------------------");
        Assert.Equal("--------------------", attribute2.Boundary);
    }

    [Fact]
    public void MultipartAttribute_ReturnOK()
    {
        var attributeUsage = typeof(MultipartAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Parameter, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new MultipartAttribute();
        Assert.Null(attribute.Name);
        Assert.Null(attribute.FileName);
        Assert.Null(attribute.ContentType);
        Assert.Null(attribute.ContentEncoding);
        Assert.Equal(FileSourceType.None, attribute.AsFileFrom);
        Assert.True(attribute.AsFormItem);

        var attribute2 = new MultipartAttribute("file");
        Assert.Equal("file", attribute2.Name);
    }

    [Fact]
    public void MultipartObjectAttribute_ReturnOK()
    {
        var attributeUsage = typeof(MultipartObjectAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Parameter, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new MultipartObjectAttribute();
        Assert.Null(attribute.Name);
        Assert.Null(attribute.FileName);
        Assert.Null(attribute.ContentType);
        Assert.Null(attribute.ContentEncoding);
        Assert.Equal(FileSourceType.None, attribute.AsFileFrom);
        Assert.False(attribute.AsFormItem);
    }

    [Fact]
    public void CookieAttribute_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new CookieAttribute(null!));
        Assert.Throws<ArgumentException>(() => new CookieAttribute(string.Empty));
        Assert.Throws<ArgumentException>(() => new CookieAttribute(" "));

        Assert.Throws<ArgumentNullException>(() => new CookieAttribute(null!, null));
        Assert.Throws<ArgumentException>(() => new CookieAttribute(string.Empty, null));
        Assert.Throws<ArgumentException>(() => new CookieAttribute(" ", null));
    }

    [Fact]
    public void CookieAttribute_ReturnOK()
    {
        var attributeUsage = typeof(CookieAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Parameter,
            attributeUsage.ValidOn);
        Assert.True(attributeUsage.AllowMultiple);

        var attribute = new CookieAttribute();
        Assert.Null(attribute.Name);
        Assert.Null(attribute.Value);
        Assert.Null(attribute.AliasAs);
        Assert.False(attribute.HasSetValue);

        var attribute2 = new CookieAttribute("name");
        Assert.Equal("name", attribute2.Name);
        Assert.Null(attribute2.Value);
        Assert.Null(attribute2.AliasAs);
        Assert.False(attribute2.HasSetValue);

        var attribute3 = new CookieAttribute("name", null);
        Assert.Equal("name", attribute3.Name);
        Assert.Null(attribute3.Value);
        Assert.Null(attribute3.AliasAs);
        Assert.True(attribute3.HasSetValue);
    }

    [Fact]
    public void DisableCacheAttribute_ReturnOK()
    {
        var attributeUsage = typeof(DisableCacheAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new DisableCacheAttribute();
        Assert.True(attribute.Disabled);

        var attribute2 = new DisableCacheAttribute(false);
        Assert.False(attribute2.Disabled);
    }

    [Fact]
    public void EnsureSuccessStatusCodeAttribute_ReturnOK()
    {
        var attributeUsage = typeof(EnsureSuccessStatusCodeAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new EnsureSuccessStatusCodeAttribute();
        Assert.True(attribute.Enabled);

        var attribute2 = new EnsureSuccessStatusCodeAttribute(false);
        Assert.False(attribute2.Enabled);
    }

    [Fact]
    public void PathAttribute_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new PathAttribute(null!, null));
        Assert.Throws<ArgumentException>(() => new PathAttribute(string.Empty, null));
        Assert.Throws<ArgumentException>(() => new PathAttribute(" ", null));
    }

    [Fact]
    public void PathAttribute_ReturnOK()
    {
        var attributeUsage = typeof(PathAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.True(attributeUsage.AllowMultiple);

        var attribute = new PathAttribute("name", "furion");
        Assert.Equal("name", attribute.Name);
        Assert.Equal("furion", attribute.Value);
    }

    [Fact]
    public void HeaderAttribute_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new HeaderAttribute(null!));
        Assert.Throws<ArgumentException>(() => new HeaderAttribute(string.Empty));
        Assert.Throws<ArgumentException>(() => new HeaderAttribute(" "));

        Assert.Throws<ArgumentNullException>(() => new HeaderAttribute(null!, null));
        Assert.Throws<ArgumentException>(() => new HeaderAttribute(string.Empty, null));
        Assert.Throws<ArgumentException>(() => new HeaderAttribute(" ", null));
    }

    [Fact]
    public void HeaderAttribute_ReturnOK()
    {
        var attributeUsage = typeof(HeaderAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Parameter,
            attributeUsage.ValidOn);
        Assert.True(attributeUsage.AllowMultiple);

        var attribute = new HeaderAttribute();
        Assert.Null(attribute.Name);
        Assert.Null(attribute.Value);
        Assert.Null(attribute.AliasAs);
        Assert.False(attribute.HasSetValue);
        Assert.False(attribute.Replace);

        var attribute2 = new HeaderAttribute("Set-Cookie");
        Assert.Equal("Set-Cookie", attribute2.Name);
        Assert.Null(attribute2.Value);
        Assert.Null(attribute2.AliasAs);
        Assert.False(attribute2.HasSetValue);

        var attribute3 = new HeaderAttribute("Set-Cookie", null);
        Assert.Equal("Set-Cookie", attribute3.Name);
        Assert.Null(attribute3.Value);
        Assert.Null(attribute3.AliasAs);
        Assert.True(attribute3.HasSetValue);
        Assert.False(attribute3.Escape);
    }

    [Fact]
    public void HttpClientNameAttribute_ReturnOK()
    {
        var attributeUsage = typeof(HttpClientNameAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new HttpClientNameAttribute(null);
        Assert.Null(attribute.Name);

        var attribute2 = new HttpClientNameAttribute("client-name");
        Assert.Equal("client-name", attribute2.Name);
    }

    [Fact]
    public void ProfilerAttribute_ReturnOK()
    {
        var attributeUsage = typeof(ProfilerAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new ProfilerAttribute();
        Assert.True(attribute.Enabled);

        var attribute2 = new ProfilerAttribute(false);
        Assert.False(attribute2.Enabled);
    }

    [Fact]
    public void PerformanceOptimizationAttribute_ReturnOK()
    {
        var attributeUsage = typeof(PerformanceOptimizationAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new PerformanceOptimizationAttribute();
        Assert.True(attribute.Enabled);

        var attribute2 = new PerformanceOptimizationAttribute(false);
        Assert.False(attribute2.Enabled);
    }

    [Fact]
    public void AutoSetHostHeaderAttribute_ReturnOK()
    {
        var attributeUsage = typeof(AutoSetHostHeaderAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new AutoSetHostHeaderAttribute();
        Assert.True(attribute.Enabled);

        var attribute2 = new AutoSetHostHeaderAttribute(false);
        Assert.False(attribute2.Enabled);
    }

    [Fact]
    public void QueryAttribute_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new QueryAttribute(null!));
        Assert.Throws<ArgumentException>(() => new QueryAttribute(string.Empty));
        Assert.Throws<ArgumentException>(() => new QueryAttribute(" "));

        Assert.Throws<ArgumentNullException>(() => new QueryAttribute(null!, null));
        Assert.Throws<ArgumentException>(() => new QueryAttribute(string.Empty, null));
        Assert.Throws<ArgumentException>(() => new QueryAttribute(" ", null));
    }

    [Fact]
    public void QueryAttribute_ReturnOK()
    {
        var attributeUsage = typeof(QueryAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Parameter,
            attributeUsage.ValidOn);
        Assert.True(attributeUsage.AllowMultiple);

        var attribute = new QueryAttribute();
        Assert.Null(attribute.Name);
        Assert.Null(attribute.Value);
        Assert.Null(attribute.AliasAs);
        Assert.False(attribute.HasSetValue);
        Assert.Null(attribute.Prefix);
        Assert.False(attribute.Replace);
        Assert.False(attribute.IgnoreNullValues);

        var attribute2 = new QueryAttribute("name");
        Assert.Equal("name", attribute2.Name);
        Assert.Null(attribute2.Value);
        Assert.Null(attribute2.AliasAs);
        Assert.False(attribute2.HasSetValue);

        var attribute3 = new QueryAttribute("name", null);
        Assert.Equal("name", attribute3.Name);
        Assert.Null(attribute3.Value);
        Assert.Null(attribute3.AliasAs);
        Assert.True(attribute3.HasSetValue);
    }

    [Fact]
    public void PathSegmentAttribute_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new PathSegmentAttribute(null!));
        Assert.Throws<ArgumentException>(() => new PathSegmentAttribute(string.Empty));
        Assert.Throws<ArgumentException>(() => new PathSegmentAttribute(" "));
    }

    [Fact]
    public void PathSegmentAttribute_ReturnOK()
    {
        var attributeUsage = typeof(PathSegmentAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Parameter,
            attributeUsage.ValidOn);
        Assert.True(attributeUsage.AllowMultiple);

        var attribute = new PathSegmentAttribute();
        Assert.Null(attribute.Segment);
        Assert.False(attribute.Remove);

        var attribute2 = new PathSegmentAttribute("docs");
        Assert.Equal("docs", attribute2.Segment);
        Assert.False(attribute.Remove);
    }

    [Fact]
    public void SimulateBrowserAttribute_ReturnOK()
    {
        var attributeUsage = typeof(SimulateBrowserAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new SimulateBrowserAttribute();
        Assert.False(attribute.Mobile);

        var attribute2 = new SimulateBrowserAttribute { Mobile = true };
        Assert.True(attribute2.Mobile);
    }

    [Fact]
    public void TimeoutAttribute_ReturnOK()
    {
        var attributeUsage = typeof(TimeoutAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new TimeoutAttribute(200);
        Assert.Equal(200, attribute.Timeout);
    }

    [Fact]
    public void TraceIdentifierAttribute_ReturnOK()
    {
        var attributeUsage = typeof(TraceIdentifierAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new TraceIdentifierAttribute("furion");
        Assert.Equal("furion", attribute.Identifier);
    }

    [Fact]
    public void AcceptLanguageAttribute_ReturnOK()
    {
        var attributeUsage = typeof(AcceptLanguageAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new AcceptLanguageAttribute("fr-FR");
        Assert.Equal("fr-FR", attribute.Language);

        var attribute2 = new AcceptLanguageAttribute(null);
        Assert.Null(attribute2.Language);
    }

    [Fact]
    public void BaseAddressAttribute_ReturnOK()
    {
        var attributeUsage = typeof(BaseAddressAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new BaseAddressAttribute(null);
        Assert.Null(attribute.BaseAddress);

        var attribute2 = new BaseAddressAttribute("https://localhost");
        Assert.Equal("https://localhost", attribute2.BaseAddress);
    }

    [Fact]
    public void RefererAttribute_ReturnOK()
    {
        var attributeUsage = typeof(RefererAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new RefererAttribute(null);
        Assert.Null(attribute.Referer);

        var attribute2 = new RefererAttribute("https://localhost");
        Assert.Equal("https://localhost", attribute2.Referer);
    }

    [Fact]
    public void HttpVersionAttribute_ReturnOK()
    {
        var attributeUsage = typeof(HttpVersionAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new HttpVersionAttribute(null);
        Assert.Null(attribute.Version);

        var attribute2 = new HttpVersionAttribute("1.2");
        Assert.Equal("1.2", attribute2.Version);
    }

    [Fact]
    public void SuppressExceptionsAttribute_ReturnOK()
    {
        var attributeUsage = typeof(SuppressExceptionsAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new SuppressExceptionsAttribute();
        Assert.Single(attribute.Types);
        Assert.Equal(typeof(Exception), attribute.Types[0]);

        var attribute2 = new SuppressExceptionsAttribute(true);
        Assert.Single(attribute2.Types);
        Assert.Equal(typeof(Exception), attribute2.Types[0]);

        var attribute3 = new SuppressExceptionsAttribute(false);
        Assert.Empty(attribute3.Types);

        var attribute4 = new SuppressExceptionsAttribute(typeof(Exception), typeof(TimeoutAttribute));
        Assert.Equal(2, attribute4.Types.Length);
        Assert.Equal([typeof(Exception), typeof(TimeoutAttribute)], attribute4.Types);
    }
}