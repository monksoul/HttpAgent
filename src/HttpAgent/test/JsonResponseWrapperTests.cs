// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonResponseWrapperTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new JsonResponseWrapper(null!, null!));
        Assert.Throws<ArgumentNullException>(() => new JsonResponseWrapper(typeof(ApiResult<>), null!));
        Assert.Throws<ArgumentException>(() => new JsonResponseWrapper(typeof(ApiResult<>), string.Empty));
        Assert.Throws<ArgumentException>(() => new JsonResponseWrapper(typeof(ApiResult<>), " "));

        var exception =
            Assert.Throws<ArgumentException>(() =>
                new JsonResponseWrapper(typeof(ApiResult<string>), nameof(ApiResult<>.Data)));
        Assert.Equal(
            "The provided type must be a generic type definition (e.g., typeof(ApiResult<>)). Actual type: HttpAgent.Tests.JsonResponseWrapperTests+ApiResult`1[System.String]. (Parameter 'genericType')",
            exception.Message);

        var exception2 =
            Assert.Throws<ArgumentException>(() =>
                new JsonResponseWrapper(typeof(ApiResult<,>), nameof(ApiResult<>.Data)));
        Assert.Equal(
            "The provided type must be a generic type definition (e.g., typeof(ApiResult<>)). Actual type: HttpAgent.Tests.JsonResponseWrapperTests+ApiResult`2[T,Z]. (Parameter 'genericType')",
            exception2.Message);
    }

    [Fact]
    public void New_ReturnOK()
    {
        var wrapper = new JsonResponseWrapper(typeof(ApiResult<>), nameof(ApiResult<>.Data));
        Assert.Equal(typeof(ApiResult<>), wrapper.GenericType);
        Assert.Equal("Data", wrapper.PropertyName);
    }

    [Fact]
    public void GetResultValue_Invalid_Parameters()
    {
        var wrapper = new JsonResponseWrapper(typeof(ApiResult<>), nameof(ApiResult<>.Data));
        var exception =
            Assert.Throws<ArgumentException>(() => wrapper.GetResultValue(new object(), new HttpResponseMessage()));
        Assert.Equal(
            "The instance type 'System.Object' is not a constructed generic type of 'HttpAgent.Tests.JsonResponseWrapperTests+ApiResult`1[T]'. (Parameter 'instance')",
            exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() =>
            wrapper.GetResultValue(new ApiResult<string, int>(), new HttpResponseMessage()));
        Assert.Equal(
            "The instance type 'HttpAgent.Tests.JsonResponseWrapperTests+ApiResult`2[System.String,System.Int32]' is not a constructed generic type of 'HttpAgent.Tests.JsonResponseWrapperTests+ApiResult`1[T]'. (Parameter 'instance')",
            exception2.Message);
    }

    [Fact]
    public void GetResultValue_ReturnOK()
    {
        var wrapper = new JsonResponseWrapper(typeof(ApiResult<>), nameof(ApiResult<>.Data));
        Assert.Null(wrapper.GetResultValue(null, new HttpResponseMessage()));

        var apiResult = new ApiResult<JsonModel> { Success = true, Data = new JsonModel { Id = 1, Name = "Furion" } };
        var jsonModel = wrapper.GetResultValue(apiResult, new HttpResponseMessage()) as JsonModel;

        Assert.NotNull(jsonModel);
        Assert.Equal(1, jsonModel.Id);
        Assert.Equal("Furion", jsonModel.Name);
    }

    [Fact]
    public void GetResultValue_WithResultHandler_ReturnOK()
    {
        var wrapper = new JsonResponseWrapper(typeof(ApiResult<>), nameof(ApiResult<>.Data))
        {
            ResultHandler = context =>
            {
                if (context.Instance is { } instance)
                {
                    Assert.True(context.GetPropertyValue<bool>(nameof(ApiResult<>.Success)));
                }

                context.ResponseMessage.EnsureSuccessStatusCode();
                return context.Result;
            }
        };
        Assert.Null(wrapper.GetResultValue(null, new HttpResponseMessage()));

        var apiResult = new ApiResult<JsonModel> { Success = true, Data = new JsonModel { Id = 1, Name = "Furion" } };
        var jsonModel = wrapper.GetResultValue(apiResult, new HttpResponseMessage()) as JsonModel;

        Assert.NotNull(jsonModel);
        Assert.Equal(1, jsonModel.Id);
        Assert.Equal("Furion", jsonModel.Name);

        Assert.Throws<HttpRequestException>(() =>
            wrapper.GetResultValue(apiResult, new HttpResponseMessage(HttpStatusCode.InternalServerError)));
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