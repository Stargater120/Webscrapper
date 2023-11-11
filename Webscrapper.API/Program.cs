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
builder.Services.Configure<WebscrapperAppSettings>(configManager);
configManager.Bind(config);

var rsaKey = RSA.Create();
var privateKey = rsaKey.ExportRSAPrivateKey();

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
builder.Services.AddSingleton<KeyStore>(store =>
{
    var keyStore = new KeyStore();
    keyStore.RSAKey = privateKey;
    return keyStore;
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterServices();

builder.Services.RegisterNotificationServices();

builder.Services.AddCors(x =>
{
    x.AddPolicy("Price seer", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Price seer");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

