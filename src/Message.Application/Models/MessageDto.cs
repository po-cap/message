using System.Text.Json.Serialization;
using Message.Domain.Entities;

namespace Message.Application.Models;

public struct MessageDto
{
    /// <summary>
    /// 收訊者
    /// </summary>
    [JsonPropertyName("to")]
    public string To { get; set; }

    /// <summary>
    /// 發訊者
    /// </summary>
    [JsonPropertyName("from")]
    public string From { get; set; }

    /// <summary>
    /// 訊息內容
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }
    
    /// <summary>
    /// 訊息類型
    /// </summary>
    [JsonPropertyName("type")]
    public DataType Type { get; set; }
}