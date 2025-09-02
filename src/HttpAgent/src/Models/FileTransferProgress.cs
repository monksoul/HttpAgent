// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     文件传输的进度信息
/// </summary>
public sealed class FileTransferProgress
{
    /// <summary>
    ///     使用一个小的正值来防止除零错误
    /// </summary>
    internal const double _epsilon = double.Epsilon;

    /// <summary>
    ///     标记是否已打印文件头
    /// </summary>
    /// <remarks>仅适用于 <see cref="UpdateConsoleProgress" /> 方法。</remarks>
    internal bool _hasPrintedHeader;

    /// <summary>
    ///     <inheritdoc cref="FileTransferProgress" />
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="totalFileSize">文件的总大小</param>
    /// <param name="fileName">文件的名称</param>
    internal FileTransferProgress(string filePath, long totalFileSize, string? fileName = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        TotalFileSize = totalFileSize;

        FilePath = filePath;
        FileName = fileName ?? Path.GetFileName(filePath);
    }

    /// <summary>
    ///     文件路径
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    ///     文件的名称
    /// </summary>
    public string FileName { get; }

    /// <summary>
    ///     文件的总大小
    /// </summary>
    /// <remarks>以字节为单位。</remarks>
    public long TotalFileSize { get; }

    /// <summary>
    ///     已传输的数据量
    /// </summary>
    /// <remarks>以字节为单位。</remarks>
    public long Transferred { get; private set; }

    /// <summary>
    ///     已完成的传输百分比
    /// </summary>
    public double PercentageComplete { get; private set; }

    /// <summary>
    ///     当前的传输速率
    /// </summary>
    /// <remarks>以字节/秒为单位。</remarks>
    public double TransferRate { get; private set; }

    /// <summary>
    ///     从开始传输到现在的持续时间
    /// </summary>
    public TimeSpan TimeElapsed { get; private set; }

    /// <summary>
    ///     预估剩余传输时间
    /// </summary>
    public TimeSpan EstimatedTimeRemaining { get; private set; }

    /// <inheritdoc />
    public override string ToString() =>
        StringUtility.FormatKeyValuesSummary([
            new KeyValuePair<string, IEnumerable<string>>("File Name", [FileName]),
            new KeyValuePair<string, IEnumerable<string>>("File Path", [FilePath]),
            new KeyValuePair<string, IEnumerable<string>>("Total File Size",
                [$"{TotalFileSize.ToSizeUnits("MB"):F2}MB"]),
            new KeyValuePair<string, IEnumerable<string>>("Transferred", [$"{Transferred.ToSizeUnits("MB"):F2}MB"]),
            new KeyValuePair<string, IEnumerable<string>>("Percentage Complete", [$"{PercentageComplete:F2}%"]),
            new KeyValuePair<string, IEnumerable<string>>("Transfer Rate",
                [$"{TransferRate.ToSizeUnits("MB"):F2}MB/s"]),
            new KeyValuePair<string, IEnumerable<string>>("Time Elapsed (s)", [$"{TimeElapsed.TotalSeconds:F2}"]),
            new KeyValuePair<string, IEnumerable<string>>("Estimated Time Remaining (s)",
                [$"{EstimatedTimeRemaining.TotalSeconds:F2}"])
        ], "Transfer Progress")!;

    /// <summary>
    ///     输出简要进度字符串
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public string ToSummaryString() =>
        $"Transferred {Transferred.ToSizeUnits("MB"):F2}MB of {TotalFileSize.ToSizeUnits("MB"):F2}MB ({PercentageComplete:F2}% complete, Speed: {TransferRate.ToSizeUnits("MB"):F2}MB/s, Time: {TimeElapsed.TotalSeconds:F2}s, ETA: {EstimatedTimeRemaining.TotalSeconds:F2}s), File: {FileName}, Path: {FilePath}.";

    /// <summary>
    ///     在控制台中更新（打印）文件传输进度条
    /// </summary>
    public void UpdateConsoleProgress()
    {
        // 获取控制台宽度
        int windowWidth;
        try
        {
            windowWidth = Console.WindowWidth;
        }
        catch
        {
            windowWidth = 80;
        }

        // 计算自适应进度条宽度，10-30 字符最合适
        var barWidth = (int)Math.Clamp(windowWidth * 0.3, 10, 30);

        // 检查是否已打印文件头
        if (!_hasPrintedHeader)
        {
            Console.WriteLine($"File: {FileName}, Path: {FilePath}");
            _hasPrintedHeader = true;
        }

        // 计算进度条
        var progress = (int)Math.Clamp(PercentageComplete, 0, 100);
        var filledLength = (int)(progress / 100.0 * barWidth);
        var progressBar = new string('#', filledLength) + new string('.', barWidth - filledLength);

        // 生成进度文本
        var progressText =
            $"[{progressBar}] {PercentageComplete:F2}% ({Transferred.ToSizeUnits("MB"):F2}MB/{TotalFileSize.ToSizeUnits("MB"):F2}MB) Speed: {TransferRate.ToSizeUnits("MB"):F2}MB/s, Time: {TimeElapsed.TotalMilliseconds.FormatDuration()}, ETA: {EstimatedTimeRemaining.TotalMilliseconds.FormatDuration()}.";

        // 处理窗口过小问题（截断处理）
        if (progressText.Length >= windowWidth)
        {
            progressText = progressText[..(windowWidth - 4)] + "...";
        }

        // 清除当前行并写入新进度（不换行）
        try
        {
            Console.CursorLeft = 0;
            Console.Write(progressText.PadRight(windowWidth - 1));
            Console.CursorLeft = 0;
        }
        // 控制台不可用
        catch (IOException) { }
        // 某些平台不支持
        catch (PlatformNotSupportedException) { }
    }

    /// <summary>
    ///     更新文件传输进度
    /// </summary>
    /// <param name="transferred">已传输的数据量</param>
    /// <param name="timeElapsed">从开始传输到现在的持续时间</param>
    internal void UpdateProgress(long transferred, TimeSpan timeElapsed)
    {
        // 计算已完成的传输百分比和当前的传输速率
        var percentageComplete = TotalFileSize > 0 ? 100.0 * transferred / TotalFileSize : -1;
        var transferRate = timeElapsed.TotalSeconds > _epsilon ? transferred / timeElapsed.TotalSeconds : 0;

        // 更新内部进度状态
        Transferred = transferred;
        TimeElapsed = timeElapsed;
        PercentageComplete = percentageComplete;
        TransferRate = transferRate;

        // 计算预估剩余传输时间
        EstimatedTimeRemaining = CalculateEstimatedTimeRemaining();
    }

    /// <summary>
    ///     计算预估剩余传输时间
    /// </summary>
    /// <returns>
    ///     <see cref="TimeSpan" />
    /// </returns>
    internal TimeSpan CalculateEstimatedTimeRemaining()
    {
        // 如果文件大小小于等于 0 或传输速率为 0 或接近 0，则认为无法预估
        if (TotalFileSize <= 0 || TransferRate <= _epsilon)
        {
            return TimeSpan.MaxValue;
        }

        // 计算剩余时间
        var secondsRemaining = (TotalFileSize - Transferred) / TransferRate;

        // 如果剩余时间超过最大值，则返回最大值
        return secondsRemaining > TimeSpan.MaxValue.TotalSeconds
            ? TimeSpan.MaxValue
            : TimeSpan.FromSeconds(secondsRemaining);
    }
}