// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class FormNamingPolicyTests
{
    [Fact]
    public void Definition_ReturnOK()
    {
        var names = Enum.GetNames<FormNamingPolicy>();
        Assert.Equal(6, names.Length);

        string[] strings =
        [
            nameof(FormNamingPolicy.None), nameof(FormNamingPolicy.CamelCase),
            nameof(FormNamingPolicy.SnakeCaseLower), nameof(FormNamingPolicy.SnakeCaseUpper),
            nameof(FormNamingPolicy.KebabCaseLower),
            nameof(FormNamingPolicy.KebabCaseUpper)
        ];
        Assert.True(strings.SequenceEqual(names));
    }
}