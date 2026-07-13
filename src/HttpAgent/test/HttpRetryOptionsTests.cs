// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRetryOptionsTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var options = new HttpRetryOptions();
        Assert.Equal(0, options.MaxRetries);
        Assert.Equal(TimeSpan.FromSeconds(1), options.RetryInterval);
        Assert.False(options.UseExponentialBackoff);
        Assert.Null(options.RetryIntervals);
        Assert.Null(options.RetryStatusCodes);
        Assert.Null(options.RetryExceptionTypes);
        Assert.Null(options.OnRetry);
        Assert.False(options.RetryIndefinitely);
    }

    [Fact]
    public void SetMaxRetries_Invalid_Parameters()
    {
        var options = new HttpRetryOptions();

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => options.SetMaxRetries(-1));
        Assert.Equal("Max retries must be non-negative. (Parameter 'maxRetries')", exception.Message);
    }

    [Fact]
    public void SetMaxRetries_ReturnOK()
    {
        var options = new HttpRetryOptions();
        options.SetMaxRetries(10);
        Assert.Equal(10, options.MaxRetries);
    }

    [Fact]
    public void SetRetryInterval_Invalid_Parameters()
    {
        var options = new HttpRetryOptions();

        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(() => options.SetRetryInterval(TimeSpan.FromMilliseconds(-1)));
        Assert.Equal("Retry interval must be non-negative. (Parameter 'interval')", exception.Message);

        var exception2 = Assert.Throws<ArgumentOutOfRangeException>(() => options.SetRetryInterval(-1));
        Assert.Equal("Retry interval must be non-negative. (Parameter 'milliseconds')", exception2.Message);
    }

    [Fact]
    public void SetRetryInterval_ReturnOK()
    {
        var options = new HttpRetryOptions();

        options.SetRetryInterval(2000);
        Assert.Equal(TimeSpan.FromMilliseconds(2000), options.RetryInterval);

        options.SetRetryInterval(TimeSpan.FromMilliseconds(2000));
        Assert.Equal(TimeSpan.FromMilliseconds(2000), options.RetryInterval);
    }

    [Fact]
    public void SetUseExponentialBackoff_ReturnOK()
    {
        var options = new HttpRetryOptions();
        options.SetUseExponentialBackoff(true);
        Assert.True(options.UseExponentialBackoff);
    }

    [Fact]
    public void SetRetryIntervals_Invalid_Parameters()
    {
        var options = new HttpRetryOptions();

        Assert.Throws<ArgumentNullException>(() => options.SetRetryIntervals((TimeSpan[])null!));

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            options.SetRetryIntervals(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(-1)));
        Assert.Equal("Retry interval at index 1 must be non-negative. (Parameter 'intervals')", exception.Message);
    }

    [Fact]
    public void SetRetryIntervals_ReturnOK()
    {
        var options = new HttpRetryOptions();

        options.SetRetryIntervals(TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(2000));
        Assert.NotNull(options.RetryIntervals);
        Assert.Equal([TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(2000)], options.RetryIntervals);

        options.SetRetryIntervals(1000, 2000);
        Assert.NotNull(options.RetryIntervals);
        Assert.Equal([TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(2000)], options.RetryIntervals);
    }

    [Fact]
    public void AddRetryStatusCode_ReturnOK()
    {
        var options = new HttpRetryOptions();
        options.AddRetryStatusCode(HttpStatusCode.InternalServerError);
        Assert.NotNull(options.RetryStatusCodes);
        Assert.Equal([HttpStatusCode.InternalServerError], options.RetryStatusCodes);

        options.AddRetryStatusCode(401);
        Assert.Equal([HttpStatusCode.InternalServerError, HttpStatusCode.Unauthorized], options.RetryStatusCodes);
    }

    [Fact]
    public void AddRetryStatusCodes_Invalid_Parameters()
    {
        var options = new HttpRetryOptions();
        Assert.Throws<ArgumentNullException>(() => options.AddRetryStatusCodes((IEnumerable<HttpStatusCode>)null!));
        Assert.Throws<ArgumentNullException>(() => options.AddRetryStatusCodes((IEnumerable<int>)null!));
    }

    [Fact]
    public void AddRetryStatusCodes_ReturnOK()
    {
        var options = new HttpRetryOptions();
        options.AddRetryStatusCodes(HttpStatusCode.InternalServerError, HttpStatusCode.Unauthorized);
        Assert.Equal([HttpStatusCode.InternalServerError, HttpStatusCode.Unauthorized], options.RetryStatusCodes);

        options.AddRetryStatusCodes(500, 401);
        Assert.Equal([HttpStatusCode.InternalServerError, HttpStatusCode.Unauthorized], options.RetryStatusCodes);
    }

    [Fact]
    public void AddRetryException_Invalid_Parameters()
    {
        var options = new HttpRetryOptions();

        Assert.Throws<ArgumentException>(() => options.AddRetryException(null!));
        var exception = Assert.Throws<ArgumentException>(() => options.AddRetryException(typeof(string)));
        Assert.Equal("The type 'System.String' must be derived from System.Exception. (Parameter 'exceptionTypes')",
            exception.Message);
    }

    [Fact]
    public void AddRetryException_ReturnOK()
    {
        var options = new HttpRetryOptions();
        options.AddRetryException<Exception>().AddRetryException(typeof(Exception))
            .AddRetryException<InvalidCastException>();

        Assert.Equal([typeof(Exception), typeof(InvalidCastException)], options.RetryExceptionTypes);
    }

    [Fact]
    public void AddRetryExceptions_Invalid_Parameters()
    {
        var options = new HttpRetryOptions();

        Assert.Throws<ArgumentNullException>(() => options.AddRetryExceptions(null!));
        var exception = Assert.Throws<ArgumentException>(() => options.AddRetryExceptions(typeof(string)));
        Assert.Equal("The type 'System.String' must be derived from System.Exception. (Parameter 'exceptionTypes')",
            exception.Message);
    }

    [Fact]
    public void AddRetryExceptions_ReturnOK()
    {
        var options = new HttpRetryOptions();
        options.AddRetryExceptions(typeof(Exception), typeof(Exception), typeof(InvalidCastException));

        Assert.Equal([typeof(Exception), typeof(InvalidCastException)], options.RetryExceptionTypes);
    }

    [Fact]
    public void SetOnRetry_ReturnOK()
    {
        var options = new HttpRetryOptions();
        Assert.Null(options.OnRetry);

        options.SetOnRetry(_ => { });
        Assert.NotNull(options.OnRetry);
    }

    [Fact]
    public void SetRetryIndefinitely_ReturnOK()
    {
        var options = new HttpRetryOptions();
        options.SetRetryIndefinitely(true);
        Assert.True(options.RetryIndefinitely);

        options.SetRetryIndefinitely(false);
        Assert.False(options.RetryIndefinitely);
    }
}