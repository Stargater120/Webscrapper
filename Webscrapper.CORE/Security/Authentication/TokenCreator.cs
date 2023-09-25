using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Webscrapper.API;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE.Security.Authentication;

public interface ITokenCreator
{
    string CreateToken(User user);
    string CreateRefreshToken();
}

public class TokenCreator : ITokenCreator
{
    private readonly RSA _key;

    public TokenCreator(IServiceProvider provider)
    {
        var appSettings = provider.GetRequiredService<WebscrapperAppSettings>();
        _key = RSA.Create();
        _key.ImportRSAPrivateKey(appSettings.RSAKey, out _);
    }

    public string CreateToken(User user)
    {
        var key = new RsaSecurityKey(_key);
        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(new SecurityTokenDescriptor()
        {
            Subject = user.ClaimsIdentity,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha512),
            Expires = DateTime.Now.AddMinutes(15),
            IssuedAt = DateTime.Now
        });

        return token;
    }

    public string CreateRefreshToken()
    {


        return String.Empty;
    }
}