// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpContentConverterResultTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var result = new HttpContentConverterResult<ObjectModel>(new HttpResponseMessage(), new ObjectModel(),
            new ObjectContentConverter());
        Assert.NotNull(result.ResponseMessage);
        Assert.NotNull(result.Result);
        Assert.NotNull(result.Converter);

        var result2 =
            new HttpContentConverterResult(new HttpResponseMessage(), new ObjectModel(), new ObjectContentConverter());
        Assert.NotNull(result2.ResponseMessage);
        Assert.NotNull(result2.Result);
        Assert.NotNull(result2.Converter);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var result = new HttpContentConverterResult<ObjectModel>(new HttpResponseMessage(), new ObjectModel(),
            new ObjectContentConverter());
        result.Dispose();

        var result2 =
            new HttpContentConverterResult(new HttpResponseMessage(), new ObjectModel(), new ObjectContentConverter());
        result2.Dispose();
    }
}