using System.Security.Claims;
using System.Security.Cryptography;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Webscrapper.API;
using Webscrapper.Database;
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

    public TokenCreator(KeyStore keyStore)
    {
        _key = RSA.Create();
        _key.ImportRSAPrivateKey(keyStore.RSAKey, out _);
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
        string Token = Guid.NewGuid().ToString() + Guid.NewGuid();

        return BCrypt.Net.BCrypt.EnhancedHashPassword(Token, HashType.SHA512);
    }
}