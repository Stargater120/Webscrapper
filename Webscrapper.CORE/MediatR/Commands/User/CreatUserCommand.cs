using System.Security.Claims;
using MediatR;
using Webscrapper.Database;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE.MediatR.Commands;

public class CreatUserCommand : IRequest<Unit>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<Guid> Subscriptions { get; set; }
}

public class CreateUserCommandHandler : NeedsDBContext, IRequestHandler<CreatUserCommand, Unit>
{
    public CreateUserCommandHandler(DatabaseInitializer databaseInitializer) : base(databaseInitializer)
    {
    }

    public async Task<Unit> Handle(CreatUserCommand request, CancellationToken cancellationToken)
    {
        var newUser = new User()
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            SubscriptionIds = request.Subscriptions,
            ClaimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim("News", "true"),
                new Claim("Products", "true")
            })
        };
        await _dbContext.User.InsertOneAsync(newUser, null, cancellationToken);
        
        return Unit.Value;
    }
}