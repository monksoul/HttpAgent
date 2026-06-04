namespace HttpAgent.Tests;

public class JsonLinesContentProcessorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var processor = new JsonLinesContentProcessor();
        Assert.NotNull(processor);
        Assert.True(typeof(IHttpContentProcessor).IsAssignableFrom(typeof(JsonLinesContentProcessor)));
    }

    [Fact]
    public void CanProcess_ReturnOK()
    {
        var processor = new JsonLinesContentProcessor();

        Assert.False(processor.CanProcess(new HttpContentProcessorContext(null, "application/json")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(null, "application/x-ndjson")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(null, "application/x-jsonlines")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(null, "application/jsonlines")));
        Assert.True(processor.CanProcess(new HttpContentProcessorContext(null, "application/jsonl")));
    }

    [Fact]
    public void Process_Invalid_Parameters()
    {
        var processor = new JsonLinesContentProcessor();

        var exception =
            Assert.Throws<InvalidOperationException>(() =>
                processor.Process(new HttpContentProcessorContext("Furion", "application/x-ndjson", Encoding.UTF8)));
        Assert.Equal(
            "Expected IEnumerable<T> where T is a class type other than string, but received type `System.String`.",
            exception.Message);
    }

    [Fact]
    public async Task Process_ReturnOK()
    {
        var processor = new JsonLinesContentProcessor();

        Assert.Null(processor.Process(new HttpContentProcessorContext(null, "application/x-ndjson")));

        var stringContent = new StringContent("Hello World");
        var httpContent1 =
            processor.Process(new HttpContentProcessorContext(stringContent, "application/x-ndjson"));
        Assert.Same(stringContent, httpContent1);

        var list = new List<JsonModel> { new() { Id = 1, Name = "Furion" }, new() { Id = 2, Name = "百小僧" } };
        var httpContent2 = processor.Process(new HttpContentProcessorContext(list, "application/x-ndjson"));
        Assert.NotNull(httpContent2);
        var str = await httpContent2.ReadAsStringAsync();
        Assert.Equal("{\"id\":1,\"name\":\"Furion\"}\n{\"id\":2,\"name\":\"百小僧\"}", str);
    }
}

file class JsonModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}