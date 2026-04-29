// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     压力测试管理器
/// </summary>
internal sealed class StressTestHarnessManager
{
    /// <inheritdoc cref="IHttpRemoteService" />
    internal readonly IHttpRemoteService _httpRemoteService;

    /// <inheritdoc cref="HttpStressTestHarnessBuilder" />
    internal readonly HttpStressTestHarnessBuilder _httpStressTestHarnessBuilder;

    /// <summary>
    ///     <inheritdoc cref="StressTestHarnessManager" />
    /// </summary>
    /// <param name="httpRemoteService">
    ///     <see cref="IHttpRemoteService" />
    /// </param>
    /// <param name="httpStressTestHarnessBuilder">
    ///     <see cref="HttpStressTestHarnessBuilder" />
    /// </param>
    internal StressTestHarnessManager(IHttpRemoteService httpRemoteService,
        HttpStressTestHarnessBuilder httpStressTestHarnessBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRemoteService);
        ArgumentNullException.ThrowIfNull(httpStressTestHarnessBuilder);

        _httpRemoteService = httpRemoteService;
        _httpStressTestHarnessBuilder = httpStressTestHarnessBuilder;

        // 构建 HttpRequestBuilder 实例
        RequestBuilder = httpStressTestHarnessBuilder.Build(httpRemoteService.ServiceProvider
            .GetRequiredService<IOptions<HttpRemoteOptions>>().Value);
    }

    /// <summary>
    ///     <inheritdoc cref="HttpRequestBuilder" />
    /// </summary>
    internal HttpRequestBuilder RequestBuilder { get; }

    /// <summary>
    ///     开始测试
    /// </summary>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="StressTestHarnessResult" />
    /// </returns>
    internal StressTestHarnessResult Start(
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default) =>
        AsyncUtility.RunSync(() => StartAsync(completionOption, cancellationToken));

    /// <summary>
    ///     开始测试
    /// </summary>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="StressTestHarnessResult" />
    /// </returns>
    internal async Task<StressTestHarnessResult> StartAsync(
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        // 初始化压力测试次数和轮次
        var numberOfRequests = _httpStressTestHarnessBuilder.NumberOfRequests;
        var numberOfRounds = _httpStressTestHarnessBuilder.NumberOfRounds;

        // 初始化总的成功/失败的请求数量
        var totalSuccessfulRequests = 0L;
        var totalFailedRequests = 0L;

        // 用于记录每个请求的响应时间
        var allResponseTimes = new List<double>(numberOfRequests * numberOfRounds);

        // 初始化总的测试时间
        var totalTime = TimeSpan.Zero;

        // 初始化信号量来控制并发度
        using var semaphoreSlim = new SemaphoreSlim(_httpStressTestHarnessBuilder.MaxDegreeOfParallelism);

        // 初始化 Stopwatch 实例并开启计时操作
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // 循环执行指定轮次
            for (var round = 0; round < numberOfRounds; round++)
            {
                // 如果请求了取消，则抛出 OperationCanceledException 
                cancellationToken.ThrowIfCancellationRequested();

                // 初始化 Task 数组来存储所有并发任务
                var tasks = new Task[numberOfRequests];

                // 初始化响应时间数组
                var responseTimes = new double[numberOfRequests];

                // 重新开始计时
                stopwatch.Restart();

                // 循环创建指定并发请求数量的任务
                for (var i = 0; i < numberOfRequests; i++)
                {
                    tasks[i] = ExecuteRequestAsync(i, semaphoreSlim, completionOption, responseTimes,
                        cancellationToken);
                }

                // 等待所有任务完成
                await Task.WhenAll(tasks).ConfigureAwait(false);

                // 记录本轮测试结束时间，并累加总的测试时间
                totalTime += stopwatch.Elapsed;

                // 将本轮的响应时间追加到总的响应时间数组中
                allResponseTimes.AddRange(responseTimes);
            }
        }
        finally
        {
            // 停止计时
            stopwatch.Stop();

            // 释放资源集合
            RequestBuilder.ReleaseResources();
        }

        // 获取请求总用时（秒）
        var totalTimeInSeconds = totalTime.TotalSeconds;

        return new StressTestHarnessResult(
            numberOfRequests * numberOfRounds,
            totalTimeInSeconds,
            totalSuccessfulRequests,
            totalFailedRequests,
            allResponseTimes.ToArray());

        // 封装单次请求异步方法
        async Task ExecuteRequestAsync(int idx, SemaphoreSlim semaphore, HttpCompletionOption option, double[] times,
            CancellationToken ct)
        {
            // 等待信号量
            await semaphore.WaitAsync(ct).ConfigureAwait(false);

            // 请求开始时间
            var requestStart = Stopwatch.GetTimestamp();

            try
            {
                // 发送 HTTP 远程请求
                var httpResponseMessage =
                    await _httpRemoteService.SendAsync(RequestBuilder, option, ct).ConfigureAwait(false);

                // 空检查
                if (httpResponseMessage is null)
                {
                    // 输出调试信息
                    Debugging.Error(Constants.HTTP_RESPONSE_MESSAGE_ISNULL_MESSAGE);

                    // 原子递增失败请求计数
                    Interlocked.Increment(ref totalFailedRequests);
                    return;
                }

                // 检查响应状态码是否是成功状态
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    // 原子递增成功请求计数
                    Interlocked.Increment(ref totalSuccessfulRequests);
                }
                else
                {
                    // 原子递增失败请求计数
                    Interlocked.Increment(ref totalFailedRequests);
                }
            }
            // 任务被取消
            catch (Exception e) when (ct.IsCancellationRequested || e is OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                // 原子递增失败请求计数
                Interlocked.Increment(ref totalFailedRequests);
            }
            finally
            {
                // 计算并存储请求的响应时间
                var requestEnd = Stopwatch.GetTimestamp();
                times[idx] = (requestEnd - requestStart) * 1000.0 / Stopwatch.Frequency;

                // 释放信号量
                semaphore.Release();
            }
        }
    }
}