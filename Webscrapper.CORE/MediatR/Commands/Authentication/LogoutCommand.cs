using MediatR;
using MongoDB.Driver;
using Webscrapper.Database;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE.MediatR.Commands;

public class LogoutCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
}

public class LogoutCommandHandler : NeedsDBContext, IRequestHandler<LogoutCommand, Unit>
{
    public LogoutCommandHandler(DatabaseInitializer databaseInitializer) : base(databaseInitializer)
    {
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Id, request.UserId);
        var update = Builders<User>.Update.Set(x => x.RefreshToken, String.Empty);
        await _dbContext.User.UpdateOneAsync(filter, update, null, cancellationToken);
        
        return Unit.Value;
    }
} 