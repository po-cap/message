using System.Net.WebSockets;
using Microsoft.Extensions.DependencyInjection;

namespace Message.Application.Services;

public interface IMessenger
{
    /// <summary>
    /// 使用者 - 建立長連接 (Web Socket)
    /// </summary>
    /// <param name="socket">連連接</param>
    /// <param name="userId">使用者ID</param>
    Task RunAsync(WebSocket socket, string userId);
}