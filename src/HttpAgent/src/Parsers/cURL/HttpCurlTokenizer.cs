// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     cURL 命令词法分析器
/// </summary>
internal static class HttpCurlTokenizer
{
    /// <summary>
    ///     将 cURL 命令字符串拆分为 Token 集合
    /// </summary>
    /// <param name="curlCommand">cURL 命令字符串</param>
    /// <returns><see cref="string" /> 集合</returns>
    /// <exception cref="ArgumentException"></exception>
    internal static List<string> Tokenize(string curlCommand)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(curlCommand);

        var tokens = new List<string>();
        var currentToken = new StringBuilder();

        // 初始化单双引号标记
        var inSingleQuote = false;
        var inDoubleQuote = false;

        var i = 0;
        var length = curlCommand.Length;

        // 游标式解析
        while (i < length)
        {
            var ch = curlCommand[i];

            switch (ch)
            {
                // 处理转义字符
                case '\\' when i + 1 < length:
                    {
                        var nextCh = curlCommand[i + 1];

                        // 换行续行符（Linux/Mac）
                        if (nextCh is '\n' or '\r')
                        {
                            i++;
                            if (nextCh == '\r' && i + 1 < length && curlCommand[i + 1] == '\n')
                            {
                                i++;
                            }

                            i++;
                            continue;
                        }

                        if (inDoubleQuote && nextCh == '"')
                        {
                            currentToken.Append('"');
                            i += 2;
                            continue;
                        }

                        if (inSingleQuote && nextCh == '\'')
                        {
                            currentToken.Append('\'');
                            i += 2;
                            continue;
                        }

                        if (nextCh == '\\')
                        {
                            currentToken.Append('\\');
                            i += 2;
                            continue;
                        }

                        currentToken.Append(ch);
                        i++;
                        continue;
                    }
                // 处理 Windows CMD 换行续行符 ^
                case '^' when i + 1 < length && (curlCommand[i + 1] == '\n' || curlCommand[i + 1] == '\r'):
                    {
                        i++;
                        if (i < length && curlCommand[i] == '\n')
                        {
                            i++;
                        }

                        i++;
                        continue;
                    }
                // 处理引号切换
                case '\'' when !inDoubleQuote:
                    inSingleQuote = !inSingleQuote;
                    i++;
                    continue;
                case '"' when !inSingleQuote:
                    inDoubleQuote = !inDoubleQuote;
                    i++;
                    continue;
                // 处理空格分隔（仅在引号外）
                case ' ' or '\t' when !inSingleQuote && !inDoubleQuote:
                // 跳过换行符（引号外）
                case '\n' or '\r' when !inSingleQuote && !inDoubleQuote:
                    {
                        if (currentToken.Length > 0)
                        {
                            tokens.Add(currentToken.ToString());
                            currentToken.Clear();
                        }

                        i++;
                        continue;
                    }
                default:
                    // 普通字符
                    currentToken.Append(ch);
                    i++;
                    break;
            }
        }

        // 检查引号是否闭合
        if (inSingleQuote || inDoubleQuote)
        {
            throw new ArgumentException("Unterminated quote in cURL command.");
        }

        // 添加最后一个 Token
        if (currentToken.Length > 0)
        {
            tokens.Add(currentToken.ToString());
        }

        return tokens;
    }
}