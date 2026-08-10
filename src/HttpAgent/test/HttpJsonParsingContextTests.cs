// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpJsonParsingContextTests
{
    private static JsonObject CreateRootObject(string json) =>
        JsonNode.Parse(json, new JsonNodeOptions { PropertyNameCaseInsensitive = true })!.AsObject();

    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new HttpJsonParsingContext(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var root = CreateRootObject("{\"key\":\"value\"}");
        var context = new HttpJsonParsingContext(root);
        Assert.Same(root, context.RootObject);
    }

    [Fact]
    public void TryGetNode_Invalid_Parameters()
    {
        var context = new HttpJsonParsingContext(CreateRootObject("{}"));
        Assert.Throws<ArgumentNullException>(() => context.TryGetNode(null!, out _));
    }

    [Fact]
    public void TryGetNode_ReturnOK()
    {
        var context = new HttpJsonParsingContext(CreateRootObject("{\"name\":\"Furion\"}"));
        Assert.True(context.TryGetNode("name", out var node));
        Assert.NotNull(node);
        Assert.Equal("Furion", node.GetValue<string>());
        Assert.False(context.TryGetNode("missing", out _));
    }

    [Fact]
    public void GetNode_Invalid_Parameters()
    {
        var context = new HttpJsonParsingContext(CreateRootObject("{}"));
        Assert.Throws<ArgumentNullException>(() => context.GetNode(null!));
    }

    [Fact]
    public void GetNode_ReturnOK()
    {
        var context = new HttpJsonParsingContext(CreateRootObject("{\"id\":1}"));
        var node = context.GetNode("id");
        Assert.NotNull(node);
        Assert.Equal(1, node.GetValue<int>());
        Assert.Null(context.GetNode("nonexistent"));
    }

    [Fact]
    public void ContainsProperty_Invalid_Parameters()
    {
        var context = new HttpJsonParsingContext(CreateRootObject("{}"));
        Assert.Throws<ArgumentNullException>(() => context.ContainsProperty(null!));
    }

    [Fact]
    public void ContainsProperty_ReturnOK()
    {
        var context = new HttpJsonParsingContext(CreateRootObject("{\"active\":true}"));
        Assert.True(context.ContainsProperty("active"));
        Assert.False(context.ContainsProperty("inactive"));
    }
}