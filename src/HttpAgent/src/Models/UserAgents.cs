// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     主流浏览器 <c>User-Agent</c> 字符串常量静态类
/// </summary>
public static class UserAgents
{
    /// <summary>
    ///     随机获取一个浏览器的 <c>User-Agent</c>
    /// </summary>
    /// <remarks>支持桌面端或移动端切换。</remarks>
    /// <param name="isMobile">是否为移动端，默认值为：<c>false</c>（即桌面端）</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string GetRandom(bool isMobile = false)
    {
        // 初始化桌面端和移动端 User-Agent 列表
        var pcPool = new[] { Chrome.PC, Firefox.PC, Safari.PC, Edge.PC, Opera.PC, Generic.PC };
        var mobilePool = new[]
        {
            Chrome.Mobile, Firefox.Mobile, Safari.Mobile, Edge.Mobile, Opera.Mobile, Generic.Mobile
        };

        var pool = !isMobile ? pcPool : mobilePool;

        // 返回随机浏览器的 User-Agent
        return pool[Random.Shared.Next(pool.Length)];
    }

    /// <summary>
    ///     获取指定浏览器类型的 <c>User-Agent</c>
    /// </summary>
    /// <remarks>支持桌面端或移动端切换。</remarks>
    /// <param name="browser">浏览器枚举（Chrome/Firefox/Safari/Edge/Opera/Generic）</param>
    /// <param name="isMobile">是否为移动端，默认值为：<c>false</c>（即桌面端）</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string GetByBrowser(string? browser, bool isMobile = false) =>
        browser?.ToUpperInvariant() switch
        {
            "CHROME" => isMobile ? Chrome.Mobile : Chrome.PC,
            "FIREFOX" => isMobile ? Firefox.Mobile : Firefox.PC,
            "SAFARI" => isMobile ? Safari.Mobile : Safari.PC,
            "EDGE" => isMobile ? Edge.Mobile : Edge.PC,
            "OPERA" => isMobile ? Opera.Mobile : Opera.PC,
            _ => isMobile ? Generic.Mobile : Generic.PC
        };

    /// <summary>
    ///     Google Chrome
    /// </summary>
    public static class Chrome
    {
        /// <summary>
        ///     桌面端
        /// </summary>
        public const string PC =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/145.0.0.0 Safari/537.36";

        /// <summary>
        ///     移动端
        /// </summary>
        public const string Mobile =
            "Mozilla/5.0 (Linux; Android 15; Pixel 9) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/145.0.0.0 Mobile Safari/537.36";
    }

    /// <summary>
    ///     Mozilla Firefox
    /// </summary>
    public static class Firefox
    {
        /// <summary>
        ///     桌面端
        /// </summary>
        public const string PC = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:139.0) Gecko/20100101 Firefox/139.0";

        /// <summary>
        ///     移动端
        /// </summary>
        public const string Mobile = "Mozilla/5.0 (Android 15; Mobile; rv:139.0) Gecko/139.0 Firefox/139.0";
    }

    /// <summary>
    ///     Apple Safari
    /// </summary>
    public static class Safari
    {
        /// <summary>
        ///     桌面端
        /// </summary>
        public const string PC =
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 15_4) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.4 Safari/605.1.15";

        /// <summary>
        ///     移动端
        /// </summary>
        public const string Mobile =
            "Mozilla/5.0 (iPhone; CPU iPhone OS 18_4 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.4 Mobile/15E148 Safari/604.1";
    }

    /// <summary>
    ///     Microsoft Edge
    /// </summary>
    public static class Edge
    {
        /// <summary>
        ///     桌面端
        /// </summary>
        public const string PC =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/145.0.0.0 Safari/537.36 Edg/145.0.0.0";

        /// <summary>
        ///     移动端
        /// </summary>
        public const string Mobile =
            "Mozilla/5.0 (Linux; Android 15; SM-S928B) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/145.0.0.0 Mobile Safari/537.36 EdgA/145.0.0.0";
    }

    /// <summary>
    ///     Opera
    /// </summary>
    public static class Opera
    {
        /// <summary>
        ///     桌面端
        /// </summary>
        public const string PC =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/145.0.0.0 Safari/537.36 OPR/110.0.0.0";

        /// <summary>
        ///     移动端
        /// </summary>
        public const string Mobile =
            "Mozilla/5.0 (Linux; Android 15; Pixel 9) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/145.0.0.0 Mobile Safari/537.36 OPR/85.0.0.0";
    }

    /// <summary>
    ///     通用
    /// </summary>
    public static class Generic
    {
        /// <summary>
        ///     桌面端
        /// </summary>
        public const string PC =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/145.0.0.0 Safari/537.36";

        /// <summary>
        ///     移动端
        /// </summary>
        public const string Mobile =
            "Mozilla/5.0 (Linux; Android 15; Mobile) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/145.0.0.0 Mobile Safari/537.36";
    }
}