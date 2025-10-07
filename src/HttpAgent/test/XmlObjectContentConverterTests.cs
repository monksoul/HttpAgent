// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class XmlObjectContentConverterTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var converter = new XmlObjectContentConverter<string>();
        Assert.NotNull(converter);
        Assert.True(typeof(IHttpContentConverter<string>).IsAssignableFrom(typeof(XmlObjectContentConverter<string>)));
        Assert.True(typeof(XmlObjectContentConverter).IsAssignableFrom(typeof(XmlObjectContentConverter<string>)));
    }

    [Fact]
    public void Read_ReturnOK()
    {
        using var stringContent = new StringContent("""
                                                    <XmlModel>
                                                       <Name>Furion</Name>
                                                       <Age>30</Age>
                                                    </XmlModel>
                                                    """, Encoding.UTF8,
            new MediaTypeHeaderValue("application/xml"));
        var httpResponseMessage3 = new HttpResponseMessage();
        httpResponseMessage3.Content = stringContent;

        var converter3 = new XmlObjectContentConverter<XmlModel>();
        var xmlModel = converter3.Read(httpResponseMessage3);
        Assert.NotNull(xmlModel);
        Assert.Equal("Furion", xmlModel.Name);
        Assert.Equal(30, xmlModel.Age);
    }

    [Fact]
    public async Task ReadAsync_ReturnOK()
    {
        using var stringContent = new StringContent("""
                                                    <XmlModel>
                                                       <Name>Furion</Name>
                                                       <Age>30</Age>
                                                    </XmlModel>
                                                    """, Encoding.UTF8,
            new MediaTypeHeaderValue("application/xml"));
        var httpResponseMessage3 = new HttpResponseMessage();
        httpResponseMessage3.Content = stringContent;

        var converter3 = new XmlObjectContentConverter<XmlModel>();
        var xmlModel = await converter3.ReadAsync(httpResponseMessage3);
        Assert.NotNull(xmlModel);
        Assert.Equal("Furion", xmlModel.Name);
        Assert.Equal(30, xmlModel.Age);
    }

    [Fact]
    public async Task ReadAsync_WithCancellationToken_ReturnOK()
    {
        using var stringContent = new StringContent("""
                                                    <XmlModel>
                                                       <Name>Furion</Name>
                                                       <Age>30</Age>
                                                    </XmlModel>
                                                    """);
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        using var cancellationTokenSource = new CancellationTokenSource();

        var converter = new XmlObjectContentConverter<XmlModel>();
        var xmlModel = await converter.ReadAsync(httpResponseMessage, cancellationTokenSource.Token);
        Assert.NotNull(xmlModel);
        Assert.Equal("Furion", xmlModel.Name);
        Assert.Equal(30, xmlModel.Age);

        using var stringContent2 = new StringContent("""
                                                     <XmlModel>
                                                        <Name>Furion</Name>
                                                        <Age>30</Age>
                                                     </XmlModel>
                                                     """);
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        using var cancellationTokenSource2 = new CancellationTokenSource();

        var converter2 = new XmlObjectContentConverter<XmlModel>();
        var xmlModel2 = await converter2.ReadAsync(httpResponseMessage2, cancellationTokenSource2.Token);
        Assert.NotNull(xmlModel2);
        Assert.Equal("Furion", xmlModel2.Name);
        Assert.Equal(30, xmlModel2.Age);
    }

    [Fact]
    public void Read_WithType_ReturnOK()
    {
        using var stringContent = new StringContent("""
                                                    <XmlModel>
                                                       <Name>Furion</Name>
                                                       <Age>30</Age>
                                                    </XmlModel>
                                                    """, Encoding.UTF8,
            new MediaTypeHeaderValue("application/xml"));
        var httpResponseMessage3 = new HttpResponseMessage();
        httpResponseMessage3.Content = stringContent;

        var converter3 = new XmlObjectContentConverter();
        var xmlModel = converter3.Read(typeof(XmlModel), httpResponseMessage3) as XmlModel;
        Assert.NotNull(xmlModel);
        Assert.Equal("Furion", xmlModel.Name);
        Assert.Equal(30, xmlModel.Age);
    }

    [Fact]
    public async Task ReadAsync_WithType_ReturnOK()
    {
        using var stringContent = new StringContent("""
                                                    <XmlModel>
                                                       <Name>Furion</Name>
                                                       <Age>30</Age>
                                                    </XmlModel>
                                                    """, Encoding.UTF8,
            new MediaTypeHeaderValue("application/xml"));
        var httpResponseMessage3 = new HttpResponseMessage();
        httpResponseMessage3.Content = stringContent;

        var converter3 = new XmlObjectContentConverter();
        var xmlModel = await converter3.ReadAsync(typeof(XmlModel), httpResponseMessage3) as XmlModel;
        Assert.NotNull(xmlModel);
        Assert.Equal("Furion", xmlModel.Name);
        Assert.Equal(30, xmlModel.Age);
    }

    [Fact]
    public async Task ReadAsync_WithType_WithCancellationToken_ReturnOK()
    {
        using var stringContent = new StringContent("""
                                                    <XmlModel>
                                                       <Name>Furion</Name>
                                                       <Age>30</Age>
                                                    </XmlModel>
                                                    """);
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        using var cancellationTokenSource = new CancellationTokenSource();

        var converter = new XmlObjectContentConverter();
        var xmlModel =
            await converter.ReadAsync(typeof(XmlModel), httpResponseMessage, cancellationTokenSource.Token) as
                XmlModel;
        Assert.NotNull(xmlModel);
        Assert.Equal("Furion", xmlModel.Name);
        Assert.Equal(30, xmlModel.Age);

        using var stringContent2 = new StringContent("""
                                                     <XmlModel>
                                                        <Name>Furion</Name>
                                                        <Age>30</Age>
                                                     </XmlModel>
                                                     """);
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        using var cancellationTokenSource2 = new CancellationTokenSource();

        var converter2 = new XmlObjectContentConverter();
        var xmlModel2 =
            await converter2.ReadAsync(typeof(XmlModel), httpResponseMessage2, cancellationTokenSource2.Token) as
                XmlModel;
        Assert.NotNull(xmlModel2);
        Assert.Equal("Furion", xmlModel2.Name);
        Assert.Equal(30, xmlModel2.Age);
    }

    [Fact]
    public void DeserializeXml_ReturnOK()
    {
        var converter = new XmlObjectContentConverter<XmlModel>();
        var deserializeXmlMethod = typeof(XmlObjectContentConverter)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(m => m is { Name: "DeserializeXml", IsGenericMethod: false })!;

        const string xmlString = """
                                 <XmlModel>
                                    <Name>Furion</Name>
                                    <Age>30</Age>
                                 </XmlModel>
                                 """;

        var xmlModel = deserializeXmlMethod.Invoke(converter, [typeof(XmlModel), xmlString]) as XmlModel;
        Assert.NotNull(xmlModel);
        Assert.Equal("Furion", xmlModel.Name);
        Assert.Equal(30, xmlModel.Age);
    }
}