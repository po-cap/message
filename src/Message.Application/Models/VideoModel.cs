using Po.Media;

namespace Message.Application.Models;

public class VideoModel
{
    /// <summary>
    /// 影片
    /// </summary>
    public Media Video { get; set; }

    /// <summary>
    /// 封面照
    /// </summary>
    public Media Thumbnail { get; set; }
}