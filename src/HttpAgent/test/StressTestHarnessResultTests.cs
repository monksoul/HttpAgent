// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class StressTestHarnessResultTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        var exception = Assert.Throws<ArgumentException>(() => new StressTestHarnessResult(10,
            0.9282536, 10, 0,
            []));

        Assert.Equal(
            "The number of response times (0) does not match the total number of requests (10). (Parameter 'responseTimes')",
            exception.Message);
    }

    [Fact]
    public void New_ReturnOK()
    {
        long[] responseTimes =
            [7207818, 5979207, 4279881, 9191534, 9209802, 6536028, 4388344, 3655751, 6319666, 6184336];

        var result = new StressTestHarnessResult(10, 0.9282536, 10, 0,
            responseTimes);

        Assert.Equal(10000, StressTestHarnessResult._ticksPerMillisecond);
        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(0.9282536, result.TotalTimeInSeconds);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.Equal(10.772918090487341, result.QueriesPerSecond);

        Assert.Equal(365.5751, result.MinResponseTime);
        Assert.Equal(920.9802, result.MaxResponseTime);
        Assert.Equal(629.5236, result.AverageResponseTime);

        Assert.Equal(365.5751, result.Percentile10ResponseTime);
        Assert.Equal(438.8344, result.Percentile25ResponseTime);
        Assert.Equal(618.4336, result.Percentile50ResponseTime);
        Assert.Equal(720.7818, result.Percentile75ResponseTime);
        Assert.Equal(919.1534, result.Percentile90ResponseTime);
        Assert.Equal(920.9802, result.Percentile99ResponseTime);
        Assert.Equal(920.9802, result.Percentile9999ResponseTime);
    }

    [Fact]
    public void ToString_ReturnOK()
    {
        long[] responseTimes =
            [7207818, 5979207, 4279881, 9191534, 9209802, 6536028, 4388344, 3655751, 6319666, 6184336];

        var result = new StressTestHarnessResult(10, 0.9282536, 10, 0,
            responseTimes);

        Assert.Equal(
            "[34m[1mStress Test Harness Result:[0m \r\n  Total Requests:          10\r\n  Total Time (s):          0.93\r\n  Successful Requests:     10\r\n  Failed Requests:         0\r\n  QPS:                     10.77\r\n  Min RT (ms):             365.58\r\n  Max RT (ms):             920.98\r\n  Avg RT (ms):             629.52\r\n  P10 RT (ms):             365.58\r\n  P25 RT (ms):             438.83\r\n  P50 RT (ms):             618.43\r\n  P75 RT (ms):             720.78\r\n  P90 RT (ms):             919.15\r\n  P99 RT (ms):             920.98\r\n  P99.99 RT (ms):          920.98",
            result.ToString());
    }

    [Fact]
    public void CalculateQueriesPerSecond_ReturnOK()
    {
        long[] responseTimes =
            [7207818, 5979207, 4279881, 9191534, 9209802, 6536028, 4388344, 3655751, 6319666, 6184336];

        var result = new StressTestHarnessResult(10, 0.9282536, 10, 0,
            responseTimes);

        result.CalculateQueriesPerSecond(100, 0);
        Assert.Equal(0, result.QueriesPerSecond);

        result.CalculateQueriesPerSecond(10, 2);
        Assert.Equal(5, result.QueriesPerSecond);
    }

    [Fact]
    public void CalculateMinMaxAvgResponseTime_ReturnOK()
    {
        long[] responseTimes =
            [7207818, 5979207, 4279881, 9191534, 9209802, 6536028, 4388344, 3655751, 6319666, 6184336];

        var result = new StressTestHarnessResult(10, 0.9282536, 10, 0,
            responseTimes);

        result.CalculateMinMaxAvgResponseTime(responseTimes, 10);

        Assert.Equal(365.5751, result.MinResponseTime);
        Assert.Equal(920.9802, result.MaxResponseTime);
        Assert.Equal(629.5236, result.AverageResponseTime);
    }

    [Fact]
    public void CalculatePercentiles_ReturnOK()
    {
        long[] responseTimes =
            [7207818, 5979207, 4279881, 9191534, 9209802, 6536028, 4388344, 3655751, 6319666, 6184336];

        var result = new StressTestHarnessResult(10, 0.9282536, 10, 0,
            responseTimes);

        result.CalculatePercentiles(responseTimes);

        Assert.Equal(365.5751, result.Percentile10ResponseTime);
        Assert.Equal(438.8344, result.Percentile25ResponseTime);
        Assert.Equal(618.4336, result.Percentile50ResponseTime);
        Assert.Equal(720.7818, result.Percentile75ResponseTime);
        Assert.Equal(919.1534, result.Percentile90ResponseTime);
        Assert.Equal(920.9802, result.Percentile99ResponseTime);
        Assert.Equal(920.9802, result.Percentile9999ResponseTime);
    }

    [Fact]
    public void CalculatePercentile_ReturnOK()
    {
        long[] responseTimes =
            [7207818, 5979207, 4279881, 9191534, 9209802, 6536028, 4388344, 3655751, 6319666, 6184336];

        var sortedResponseTimes = responseTimes.OrderBy(x => x).ToArray();

        Assert.Equal(365.5751, StressTestHarnessResult.CalculatePercentile(sortedResponseTimes, 0.1));
        Assert.Equal(438.8344, StressTestHarnessResult.CalculatePercentile(sortedResponseTimes, 0.25));
        Assert.Equal(618.4336, StressTestHarnessResult.CalculatePercentile(sortedResponseTimes, 0.5));
        Assert.Equal(720.7818, StressTestHarnessResult.CalculatePercentile(sortedResponseTimes, 0.75));
        Assert.Equal(919.1534, StressTestHarnessResult.CalculatePercentile(sortedResponseTimes, 0.9));
        Assert.Equal(920.9802, StressTestHarnessResult.CalculatePercentile(sortedResponseTimes, 0.99));
        Assert.Equal(920.9802, StressTestHarnessResult.CalculatePercentile(sortedResponseTimes, 0.9999));
    }
}