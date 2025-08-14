using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Message.Application.Models;
using Message.Application.Services;
using Message.Domain.Entities;
using Message.Domain.Repositories;
using Shared.Mediator.Interface;

namespace Message.Application.Commands;

public record struct SendMessageCommand : IRequest<MessageModel?>
{
    /// <summary>
    /// 發送者的連接
    /// </summary>
    public required ConnectionModel Connection { get; set; }

    /// <summary>
    /// 訊息種類
    /// </summary>
    public required NoteType Type { get; set; }
    
    /// <summary>
    /// 內容
    /// </summary>
    public required string Content { get; set; }
    
}


public record struct SendTextMessageHandler : IRequestHandler<SendMessageCommand,MessageModel?>
{
    private readonly ISnowFlake _snowFlake;
    private readonly IConnection _connection;
    private readonly INoteRepository _noteRepository;

    public SendTextMessageHandler(
        ISnowFlake snowFlake, 
        IConnection connection, 
        INoteRepository noteRepository)
    {
        _snowFlake = snowFlake;
        _connection = connection;
        _noteRepository = noteRepository;
    }

    public async Task<MessageModel?> HandleAsync(SendMessageCommand request)
    {
        /**********************************************************************
         * variables -
         *      msg       : 訊息
         *      read      : 收訊者是否已讀訊息
         *      uri       : 聊天室 uri
         *      partnerId : 收訊者 ID
         *      id        : 訊息 ID
         **********************************************************************/
        MessageModel msg;
        bool read         = false;
        var connection    = request.Connection;
        var uri           = connection.Uri;
        var partnerId     = connection.PartnerId;
        var id            = _snowFlake.Get();

        /**********************************************************************
         * task -
         *      建立訊息
         **********************************************************************/
        if (uri == null) 
            return null;
        else if (partnerId == null) 
            return null;
        else
        {
            msg = new MessageModel
            {
                Id = id,
                Uri = connection.Uri ?? "",
                ReceiverId = connection.PartnerId?? 0,
                Content = request.Content,
                Type = request.Type,
                Status = 0
            };
        }

        /**********************************************************************
         * task -
         *      將訊息傳給聊對象
         **********************************************************************/
        if (_connection.Users.TryGetValue(partnerId.Value, out var partnerConnection))
        {
            // 取得收訊者 Web Socket
            var socket = partnerConnection.WebSocket;
            
            // 如果收訊者有在聊天室當中，傳訊後，應該是已讀狀態
            if (partnerConnection.Uri == uri)
                read = true;
            
            // 對訊息做編碼
            var content = JsonSerializer.Serialize(msg, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            var body    = Encoding.UTF8.GetBytes(content);

            /**********************************************************************
             * sub task -
             *      傳送訊息
             **********************************************************************/
            // 向系統申請一塊記憶體空間
            var buffer  = ArrayPool<byte>.Shared.Rent(body.Length);
            try
            {
                // 將訊息放入剛剛租用的記憶體空間內
                body.CopyTo(buffer, 0);
                var segment = new ArraySegment<byte>(buffer, 0, body.Length);

                // 傳送
                if (partnerConnection.WebSocket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(
                        segment,
                        WebSocketMessageType.Text,
                        endOfMessage: true,
                        CancellationToken.None);
                }
            }
            catch (Exception)
            {
                // 如果傳送失敗，表示訊息沒有已讀...
                read = false;
            }
            finally
            {
                // 向系統歸還一塊記憶體空間
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
        
        /**********************************************************************
         * task -
         *      將訊息存入資料庫
         **********************************************************************/
        _noteRepository.Add(new Note
        {
            Id = id,
            ReceiverId = partnerId.Value,
            Type = NoteType.text,
            Content = request.Content,
            CreatedAt = DateTimeOffset.Now,
            ReadAt = read ? DateTimeOffset.Now : null,
            BuyerId = connection.buyerId!.Value,
            ItemId = connection.itemId!.Value
        });

        return msg;
    }
}

