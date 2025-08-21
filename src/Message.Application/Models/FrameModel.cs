using System.Text.Json.Serialization;
using Message.Domain.Entities;

namespace Message.Application.Models;

/// <summary>
/// 讀取到的 message
/// </summary>
public class FrameModel
{
    /// <summary>
    /// 訊息內容
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }
    
    /// <summary>
    /// 訊息類型
    /// </summary>
    [JsonPropertyName("type")]
    public NoteType Type { get; set; }

    /// <summary>
    /// 標籤，主要是用來讓用戶查看訊息是否成空傳送出去用的
    /// </summary>
    [JsonPropertyName("tag")]
    public string Tag { get; set; }
}

