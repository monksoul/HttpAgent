// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ObjectContentConverterFactoryTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var factory = new ObjectContentConverterFactory();
        Assert.NotNull(factory);
    }

    [Fact]
    public void GetConverter_ReturnOK()
    {
        var factory = new ObjectContentConverterFactory();
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        Assert.True(factory.GetConverter(typeof(object), httpResponseMessage).GetType() ==
                    typeof(ObjectContentConverter));
        Assert.True(factory.GetConverter<object>(httpResponseMessage).GetType() ==
                    typeof(ObjectContentConverter<object>));

        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
        Assert.True(factory.GetConverter(typeof(object), httpResponseMessage).GetType() ==
                    typeof(XmlObjectContentConverter));
        Assert.True(factory.GetConverter<object>(httpResponseMessage).GetType() ==
                    typeof(XmlObjectContentConverter<object>));

        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml-patch+xml");
        Assert.True(factory.GetConverter(typeof(object), httpResponseMessage).GetType() ==
                    typeof(XmlObjectContentConverter));
        Assert.True(factory.GetConverter<object>(httpResponseMessage).GetType() ==
                    typeof(XmlObjectContentConverter<object>));

        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
        Assert.True(factory.GetConverter(typeof(object), httpResponseMessage).GetType() ==
                    typeof(XmlObjectContentConverter));
        Assert.True(factory.GetConverter<object>(httpResponseMessage).GetType() ==
                    typeof(XmlObjectContentConverter<object>));
    }
}