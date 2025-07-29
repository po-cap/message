using System.Security.Claims;
using Message.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddAuthentication("jwt")
        .AddJwtBearer("jwt", o =>
        {
            // Description - 
            //     告訴 framework，不要把 claim type 變成 Microsoft 自定義的 Type 
            o.MapInboundClaims = false;
        
            // Description - 
            //     定義 openid 的 endpoint
            var OIDC = builder.Configuration["OIDC"];
            o.Authority = $"{OIDC}/oauth";
        
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
                OnTokenValidated = context =>
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
    


    builder.Services.AddApplication(builder.Configuration);
}


var app = builder.Build();
{
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
            var sub = ctx.User.FindFirstValue("sub");
            if (sub == null)
            {
                ctx.Response.StatusCode = 401;
            }
            else
            {
                using var socket = await ctx.WebSockets.AcceptWebSocketAsync();
                await messenger.RunAsync(
                    socket:socket,
                    userId:sub);
            } 
        }
    }).RequireAuthorization();
    
    
    app.Run();
}

