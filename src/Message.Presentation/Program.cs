using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Message.Application;
using Message.Application.Models;
using Message.Application.Services;
using Message.Domain.Repositories;
using Message.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Po.Api.Response;
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
            // 當認證失敗時
            OnChallenge = async context =>
            {
                context.HandleResponse();
        
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Unauthorized",
                    Detail = context.ErrorDescription ?? "无效的认证令牌",
                    Instance = context.Request.Path
                };
                
                var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
                problemDetails.Extensions["traceId"] = traceId;
        
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/problem+json";
        
                await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
            },
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
       .AddApplication()
       .AddInfrastructure(builder.Configuration);



var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor   |
                       ForwardedHeaders.XForwardedHost  | 
                       ForwardedHeaders.XForwardedProto, 
        
    KnownProxies = { IPAddress.Parse("127.0.0.1") }
});

app.UseExceptionHandle();
app.UseAuthentication();
app.UseAuthorization();
    
app.UseWebSockets();


app.MapPost("/conversation", (
    HttpContext ctx, 
    IConversationRepository repository,
    long itemId) =>
{
    var sub = ctx.User.FindFirst("sub")?.Value;
    if(sub == null)
        throw Failure.Unauthorized();
    
    if(!long.TryParse(sub, out var userId))
        throw Failure.Unauthorized();
    
    var conversation = repository.Add(buyerId: userId, itemId: itemId);
    
    return Results.Ok(conversation.ToModel(userId));
}).RequireAuthorization();

app.MapGet("/conversation", (
    HttpContext ctx, 
    IConversationRepository repository) =>
{
    var sub = ctx.User.FindFirst("sub")?.Value;
    if(sub == null)
        throw Failure.Unauthorized();
    
    if(!long.TryParse(sub, out var userId))
        throw Failure.Unauthorized();

    var conversations = repository.Get(userId);
    
    return Results.Ok(conversations.Select(x => x.ToModel(userId)));
}).RequireAuthorization();

app.MapGet("/conversation/{buyerId:long}/{itemId:long}", (
    HttpContext ctx, 
    IConversationRepository repository,
    long buyerId,
    long itemId) =>
{
    var sub = ctx.User.FindFirst("sub")?.Value;
    if(sub == null)
        throw Failure.Unauthorized();
    
    if(!long.TryParse(sub, out var userId))
        throw Failure.Unauthorized();

    var conversation = repository.Get(buyerId, itemId);
    
    if(userId != buyerId && userId != conversation.Item.User.Id)
        throw Failure.Unauthorized();

    return Results.Ok(conversation.ToModel(userId));

}).RequireAuthorization();


app.MapGet("/chat/{buyerId:long}/{itemId:long}", async (
    HttpContext ctx, 
    IMessenger messenger,
    long buyerId,
    long itemId) =>
{
    if (!ctx.User.Identity?.IsAuthenticated ?? true)
    {
        ctx.Response.StatusCode = 401;
        return;
    }
        
    if (ctx.WebSockets.IsWebSocketRequest)
    {
        var sub = ctx.User.FindFirst("sub")?.Value;
        if(sub == null)
            throw Failure.Unauthorized();
    
        if(!long.TryParse(sub, out var userId))
            throw Failure.Unauthorized();
        
        // processing - 取得 WebSocket
        using var socket = await ctx.WebSockets.AcceptWebSocketAsync();
        
        // processing - 跑 WebSocket 處理邏輯
        await messenger.RunAsync(
            socket:socket, 
            buyerId: buyerId,
            itemId: itemId,
            userId: userId);
    }
}).RequireAuthorization();





app.Run();

