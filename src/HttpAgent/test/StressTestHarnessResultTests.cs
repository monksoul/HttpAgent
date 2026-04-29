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
        double[] responseTimes =
            [7207818, 5979207, 4279881, 9191534, 9209802, 6536028, 4388344, 3655751, 6319666, 6184336];

        var result = new StressTestHarnessResult(10, 0.9282536, 10, 0,
            responseTimes);

        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(0.9282536, result.TotalTimeInSeconds);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.Equal(10.772918090487341, result.QueriesPerSecond);

        Assert.Equal(3655751, result.MinResponseTime);
        Assert.Equal(9209802, result.MaxResponseTime);
        Assert.Equal(6295236.7, result.AverageResponseTime);

        Assert.Equal(3655751, result.Percentile10ResponseTime);
        Assert.Equal(4388344, result.Percentile25ResponseTime);
        Assert.Equal(6184336, result.Percentile50ResponseTime);
        Assert.Equal(7207818, result.Percentile75ResponseTime);
        Assert.Equal(9191534, result.Percentile90ResponseTime);
        Assert.Equal(9209802, result.Percentile99ResponseTime);
        Assert.Equal(9209802, result.Percentile9999ResponseTime);
    }

    [Fact]
    public void ToString_ReturnOK()
    {
        double[] responseTimes =
            [7207818, 5979207, 4279881, 9191534, 9209802, 6536028, 4388344, 3655751, 6319666, 6184336];

        var result = new StressTestHarnessResult(10, 0.9282536, 10, 0,
            responseTimes);

        var c = result.ToString();

        Assert.Equal(
            "[36m[1mStress Test Harness Result:[0m \r\n  Total Requests:          10\r\n  Total Time (s):          0.93\r\n  Successful Requests:     10\r\n  Failed Requests:         0\r\n  QPS:                     10.77\r\n  Min RT (ms):             3,655,751.00\r\n  Max RT (ms):             9,209,802.00\r\n  Avg RT (ms):             6,295,236.70\r\n  P10 RT (ms):             3,655,751.00\r\n  P25 RT (ms):             4,388,344.00\r\n  P50 RT (ms):             6,184,336.00\r\n  P75 RT (ms):             7,207,818.00\r\n  P90 RT (ms):             9,191,534.00\r\n  P99 RT (ms):             9,209,802.00\r\n  P99.99 RT (ms):          9,209,802.00",
            result.ToString());
    }

    [Fact]
    public void CalculateQueriesPerSecond_ReturnOK()
    {
        double[] responseTimes =
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
        double[] responseTimes =
            [7207818, 5979207, 4279881, 9191534, 9209802, 6536028, 4388344, 3655751, 6319666, 6184336];

        var result = new StressTestHarnessResult(10, 0.9282536, 10, 0,
            responseTimes);

        result.CalculateMinMaxAvgResponseTime(responseTimes, 10);

        Assert.Equal(3655751, result.MinResponseTime);
        Assert.Equal(9209802, result.MaxResponseTime);
        Assert.Equal(6295236.7, result.AverageResponseTime);
    }

    [Fact]
    public void CalculatePercentiles_ReturnOK()
    {
        double[] responseTimes =
            [7207818, 5979207, 4279881, 9191534, 9209802, 6536028, 4388344, 3655751, 6319666, 6184336];

        var result = new StressTestHarnessResult(10, 0.9282536, 10, 0,
            responseTimes);

        result.CalculatePercentiles(responseTimes);

        Assert.Equal(3655751, result.Percentile10ResponseTime);
        Assert.Equal(4388344, result.Percentile25ResponseTime);
        Assert.Equal(6184336, result.Percentile50ResponseTime);
        Assert.Equal(7207818, result.Percentile75ResponseTime);
        Assert.Equal(9191534, result.Percentile90ResponseTime);
        Assert.Equal(9209802, result.Percentile99ResponseTime);
        Assert.Equal(9209802, result.Percentile9999ResponseTime);
    }

    [Fact]
    public void CalculatePercentile_ReturnOK()
    {
        double[] responseTimes =
            [7207818, 5979207, 4279881, 9191534, 9209802, 6536028, 4388344, 3655751, 6319666, 6184336];

        var sortedResponseTimes = responseTimes.OrderBy(x => x).ToArray();

        Assert.Equal(3655751, StressTestHarnessResult.CalculatePercentile(sortedResponseTimes, 0.1));
        Assert.Equal(4388344, StressTestHarnessResult.CalculatePercentile(sortedResponseTimes, 0.25));
        Assert.Equal(6184336, StressTestHarnessResult.CalculatePercentile(sortedResponseTimes, 0.5));
        Assert.Equal(7207818, StressTestHarnessResult.CalculatePercentile(sortedResponseTimes, 0.75));
        Assert.Equal(9191534, StressTestHarnessResult.CalculatePercentile(sortedResponseTimes, 0.9));
        Assert.Equal(9209802, StressTestHarnessResult.CalculatePercentile(sortedResponseTimes, 0.99));
        Assert.Equal(9209802, StressTestHarnessResult.CalculatePercentile(sortedResponseTimes, 0.9999));
    }
}