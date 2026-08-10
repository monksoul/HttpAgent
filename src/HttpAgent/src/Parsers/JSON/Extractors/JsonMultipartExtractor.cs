// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     JSON 多部分表单提取器
/// </summary>
/// <remarks>
///     <para>支持 cURL 风格的文件上传语法：</para>
///     <list type="bullet">
///         <item>
///             <description>
///                 <c>"file": "@filepath"</c>
///             </description>
///         </item>
///         <item>
///             <description>
///                 <c>"file": "@filepath;type=mime/type"</c>
///             </description>
///         </item>
///         <item>
///             <description>
///                 <c>"file": "@filepath;filename=custom.txt"</c>
///             </description>
///         </item>
///     </list>
///     <para>同时支持值为 JSON 数组，例如 <c>"files": ["@file1", "@file2"]</c>，将按相同字段名添加多个文件。</para>
/// </remarks>
internal sealed class JsonMultipartExtractor : HttpJsonExtractorBase
{
    /// <inheritdoc />
    protected override string PropertyName => "multipart";

    /// <inheritdoc />
    protected override void Extract(HttpRequestBuilder httpRequestBuilder, JsonNode node,
        HttpJsonParsingContext context)
    {
        // 检查是否是 JSON 对象
        if (node is not JsonObject multipartObj)
        {
            return;
        }

        // 设置多部分表单内容
        httpRequestBuilder.SetMultipartContent(multipart =>
        {
            // 遍历所有表单项
            foreach (var (name, valueNode) in multipartObj)
            {
                ProcessMultipartItem(multipart, name, valueNode);
            }
        });
    }

    /// <summary>
    ///     处理单个多部分表单项
    /// </summary>
    /// <remarks>
    ///     支持字符串（包含 <c>@</c> 文件语法）、数字、布尔值、对象等类型。
    ///     当值为 <see cref="JsonArray" /> 时，会递归处理数组中的每个元素。
    /// </remarks>
    /// <param name="multipart">
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </param>
    /// <param name="name">表单名称</param>
    /// <param name="valueNode">
    ///     <see cref="JsonNode" /> 值节点
    /// </param>
    internal static void ProcessMultipartItem(HttpMultipartFormDataBuilder multipart, string name, JsonNode? valueNode)
    {
        switch (valueNode)
        {
            // 空检查
            case null:
                return;
            // 处理字符串类型的值（支持 @ 文件语法）
            case JsonValue jsonValue when jsonValue.TryGetValue<string>(out var strValue):
                // 复用 cURL 表单提取器的处理逻辑
                CurlFormExtractor.ProcessFormItem(multipart, name, strValue);
                break;
            // 处理数组：递归处理每个元素，保留同一个表单字段名
            case JsonArray jsonArray:
                foreach (var item in jsonArray)
                {
                    ProcessMultipartItem(multipart, name, item);
                }

                break;
            // 处理数字、布尔值、对象等非字符串类型
            default:
                multipart.AddFormItem(valueNode.ToJsonString(), name);
                break;
        }
    }
}