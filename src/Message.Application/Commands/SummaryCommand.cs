using Message.Application.Models;
using Message.Domain.Repositories;
using Shared.Mediator.Interface;

namespace Message.Application.Commands;

public class SummaryCommand : IRequest<IEnumerable<ConversationDto>>
{
    /// <summary>
    /// 統計誰的
    /// </summary>
    public long To { get; set; }
}

public class SummaryHandler : IRequestHandler<SummaryCommand,IEnumerable<ConversationDto>>
{
    private readonly INoteRepository _noteRepository;
    private readonly IUserRepository _userRepository;

    public SummaryHandler(INoteRepository noteRepository, IUserRepository userRepository)
    {
        _noteRepository = noteRepository;
        _userRepository = userRepository;
    }


    public Task<IEnumerable<ConversationDto>> HandleAsync(SummaryCommand request)
    {
        var notes = _noteRepository.Summary(request.To);
        
        var group = notes.GroupBy(x => x.SenderId);

        var ids = group.Select(x => x.Key).ToArray();
        
        var users = _userRepository.Get(ids);

        var conversations = group.Select((messages) =>
        {
            var user = users.First(x => x.Id == messages.Key);
            
            return new ConversationDto()
            {
                SenderId = messages.Key,
                UnReadCount = messages.Count(),
                LastMessage = messages.Last().ToDto(),
                SenderAvatar = user.Avatar,
                SenderName = user.DisplayName
            };
        });

        return Task.FromResult(conversations);
    }
}