// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     cURL 解析上下文
/// </summary>
public sealed class HttpCurlParsingContext
{
    /// <summary>
    ///     <inheritdoc cref="HttpCurlParsingContext" />
    /// </summary>
    /// <param name="tokens">Token 集合</param>
    /// <exception cref="ArgumentNullException"></exception>
    internal HttpCurlParsingContext(IReadOnlyList<string> tokens)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(tokens);

        Tokens = tokens;
        CurrentIndex = 0;
    }

    /// <summary>
    ///     Token 集合
    /// </summary>
    public IReadOnlyList<string> Tokens { get; }

    /// <summary>
    ///     当前索引位置
    /// </summary>
    public int CurrentIndex { get; private set; }

    /// <summary>
    ///     获取当前 Token
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public string CurrentToken => CurrentIndex < Tokens.Count
        ? Tokens[CurrentIndex]
        : throw new InvalidOperationException("Token index out of range.");

    /// <summary>
    ///     是否还有下一个 Token
    /// </summary>
    public bool HasNext => CurrentIndex < Tokens.Count - 1;

    /// <summary>
    ///     是否已到达末尾
    /// </summary>
    public bool IsEndOfTokens => CurrentIndex >= Tokens.Count;

    /// <summary>
    ///     预览下一个 Token（不移动指针）
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public string? PeekNext() => HasNext ? Tokens[CurrentIndex + 1] : null;

    /// <summary>
    ///     前进指定步数
    /// </summary>
    /// <param name="count">前进步数，默认值为 <c>1</c>。</param>
    public void Advance(int count = 1) => CurrentIndex += count;

    /// <summary>
    ///     重置指针到起始位置
    /// </summary>
    public void Reset() => CurrentIndex = 0;

    /// <summary>
    ///     检查当前 Token 是否匹配指定的任一值（不区分大小写）
    /// </summary>
    /// <param name="values">要匹配的值集合</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public bool CurrentTokenMatches(params string[] values) =>
        !IsEndOfTokens && values.Any(u => string.Equals(CurrentToken, u, StringComparison.OrdinalIgnoreCase));
}