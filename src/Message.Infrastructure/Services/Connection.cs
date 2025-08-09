using System.Collections.Concurrent;
using Message.Application.Models;
using Message.Application.Services;

namespace Message.Infrastructure.Services;

public class Connection : IConnection
{
    public Connection()
    {
        Users = [];
    }
    
    public ConcurrentDictionary<long, ConnectionModel> Users { get; }
}