// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpStressTestHarnessBuilderTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new HttpStressTestHarnessBuilder(null!, null));

    [Fact]
    public void New_ReturnOK()
    {
        var builder = new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Equal(HttpMethod.Get, builder.HttpMethod);
        Assert.NotNull(builder.RequestUri);
        Assert.Equal("http://localhost/", builder.RequestUri.ToString());
        Assert.Equal(100, builder.NumberOfRequests);
        Assert.Equal(100, builder.MaxDegreeOfParallelism);
        Assert.Equal(1, builder.NumberOfRounds);
        Assert.Null(builder._requestConfigure);
    }

    [Fact]
    public void SetNumberOfRequests_Invalid_Parameters()
    {
        var builder = new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var exception = Assert.Throws<ArgumentException>(() => builder.SetNumberOfRequests(-1));
        Assert.Equal("Number of requests must be greater than 0. (Parameter 'numberOfRequests')", exception.Message);

        Assert.Throws<ArgumentException>(() => builder.SetNumberOfRequests(0));
    }

    [Fact]
    public void SetNumberOfRequests_ReturnOK()
    {
        var builder = new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder.SetNumberOfRequests(1000);
        Assert.Equal(1000, builder.NumberOfRequests);
    }

    [Fact]
    public void SetMaxDegreeOfParallelism_Invalid_Parameters()
    {
        var builder = new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var exception = Assert.Throws<ArgumentException>(() => builder.SetMaxDegreeOfParallelism(-1));
        Assert.Equal("Max degree of parallelism must be greater than 0. (Parameter 'maxDegreeOfParallelism')",
            exception.Message);

        Assert.Throws<ArgumentException>(() => builder.SetMaxDegreeOfParallelism(0));
    }

    [Fact]
    public void SetMaxDegreeOfParallelism_ReturnOK()
    {
        var builder = new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder.SetMaxDegreeOfParallelism(50);
        Assert.Equal(50, builder.MaxDegreeOfParallelism);
    }

    [Fact]
    public void SetNumberOfRounds_Invalid_Parameters()
    {
        var builder = new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var exception = Assert.Throws<ArgumentException>(() => builder.SetNumberOfRounds(-1));
        Assert.Equal("Number of rounds must be greater than 0. (Parameter 'numberOfRounds')",
            exception.Message);

        Assert.Throws<ArgumentException>(() => builder.SetNumberOfRounds(0));
    }

    [Fact]
    public void SetNumberOfRounds_ReturnOK()
    {
        var builder = new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder.SetNumberOfRounds(5);
        Assert.Equal(5, builder.NumberOfRounds);
    }

    [Fact]
    public void WithRequest_Invalid_Parameters()
    {
        var builder = new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.WithRequest(null!));
    }

    [Fact]
    public void WithRequest_ReturnOK()
    {
        var builder = new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Null(builder._requestConfigure);
        builder.WithRequest(requestBuilder => requestBuilder.WithHeader("framework", "Furion"));
        Assert.NotNull(builder._requestConfigure);
    }

    [Fact]
    public void DisableCache_ReturnOK()
    {
        var builder = new HttpStressTestHarnessBuilder(HttpMethod.Get, null);
        builder.DisableCache();

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null);
        builder._requestConfigure?.Invoke(httpRequestBuilder);
        Assert.True(httpRequestBuilder.DisableCacheEnabled);

        builder.DisableCache(false);
        builder._requestConfigure?.Invoke(httpRequestBuilder);
        Assert.False(httpRequestBuilder.DisableCacheEnabled);
    }

    [Fact]
    public void Build_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new HttpStressTestHarnessBuilder(HttpMethod.Post, new Uri("http://localhost")).Build(null!));

    [Fact]
    public void Build_ReturnOK()
    {
        var httpRemoteOptions = new HttpRemoteOptions();
        var builder = new HttpStressTestHarnessBuilder(HttpMethod.Post, new Uri("http://localhost"));
        var httpRequestBuilder = builder.Build(httpRemoteOptions);

        Assert.NotNull(httpRequestBuilder);
        Assert.Equal(HttpMethod.Post, httpRequestBuilder.HttpMethod);
        Assert.NotNull(httpRequestBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder.RequestUri.ToString());
        Assert.True(httpRequestBuilder.HttpClientPoolingEnabled);
        Assert.False(httpRequestBuilder.EnsureSuccessStatusCodeEnabled);
        Assert.NotNull(httpRequestBuilder.Headers);
        Assert.Single(httpRequestBuilder.Headers);
        Assert.Equal("X-Stress-Test", httpRequestBuilder.Headers.Keys.First());
        Assert.Equal("Harness", httpRequestBuilder.Headers["X-Stress-Test"].First());
        Assert.False(httpRequestBuilder.ProfilerEnabled);
        Assert.True(httpRequestBuilder.__Disable_Profiler__);
        Assert.True(httpRequestBuilder.PerformanceOptimizationEnabled);

        var builder2 = new HttpStressTestHarnessBuilder(HttpMethod.Post, new Uri("http://localhost"));
        var httpRequestBuilder2 = builder2.WithRequest(b => b.EnsureSuccessStatusCode()).Build(httpRemoteOptions);
        Assert.True(httpRequestBuilder2.EnsureSuccessStatusCodeEnabled);
    }
}