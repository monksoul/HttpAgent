// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ObjectContentConverterTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var converter = new ObjectContentConverter<string>();
        Assert.NotNull(converter);
        Assert.True(typeof(IHttpContentConverter<string>).IsAssignableFrom(typeof(ObjectContentConverter<string>)));
        Assert.True(typeof(ObjectContentConverter).IsAssignableFrom(typeof(ObjectContentConverter<string>)));
    }

    [Fact]
    public void Read_ReturnOK()
    {
        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new ObjectContentConverter<ObjectModel>();
        var objectModel = converter.Read(httpResponseMessage);
        Assert.NotNull(objectModel);
        Assert.Equal(10, objectModel.Id);
        Assert.Equal("furion", objectModel.Name);

        using var stringContent2 = new StringContent("""{"Id":10, "Name":"furion"}""");
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        var converter2 = new ObjectContentConverter<ObjectModel>();
        var objectModel2 = converter2.Read(httpResponseMessage2);
        Assert.NotNull(objectModel2);
        Assert.Equal(10, objectModel2.Id);
        Assert.Equal("furion", objectModel2.Name);

        using var stringContent3 = new StringContent("""
                                                     <XmlModel>
                                                        <Name>Furion</Name>
                                                        <Age>30</Age>
                                                     </XmlModel>
                                                     """, Encoding.UTF8,
            new MediaTypeHeaderValue("application/xml"));
        var httpResponseMessage3 = new HttpResponseMessage();
        httpResponseMessage3.Content = stringContent3;

        var converter3 = new ObjectContentConverter<XmlModel>();
        var xmlModel = converter3.Read(httpResponseMessage3);
        Assert.NotNull(xmlModel);
        Assert.Equal("Furion", xmlModel.Name);
        Assert.Equal(30, xmlModel.Age);
    }

    [Fact]
    public async Task ReadAsync_ReturnOK()
    {
        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new ObjectContentConverter<ObjectModel>();
        var objectModel = await converter.ReadAsync(httpResponseMessage);
        Assert.NotNull(objectModel);
        Assert.Equal(10, objectModel.Id);
        Assert.Equal("furion", objectModel.Name);

        using var stringContent2 = new StringContent("""{"Id":10, "Name":"furion"}""");
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        var converter2 = new ObjectContentConverter<ObjectModel>();
        var objectModel2 = await converter2.ReadAsync(httpResponseMessage2);
        Assert.NotNull(objectModel2);
        Assert.Equal(10, objectModel2.Id);
        Assert.Equal("furion", objectModel2.Name);


        using var stringContent3 = new StringContent("""
                                                     <XmlModel>
                                                        <Name>Furion</Name>
                                                        <Age>30</Age>
                                                     </XmlModel>
                                                     """, Encoding.UTF8,
            new MediaTypeHeaderValue("application/xml"));
        var httpResponseMessage3 = new HttpResponseMessage();
        httpResponseMessage3.Content = stringContent3;

        var converter3 = new ObjectContentConverter<XmlModel>();
        var xmlModel = await converter3.ReadAsync(httpResponseMessage3);
        Assert.NotNull(xmlModel);
        Assert.Equal("Furion", xmlModel.Name);
        Assert.Equal(30, xmlModel.Age);
    }

    [Fact]
    public async Task ReadAsync_WithCancellationToken_ReturnOK()
    {
        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        using var cancellationTokenSource = new CancellationTokenSource();

        var converter = new ObjectContentConverter<ObjectModel>();
        var objectModel = await converter.ReadAsync(httpResponseMessage, cancellationTokenSource.Token);
        Assert.NotNull(objectModel);
        Assert.Equal(10, objectModel.Id);
        Assert.Equal("furion", objectModel.Name);

        using var stringContent2 = new StringContent("""{"Id":10, "Name":"furion"}""");
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        using var cancellationTokenSource2 = new CancellationTokenSource();

        var converter2 = new ObjectContentConverter<ObjectModel>();
        var objectModel2 = await converter2.ReadAsync(httpResponseMessage2, cancellationTokenSource2.Token);
        Assert.NotNull(objectModel2);
        Assert.Equal(10, objectModel2.Id);
        Assert.Equal("furion", objectModel2.Name);
    }

    [Fact]
    public void Read_WithType_ReturnOK()
    {
        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new ObjectContentConverter();
        var objectModel = converter.Read(typeof(ObjectModel), httpResponseMessage) as ObjectModel;

        Assert.NotNull(objectModel);
        Assert.Equal(10, objectModel.Id);
        Assert.Equal("furion", objectModel.Name);

        using var stringContent2 = new StringContent("""{"Id":10, "Name":"furion"}""");
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        var converter2 = new ObjectContentConverter();
        var objectModel2 = converter2.Read(typeof(ObjectModel), httpResponseMessage2) as ObjectModel;
        Assert.NotNull(objectModel2);
        Assert.Equal(10, objectModel2.Id);
        Assert.Equal("furion", objectModel2.Name);

        using var stringContent3 = new StringContent("""
                                                     <XmlModel>
                                                        <Name>Furion</Name>
                                                        <Age>30</Age>
                                                     </XmlModel>
                                                     """, Encoding.UTF8,
            new MediaTypeHeaderValue("application/xml"));
        var httpResponseMessage3 = new HttpResponseMessage();
        httpResponseMessage3.Content = stringContent3;

        var converter3 = new ObjectContentConverter();
        var xmlModel = converter3.Read(typeof(XmlModel), httpResponseMessage3) as XmlModel;
        Assert.NotNull(xmlModel);
        Assert.Equal("Furion", xmlModel.Name);
        Assert.Equal(30, xmlModel.Age);
    }

    [Fact]
    public async Task ReadAsync_WithType_ReturnOK()
    {
        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new ObjectContentConverter();
        var objectModel = await converter.ReadAsync(typeof(ObjectModel), httpResponseMessage) as ObjectModel;
        Assert.NotNull(objectModel);
        Assert.Equal(10, objectModel.Id);
        Assert.Equal("furion", objectModel.Name);

        using var stringContent2 = new StringContent("""{"Id":10, "Name":"furion"}""");
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        var converter2 = new ObjectContentConverter();
        var objectModel2 = await converter2.ReadAsync(typeof(ObjectModel), httpResponseMessage2) as ObjectModel;
        Assert.NotNull(objectModel2);
        Assert.Equal(10, objectModel2.Id);
        Assert.Equal("furion", objectModel2.Name);

        using var stringContent3 = new StringContent("""
                                                     <XmlModel>
                                                        <Name>Furion</Name>
                                                        <Age>30</Age>
                                                     </XmlModel>
                                                     """, Encoding.UTF8,
            new MediaTypeHeaderValue("application/xml"));
        var httpResponseMessage3 = new HttpResponseMessage();
        httpResponseMessage3.Content = stringContent3;

        var converter3 = new ObjectContentConverter();
        var xmlModel = await converter3.ReadAsync(typeof(XmlModel), httpResponseMessage3) as XmlModel;
        Assert.NotNull(xmlModel);
        Assert.Equal("Furion", xmlModel.Name);
        Assert.Equal(30, xmlModel.Age);
    }

    [Fact]
    public async Task ReadAsync_WithType_WithCancellationToken_ReturnOK()
    {
        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        using var cancellationTokenSource = new CancellationTokenSource();

        var converter = new ObjectContentConverter();
        var objectModel =
            await converter.ReadAsync(typeof(ObjectModel), httpResponseMessage, cancellationTokenSource.Token) as
                ObjectModel;
        Assert.NotNull(objectModel);
        Assert.Equal(10, objectModel.Id);
        Assert.Equal("furion", objectModel.Name);

        using var stringContent2 = new StringContent("""{"Id":10, "Name":"furion"}""");
        var httpResponseMessage2 = new HttpResponseMessage();
        httpResponseMessage2.Content = stringContent2;

        using var cancellationTokenSource2 = new CancellationTokenSource();

        var converter2 = new ObjectContentConverter();
        var objectModel2 =
            await converter2.ReadAsync(typeof(ObjectModel), httpResponseMessage2, cancellationTokenSource2.Token) as
                ObjectModel;
        Assert.NotNull(objectModel2);
        Assert.Equal(10, objectModel2.Id);
        Assert.Equal("furion", objectModel2.Name);
    }

    [Fact]
    public async Task ReadAsync_WithJsonNamingPolicy_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpClient(string.Empty).ConfigureOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        });
        var serviceProvider = services.BuildServiceProvider();

        using var stringContent = new StringContent("""{"user_id":10, "user_name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new ObjectContentConverter<ObjectModelSnakeCase> { ServiceProvider = serviceProvider };
        var objectModel = await converter.ReadAsync(httpResponseMessage);
        Assert.NotNull(objectModel);
        Assert.Equal(10, objectModel.UserId);
        Assert.Equal("furion", objectModel.UserName);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public void GetJsonSerializerOptions_Invalid_Parameters()
    {
        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new ObjectContentConverter<ObjectModel>();
        var getJsonSerializerOptionsMethod = typeof(ObjectContentConverter).GetMethod("GetJsonSerializerOptions",
            BindingFlags.NonPublic | BindingFlags.Instance)!;

        var exception = Assert.Throws<TargetInvocationException>(() =>
            getJsonSerializerOptionsMethod.Invoke(converter, [null!]));
        Assert.True(exception.InnerException is ArgumentNullException);
    }

    [Fact]
    public void GetJsonSerializerOptions_WithDefault_ReturnOK()
    {
        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new ObjectContentConverter<ObjectModel>();
        var getJsonSerializerOptionsMethod = typeof(ObjectContentConverter).GetMethod("GetJsonSerializerOptions",
            BindingFlags.NonPublic | BindingFlags.Instance)!;

        var jsonSerializerOptions =
            getJsonSerializerOptionsMethod.Invoke(converter, [httpResponseMessage]) as JsonSerializerOptions;
        Assert.NotNull(jsonSerializerOptions);
        Assert.False(jsonSerializerOptions.IncludeFields);
    }

    [Fact]
    public void GetJsonSerializerOptions_WithHttpClientOptions_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpClient(string.Empty).ConfigureOptions(options =>
        {
            options.JsonSerializerOptions.IncludeFields = true;
        });
        var serviceProvider = services.BuildServiceProvider();

        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new ObjectContentConverter<ObjectModel> { ServiceProvider = serviceProvider };
        var getJsonSerializerOptionsMethod = typeof(ObjectContentConverter).GetMethod("GetJsonSerializerOptions",
            BindingFlags.NonPublic | BindingFlags.Instance)!;

        var jsonSerializerOptions =
            getJsonSerializerOptionsMethod.Invoke(converter, [httpResponseMessage]) as JsonSerializerOptions;
        Assert.NotNull(jsonSerializerOptions);
        Assert.True(jsonSerializerOptions.IncludeFields);

        serviceProvider.Dispose();
    }

    [Fact]
    public void GetJsonSerializerOptions_WithHttpRemoteOptions_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote().ConfigureOptions(options =>
        {
            options.JsonSerializerOptions.IncludeFields = true;
        });
        var serviceProvider = services.BuildServiceProvider();

        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new ObjectContentConverter<ObjectModel> { ServiceProvider = serviceProvider };
        var getJsonSerializerOptionsMethod = typeof(ObjectContentConverter).GetMethod("GetJsonSerializerOptions",
            BindingFlags.NonPublic | BindingFlags.Instance)!;

        var jsonSerializerOptions =
            getJsonSerializerOptionsMethod.Invoke(converter, [httpResponseMessage]) as JsonSerializerOptions;
        Assert.NotNull(jsonSerializerOptions);
        Assert.True(jsonSerializerOptions.IncludeFields);

        serviceProvider.Dispose();
    }

    [Fact]
    public void HasXmlContentType_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = new StringContent("""{"id":10, "name":"furion"}""", Encoding.UTF8,
            new MediaTypeHeaderValue("application/json"));

        var converter = new ObjectContentConverter<ObjectModel>();
        var hasXmlContentTypeMethod = typeof(ObjectContentConverter).GetMethod("HasXmlContentType",
            BindingFlags.NonPublic | BindingFlags.Instance)!;
        Assert.False((bool)hasXmlContentTypeMethod.Invoke(converter, [httpResponseMessage])!);

        httpResponseMessage.Content = new StringContent("""
                                                        <XmlModel>
                                                           <Name>Furion</Name>
                                                           <Age>30</Age>
                                                        </XmlModel>
                                                        """, Encoding.UTF8,
            new MediaTypeHeaderValue("application/xml"));
        Assert.True((bool)hasXmlContentTypeMethod.Invoke(converter, [httpResponseMessage])!);

        httpResponseMessage.Content = new StringContent("""
                                                        <XmlModel>
                                                           <Name>Furion</Name>
                                                           <Age>30</Age>
                                                        </XmlModel>
                                                        """, Encoding.UTF8,
            new MediaTypeHeaderValue("text/xml"));
        Assert.True((bool)hasXmlContentTypeMethod.Invoke(converter, [httpResponseMessage])!);

        httpResponseMessage.Content = new StringContent("""
                                                        <XmlModel>
                                                           <Name>Furion</Name>
                                                           <Age>30</Age>
                                                        </XmlModel>
                                                        """, Encoding.UTF8,
            new MediaTypeHeaderValue("application/xml-patch+xml"));
        Assert.True((bool)hasXmlContentTypeMethod.Invoke(converter, [httpResponseMessage])!);
    }

    [Fact]
    public void DeserializeXml_ReturnOK()
    {
        var converter = new ObjectContentConverter<XmlModel>();
        var deserializeXmlMethod = typeof(ObjectContentConverter)
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

        var deserializeXmlGenericMethod = typeof(ObjectContentConverter)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(m => m is { Name: "DeserializeXml", IsGenericMethod: true })!;

        var xmlModel2 =
            deserializeXmlGenericMethod.MakeGenericMethod(typeof(XmlModel)).Invoke(converter, [xmlString]) as XmlModel;
        Assert.NotNull(xmlModel2);
        Assert.Equal("Furion", xmlModel2.Name);
        Assert.Equal(30, xmlModel2.Age);
    }
}