using System.Net.WebSockets;

namespace Message.Application.Services;

public interface IMessenger
{
    /// <summary>
    /// 使用者 - 長連接處理邏輯
    /// </summary>
    /// <param name="socket">連連接</param>
    /// <param name="token">Json Web Token</param>
    /// <returns></returns>
    Task RunAsync(WebSocket socket, string token);
}