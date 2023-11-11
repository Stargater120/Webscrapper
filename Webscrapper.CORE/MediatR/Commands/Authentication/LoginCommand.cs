using MediatR;
using MongoDB.Driver;
using Webscrapper.CORE.Payloads.Authentication;
using Webscrapper.Database;
using Webscrapper.Database.Models;
using BCrypt.Net;
using Webscrapper.CORE.Security.Authentication;

namespace Webscrapper.CORE.MediatR.Commands;

public class LoginCommand : IRequest<LoginPayload>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginCommandHandler : NeedsDBContext, IRequestHandler<LoginCommand, LoginPayload>
{
    private readonly ITokenCreator _tokenCreator;
    public LoginCommandHandler(DatabaseInitializer databaseInitializer, ITokenCreator tokenCreator) : base(databaseInitializer)
    {
        _tokenCreator = tokenCreator;
    }
    
    public async Task<LoginPayload> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var pwHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var filter = Builders<User>.Filter.Eq(x => x.Email, request.Email);
        
        var user = await _dbContext.User.Find(filter).FirstAsync(cancellationToken);
        var payload = new LoginPayload()
        {
            AuthenticationToken = _tokenCreator.CreateToken(user),
            RefreshToken = _tokenCreator.CreateRefreshToken(),
            UserId = user.Id
        };
        var update = Builders<User>.Update.Set(x => x.RefreshToken, payload.RefreshToken);
        await _dbContext.User.UpdateOneAsync(filter, update, null, cancellationToken);
        return payload;
    }
}