using MediatR;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.UserSession;
using BloggingSystem.Domain.Entities;

namespace BloggingSystem.Application.Authentication.Commands
{
    public class LogoutCommand : IRequest<bool>
    {
        public long UserId { get; set; }
        public string RefreshToken { get; set; } = null!;
    }

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IRepository<UserSession> _sessionRepository;
        private readonly IDomainEventService _domainEventService;

        public LogoutCommandHandler(IRepository<UserSession> sessionRepository, IDomainEventService domainEventService)
        {
            _sessionRepository = sessionRepository;
            _domainEventService = domainEventService;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var sessionByTokenSpec = new UserSessionByTokenSpecification(request.RefreshToken);
            var session = await _sessionRepository.FirstOrDefaultAsync(sessionByTokenSpec, cancellationToken);
            
            if (session == null || session.UserId != request.UserId)
                return false;
            
            session.Delete();

            await _sessionRepository.DeleteAsync(session, cancellationToken);

            await _domainEventService.PublishEventsAsync(session.DomainEvents);
            
            return true;
        }
    }
}