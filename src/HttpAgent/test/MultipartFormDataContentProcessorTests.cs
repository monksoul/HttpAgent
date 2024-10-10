﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class MultipartFormDataContentProcessorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var multipartFormDataContentProcessor = new MultipartFormDataContentProcessor();
        Assert.NotNull(multipartFormDataContentProcessor);
        Assert.True(typeof(IHttpContentProcessor).IsAssignableFrom(typeof(MultipartFormDataContentProcessor)));
    }

    [Fact]
    public void CanProcess_ReturnOK()
    {
        var multipartFormDataContentProcessor = new MultipartFormDataContentProcessor();

        Assert.False(multipartFormDataContentProcessor.CanProcess(null, "application/octet-stream"));
        Assert.False(multipartFormDataContentProcessor.CanProcess(null, "application/x-www-form-urlencoded"));
        Assert.True(multipartFormDataContentProcessor.CanProcess(null, "multipart/form-data"));
        Assert.True(multipartFormDataContentProcessor.CanProcess(null, "Multipart/Form-data"));

        Assert.True(multipartFormDataContentProcessor.CanProcess(new { }, "multipart/form-data"));
        Assert.True(multipartFormDataContentProcessor.CanProcess(new MultipartContent(),
            "multipart/form-data"));
    }

    [Fact]
    public void Process_Invalid_Parameters()
    {
        var processor = new MultipartFormDataContentProcessor();

        Assert.Throws<NotImplementedException>(() =>
        {
            processor.Process("furion", "multipart/form-data", null);
        });
    }

    [Fact]
    public void Process_ReturnOK()
    {
        var processor = new MultipartFormDataContentProcessor();

        Assert.Null(processor.Process(null, "multipart/form-data", null));

        var multipartFormDataContent =
            new MultipartContent();
        var httpContent1 = processor.Process(multipartFormDataContent, "multipart/form-data", null);
        Assert.Same(multipartFormDataContent, httpContent1);
    }
}