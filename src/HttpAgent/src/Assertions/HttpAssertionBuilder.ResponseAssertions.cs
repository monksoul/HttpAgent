// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求响应断言构建器
/// </summary>
public sealed partial class HttpAssertionBuilder
{
    /// <summary>
    ///     断言响应状态码等于指定的 <see cref="HttpStatusCode" /> 值
    /// </summary>
    /// <param name="expected">期望值</param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    public HttpAssertionBuilder ResponseStatusCode(HttpStatusCode expected) => ResponseStatusCode((int)expected);

    /// <summary>
    ///     断言响应状态码等于指定的整数值
    /// </summary>
    /// <param name="expected">期望值</param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    /// <exception cref="HttpAssertionException"></exception>
    public HttpAssertionBuilder ResponseStatusCode(int expected)
    {
        _responseAssertions.Add(async context =>
        {
            // 获取 HTTP 状态码
            var actual = (int)context.StatusCode;

            if (actual != expected)
            {
                await HttpAssertionException.ThrowAsync(
                    $"Expected response status code to be {expected}, but found {actual}.");
            }
        });

        return this;
    }

    /// <summary>
    ///     断言响应状态码在指定的允许状态码列表中
    /// </summary>
    /// <param name="allowedStatusCodes">允许的状态码列表</param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="HttpAssertionException"></exception>
    public HttpAssertionBuilder ResponseStatusCodeIn(params int[] allowedStatusCodes)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(allowedStatusCodes);

        // 空数组检查
        if (allowedStatusCodes is { Length: 0 })
        {
            throw new ArgumentException("The allowed status codes array cannot be null or empty.",
                nameof(allowedStatusCodes));
        }

        _responseAssertions.Add(async context =>
        {
            // 获取 HTTP 状态码
            var actual = (int)context.StatusCode;

            if (!allowedStatusCodes.Contains(actual))
            {
                await HttpAssertionException.ThrowAsync(
                    $"Expected response status code to be one of [{string.Join(", ", allowedStatusCodes)}], but found {actual}.");
            }
        });

        return this;
    }

    /// <summary>
    ///     断言请求成功（即响应状态码为 2xx 范围）
    /// </summary>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    /// <exception cref="HttpAssertionException"></exception>
    public HttpAssertionBuilder ResponseIsSuccessStatusCode()
    {
        _responseAssertions.Add(async context =>
        {
            if (!context.IsSuccessStatusCode)
            {
                await HttpAssertionException.ThrowAsync(
                    $"Expected response to be successful (2xx status code), but found status code {(int)context.StatusCode}.");
            }
        });

        return this;
    }

    /// <summary>
    ///     断言响应内容包含指定的子字符串（不区分大小写）
    /// </summary>
    /// <param name="expectedSubstring">期望包含的子字符串</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="HttpAssertionException"></exception>
    public HttpAssertionBuilder ResponseContentContains(string expectedSubstring,
        CancellationToken cancellationToken = default)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrEmpty(expectedSubstring);

        _responseAssertions.Add(async context =>
        {
            // 读取响应内容字符串
            var content = await context.ReadResponseAsStringAsync(cancellationToken);

            if (string.IsNullOrEmpty(content) ||
                !content.Contains(expectedSubstring, StringComparison.OrdinalIgnoreCase))
            {
                await HttpAssertionException.ThrowAsync(
                    $"Expected response content to contain '{expectedSubstring}', but it was not found.");
            }
        });

        return this;
    }

    /// <summary>
    ///     断言响应内容完全等于指定的字符串
    /// </summary>
    /// <param name="expected">期望完全相等的字符串</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public HttpAssertionBuilder ResponseContentEquals(string expected, CancellationToken cancellationToken = default)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrEmpty(expected);

        _responseAssertions.Add(async context =>
        {
            // 读取响应内容字符串
            var content = await context.ReadResponseAsStringAsync(cancellationToken);

            if (!string.Equals(content, expected, StringComparison.Ordinal))
            {
                await HttpAssertionException.ThrowAsync(
                    $"Expected response content to be '{expected}', but found '{content}'.");
            }
        });

        return this;
    }

    /// <summary>
    ///     断言响应内容与指定的正则表达式匹配
    /// </summary>
    /// <param name="pattern">正则表达式</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public HttpAssertionBuilder ResponseContentMatches(string pattern, CancellationToken cancellationToken = default)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);

        _responseAssertions.Add(async context =>
        {
            // 读取响应内容字符串
            var content = await context.ReadResponseAsStringAsync(cancellationToken);

            if (content is null || !Regex.IsMatch(content, pattern))
            {
                await HttpAssertionException.ThrowAsync(
                    $"Expected response content to match regex '{pattern}', but it did not.");
            }
        });

        return this;
    }

    /// <summary>
    ///     断言响应内容不为空
    /// </summary>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    public HttpAssertionBuilder ResponseContentNotEmpty(CancellationToken cancellationToken = default)
    {
        _responseAssertions.Add(async context =>
        {
            // 读取响应内容字符串
            var content = await context.ReadResponseAsStringAsync(cancellationToken);

            if (string.IsNullOrEmpty(content))
            {
                await HttpAssertionException.ThrowAsync("Expected response content not to be empty.");
            }
        });

        return this;
    }

    /// <summary>
    ///     断言指定的响应标头存在（可在响应标头或内容标头中）
    /// </summary>
    /// <param name="name">标头名</param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="HttpAssertionException"></exception>
    public HttpAssertionBuilder ResponseHeaderExists(string name)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        _responseAssertions.Add(async context =>
        {
            // 尝试从响应标头或内容标头中检查
            var exists = context.ResponseMessage?.Headers.TryGetValues(name, out _) == true ||
                         context.ResponseMessage?.Content?.Headers.TryGetValues(name, out _) == true;

            if (!exists)
            {
                await HttpAssertionException.ThrowAsync(
                    $"Expected response header '{name}' to exist, but it was not found.");
            }
        });

        return this;
    }

    /// <summary>
    ///     断言响应标头的第一个值严格等于指定字符串（区分大小写）
    /// </summary>
    /// <param name="name">标头名</param>
    /// <param name="expectedValue">期望值</param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="HttpAssertionException"></exception>
    public HttpAssertionBuilder ResponseHeaderEquals(string name, string expectedValue)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(expectedValue);

        _responseAssertions.Add(async context =>
        {
            string? actualValue = null;

            // 尝试从响应标头中获取值
            if (context.ResponseMessage?.Headers.TryGetValues(name, out var headerValues) == true)
            {
                actualValue = headerValues.FirstOrDefault();
            }
            // 尝试从响应内容标头中获取值
            else if (context.ResponseMessage?.Content?.Headers.TryGetValues(name, out var contentHeaderValues) == true)
            {
                actualValue = contentHeaderValues.FirstOrDefault();
            }

            // 检查是否存在匹配项（区分大小写）
            if (actualValue != expectedValue)
            {
                await HttpAssertionException.ThrowAsync(
                    $"Expected response header '{name}' to be '{expectedValue}', but found '{actualValue}'.");
            }
        });

        return this;
    }

    /// <summary>
    ///     断言响应标头的任意一个值包含指定的子字符串（不区分大小写）
    /// </summary>
    /// <param name="name">标头名</param>
    /// <param name="expectedValue">期望值</param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="HttpAssertionException"></exception>
    public HttpAssertionBuilder ResponseHeaderContains(string name, string expectedValue)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(expectedValue);

        _responseAssertions.Add(async context =>
        {
            string[]? values = null;

            // 尝试从响应标头中获取值
            if (context.ResponseMessage?.Headers.TryGetValues(name, out var headerValues) == true)
            {
                values = headerValues.ToArray();
            }
            // 尝试从响应内容标头中获取值
            else if (context.ResponseMessage?.Content?.Headers.TryGetValues(name, out var contentHeaderValues) == true)
            {
                values = contentHeaderValues.ToArray();
            }

            // 空检查
            if (values is null || values.Length == 0)
            {
                await HttpAssertionException.ThrowAsync(
                    $"Expected response header '{name}' to contain '{expectedValue}', but the header was not found.");
            }

            // 检查是否存在匹配项（不区分大小写）
            if (!values.Any(value => value.Contains(expectedValue, StringComparison.OrdinalIgnoreCase)))
            {
                await HttpAssertionException.ThrowAsync(
                    $"Expected response header '{name}' to contain '{expectedValue}', but actual values were: [{string.Join(", ", values)}].");
            }
        });

        return this;
    }

    /// <summary>
    ///     断言指定的响应标头不存在（可在响应标头或内容标头中）
    /// </summary>
    /// <param name="name">标头名</param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="HttpAssertionException"></exception>
    public HttpAssertionBuilder ResponseHeaderNotExists(string name)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        _responseAssertions.Add(async context =>
        {
            // 尝试从响应标头或内容标头中检查
            var exists = context.ResponseMessage?.Headers.TryGetValues(name, out _) == true ||
                         context.ResponseMessage?.Content?.Headers.TryGetValues(name, out _) == true;

            if (exists)
            {
                await HttpAssertionException.ThrowAsync(
                    $"Expected response header '{name}' not to exist, but it was found.");
            }
        });

        return this;
    }

    /// <summary>
    ///     断言请求耗时低于指定的毫秒数
    /// </summary>
    /// <param name="maxMilliseconds">最大允许耗时（毫秒）</param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public HttpAssertionBuilder ResponseDurationUnder(double maxMilliseconds) =>
        // 小于或等于 0 检查
        maxMilliseconds <= 0
            ? throw new ArgumentException("Max milliseconds must be greater than 0.", nameof(maxMilliseconds))
            : ResponseDurationUnder(TimeSpan.FromMilliseconds(maxMilliseconds));

    /// <summary>
    ///     断言请求耗时低于指定的时间跨度
    /// </summary>
    /// <param name="maxDuration">最大允许耗时</param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="HttpAssertionException"></exception>
    public HttpAssertionBuilder ResponseDurationUnder(TimeSpan maxDuration)
    {
        // 小于或等于 0 检查
        if (maxDuration <= TimeSpan.Zero)
        {
            throw new ArgumentException("Max duration must be greater than 0.", nameof(maxDuration));
        }

        _responseAssertions.Add(async context =>
        {
            // 获取实际耗时
            var actualDuration = TimeSpan.FromMilliseconds(context.RequestDuration);

            if (actualDuration > maxDuration)
            {
                await HttpAssertionException.ThrowAsync(
                    $"Expected response duration to be under {maxDuration.TotalMilliseconds:F2}ms, but it took {actualDuration.TotalMilliseconds:F2}ms.");
            }
        });

        return this;
    }

    /// <summary>
    ///     自定义响应消息断言（同步检查）
    /// </summary>
    /// <param name="assertion">断言委托，参数为 <see cref="HttpResponseMessage" /></param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpAssertionBuilder ResponseSatisfies(Action<HttpResponseMessage> assertion)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(assertion);

        _responseAssertions.Add(context =>
        {
            if (context.ResponseMessage is not null)
            {
                assertion(context.ResponseMessage);
            }

            return Task.CompletedTask;
        });

        return this;
    }

    /// <summary>
    ///     自定义响应消息断言（异步检查）
    /// </summary>
    /// <param name="assertion">异步断言委托</param>
    /// <returns>
    ///     <see cref="HttpAssertionBuilder" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpAssertionBuilder ResponseSatisfies(Func<HttpResponseMessage, Task> assertion)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(assertion);

        _responseAssertions.Add(async context =>
        {
            if (context.ResponseMessage is not null)
            {
                await assertion(context.ResponseMessage);
            }
        });

        return this;
    }
}