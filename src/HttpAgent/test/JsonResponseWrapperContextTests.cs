// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonResponseWrapperContextTests
{
    public JsonResponseWrapperContextTests() => JsonResponseWrapperContext._getterCache.Clear();

    [Fact]
    public void New_ReturnOK()
    {
        var context = new JsonResponseWrapperContext(null, null, new HttpResponseMessage());
        Assert.Null(context.Instance);
        Assert.Null(context.Result);
        Assert.NotNull(context.ResponseMessage);

        Assert.NotNull(JsonResponseWrapperContext._getterCache);
        Assert.Empty(JsonResponseWrapperContext._getterCache);
    }

    [Fact]
    public void GetPropertyValue_Invalid_Parameters()
    {
        var context = new JsonResponseWrapperContext(null, null, new HttpResponseMessage());
        Assert.Throws<ArgumentNullException>(() => context.GetPropertyValue<object>(null!));
        Assert.Throws<ArgumentException>(() => context.GetPropertyValue<object>(string.Empty));
        Assert.Throws<ArgumentException>(() => context.GetPropertyValue<object>(" "));
    }

    [Fact]
    public void GetPropertyValue_ReturnOK()
    {
        var apiResult = new ApiResult<JsonModel> { Success = true, Data = new JsonModel { Id = 1, Name = "Furion" } };
        var context = new JsonResponseWrapperContext(apiResult, apiResult.Data, new HttpResponseMessage());

        Assert.Empty(JsonResponseWrapperContext._getterCache);
        Assert.True(context.GetPropertyValue<bool>(nameof(ApiResult<>.Success)));
        Assert.Single(JsonResponseWrapperContext._getterCache);
    }

    [Fact]
    public void BuildPropertyGetter_Invalid_Parameters()
    {
        var exception =
            Assert.Throws<InvalidOperationException>(() =>
                JsonResponseWrapperContext.BuildPropertyGetter(typeof(ApiResult<JsonModel>), "Data1"));
        Assert.Equal(
            "Property 'Data1' not found on type 'HttpAgent.Tests.JsonResponseWrapperContextTests+ApiResult`1[HttpAgent.Tests.JsonResponseWrapperContextTests+JsonModel]'.",
            exception.Message);
    }

    [Fact]
    public void BuildPropertyGetter_ReturnOK()
    {
        var func = JsonResponseWrapperContext.BuildPropertyGetter(typeof(ApiResult<JsonModel>),
            nameof(ApiResult<>.Data));

        var apiResult = new ApiResult<JsonModel> { Success = true, Data = new JsonModel { Id = 1, Name = "Furion" } };
        var jsonModel = func(apiResult) as JsonModel;

        Assert.NotNull(jsonModel);
        Assert.Equal(1, jsonModel.Id);
        Assert.Equal("Furion", jsonModel.Name);
    }

    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
    }

    public class ApiResult<T, Z>;

    public class JsonModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}