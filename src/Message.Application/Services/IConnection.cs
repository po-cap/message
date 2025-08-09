using System.Collections.Concurrent;
using Message.Application.Models;

namespace Message.Application.Services;

public interface IConnection
{
    ConcurrentDictionary<long, ConnectionModel> Users { get; }
}