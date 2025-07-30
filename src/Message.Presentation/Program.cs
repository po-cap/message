using System.Net;
using Message.Application;
using Message.Application.Commands;
using Message.Application.Services;
using Message.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Shared.Mediator.Interface;

var builder = WebApplication.CreateBuilder(args);

var dir    = Environment.GetEnvironmentVariable("ASPNETCORE_DIRECTORY");
var env    = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
             ?? throw new Exception("Set \"ASPNETCORE_ENVIRONMENT\"");

builder.Configuration
    .SetBasePath(dir ?? Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();


var OIDC = builder.Configuration["OIDC"];

builder.Services.AddAuthentication("jwt")
    .AddJwtBearer("jwt", o =>
    {
        // Description - 
        //     告訴 framework，不要把 claim type 變成 Microsoft 自定義的 Type 
        o.MapInboundClaims = false;
        
        // Description - 
        //     定義 openid 的 endpoint
        o.Authority = $"{OIDC}/oauth";
        // TODO: 這個很危險，但去掉不知道為啥會有問題，去搞清楚，並去掉它
        //o.RequireHttpsMetadata = false;
            
        // Description - 
        //     定義 Validate 過程中要 validate 哪些資料
        o.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            RequireExpirationTime = true
        };
            
        // 啟用詳細錯誤訊息
        o.IncludeErrorDetails = true;
        
        // 事件處理器用於記錄詳細錯誤
        o.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = _ =>
            {
                Console.WriteLine("Token validated successfully");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(o =>
{
    o.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes("jwt")
        .RequireClaim("sub")
        .Build();
});

builder.Services
       .AddApplication(builder.Configuration)
       .AddInfrastructure(builder.Configuration);



var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor   |
                       ForwardedHeaders.XForwardedHost  | 
                       ForwardedHeaders.XForwardedProto, 
        
    KnownProxies = { IPAddress.Parse("127.0.0.1") }
});   
    
app.UseAuthentication();
app.UseAuthorization();
    
app.UseWebSockets();

app.MapGet("/chat", async (HttpContext ctx, IMessenger messenger) =>
{
    if (!ctx.User.Identity?.IsAuthenticated ?? true)
    {
        ctx.Response.StatusCode = 401;
        return;
    }
        
    if (ctx.WebSockets.IsWebSocketRequest)
    {
        // processing - 取得 Json Web Token
        var token = ctx.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        
        // processing - 取得 WebSocket
        using var socket = await ctx.WebSockets.AcceptWebSocketAsync();
        
        // processing - 跑 WebSocket 處理邏輯
        await messenger.RunAsync(socket:socket, token:token);
    }
}).RequireAuthorization();

app.MapGet("/chat/message", async (IMediator mediator, long from, long to) =>
{
    var notes = await mediator.SendAsync(new GetUnReadNotesCommand()
    {
        From = from,
        To = to,
    });
    return Results.Ok(notes);
});

app.MapGet("/chat/summary", async (IMediator mediator,long to) =>
{
    var conversations = await mediator.SendAsync(new SummaryCommand()
    {
        To = to,
    });
    return Results.Ok(conversations);
});

app.Run();

