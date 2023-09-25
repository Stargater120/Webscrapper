using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using NotificationService;
using NotificationService.Email;
using Webscrapper.API;
using Webscrapper.CORE;

var builder = WebApplication.CreateBuilder(args);
var configManager = builder.Configuration;
var config = new WebscrapperAppSettings();
configManager.Bind(config);
builder.Services.Configure<WebscrapperAppSettings>(configManager);
var rsaKey = RSA.Create();
var privateKey = rsaKey.ExportRSAPrivateKey();
config.RSAKey = privateKey;
// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("jwt").AddJwtBearer("jwt", o =>
{
    o.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = false,
        ValidateIssuer = false
    };

    o.Events = new JwtBearerEvents()
    {
        OnMessageReceived = (ctx) =>
        {
            if (ctx.Request.Headers.ContainsKey("Authorization"))
            {
                ctx.Token = ctx.Request.Headers["Authorization"];
            }

            return Task.CompletedTask;
        }
    };

    o.Configuration = new OpenIdConnectConfiguration()
    {
        SigningKeys = {new RsaSecurityKey(rsaKey)}
    };

    o.MapInboundClaims = false;
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterServices();
builder.Services.RegisterNotificationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

