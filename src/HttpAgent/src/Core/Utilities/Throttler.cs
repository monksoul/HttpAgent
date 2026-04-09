// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Utilities;

/// <summary>
///     多线程安全的节流器
/// </summary>
/// <remarks>控制操作执行的最小时间间隔。</remarks>
public sealed class Throttler
{
    /// <summary>
    ///     节流间隔
    /// </summary>
    /// <remarks>单位为毫秒。</remarks>
    internal long _intervalMs;

    /// <summary>
    ///     记录上一次允许执行的时间戳
    /// </summary>
    /// <remarks>单位为毫秒。</remarks>
    internal long _lastTick;

    /// <summary>
    ///     <inheritdoc cref="Throttler" />
    /// </summary>
    /// <param name="interval">节流间隔</param>
    public Throttler(TimeSpan interval) => _intervalMs = (long)interval.TotalMilliseconds;

    /// <summary>
    ///     当前节流间隔
    /// </summary>
    public TimeSpan Interval
    {
        get => TimeSpan.FromMilliseconds(Volatile.Read(ref _intervalMs));
        set => Volatile.Write(ref _intervalMs, (long)value.TotalMilliseconds);
    }

    /// <summary>
    ///     判断当前是否允许执行操作
    /// </summary>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public bool TryEnter()
    {
        // 获取当前配置的间隔时间（毫秒）
        var intervalMs = Volatile.Read(ref _intervalMs);

        // 检查是否不限制频率
        if (intervalMs <= 0)
        {
            return true;
        }

        // 获取当前系统运行时间戳
        var now = Environment.TickCount64;

        // 获取最近一次允许执行的时间戳
        // Volatile.Read 确保读取到的是其他线程写入的最新值
        var last = Volatile.Read(ref _lastTick);

        // 检查是否是首次调用或间隔时间已到
        // ReSharper disable once InvertIf
        if (last == 0 || now - last >= intervalMs)
        {
            // CAS 原子操作，更新最近一次允许执行的时间戳
            // 返回值 == last 表示当前线程成功抢占，获得本次执行权限
            if (Interlocked.CompareExchange(ref _lastTick, now, last) == last)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     重置节流状态
    /// </summary>
    /// <remarks>下次调用立即允许执行。</remarks>
    public void Reset() => Interlocked.Exchange(ref _lastTick, 0);

    /// <summary>
    ///     获取距离下次允许执行还需等待的毫秒数
    /// </summary>
    /// <returns>
    ///     <see cref="long" />
    /// </returns>
    public long GetRemainingMilliseconds()
    {
        // 获取当前配置的间隔时间（毫秒）
        var intervalMs = Volatile.Read(ref _intervalMs);

        // 检查是否不限制频率
        if (intervalMs <= 0)
        {
            return 0;
        }

        // 获取当前系统运行时间戳
        var now = Environment.TickCount64;

        // 获取最近一次允许执行的时间戳
        // Volatile.Read 确保读取到的是其他线程写入的最新值
        var last = Volatile.Read(ref _lastTick);

        // 如果是首次调用，立即可执行
        if (last == 0)
        {
            return 0;
        }

        // 计算已过去的时间
        var elapsed = now - last;

        // 如果已到间隔，返回 0；否则返回剩余等待时间
        return elapsed >= intervalMs ? 0 : intervalMs - elapsed;
    }
}