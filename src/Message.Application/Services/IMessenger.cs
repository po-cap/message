using System.Net.WebSockets;

namespace Message.Application.Services;

public interface IMessenger
{
    Task RunAsync(WebSocket socket, long userId);
    
    ///// <summary>
    ///// 使用者 - 長連接處理邏輯
    ///// </summary>
    ///// <param name="socket">使用者的長鏈結</param>
    ///// <param name="buyerId"></param>
    ///// <param name="itemId"></param>
    ///// <param name="userId">使用者 ID</param>
    ///// <returns></returns>
    //Task RunAsync(WebSocket socket, long userId, long buyerId, long itemId);
}