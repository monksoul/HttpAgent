// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class GenericHttpContentConverterTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var converter = new GenericHttpContentConverter(typeof(IAsyncEnumerable<>), typeArgs =>
            (IHttpContentConverter)Activator.CreateInstance(
                typeof(AsyncEnumerableContentConverter<>).MakeGenericType(typeArgs[0]))!);

        Assert.NotNull(converter.GenericType);
        Assert.NotNull(converter.Factory);
    }
}