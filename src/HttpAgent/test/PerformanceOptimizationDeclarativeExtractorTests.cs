﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class PerformanceOptimizationDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(PerformanceOptimizationDeclarativeExtractor)));

        var extractor = new PerformanceOptimizationDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IPerformanceOptimizationDeclarativeExtractorTest1).GetMethod(
                nameof(IPerformanceOptimizationDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, []);
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new PerformanceOptimizationDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.False(httpRequestBuilder1.PerformanceOptimizationEnabled);

        var method2 =
            typeof(IPerformanceOptimizationDeclarativeExtractorTest2).GetMethod(
                nameof(IPerformanceOptimizationDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, []);
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new PerformanceOptimizationDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.True(httpRequestBuilder2.PerformanceOptimizationEnabled);

        var method3 =
            typeof(IPerformanceOptimizationDeclarativeExtractorTest2).GetMethod(
                nameof(IPerformanceOptimizationDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, []);
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new PerformanceOptimizationDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.False(httpRequestBuilder3.PerformanceOptimizationEnabled);
    }
}

public interface IPerformanceOptimizationDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[PerformanceOptimization]
public interface IPerformanceOptimizationDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [PerformanceOptimization(false)]
    [Post("http://localhost:5000")]
    Task Test2();
}