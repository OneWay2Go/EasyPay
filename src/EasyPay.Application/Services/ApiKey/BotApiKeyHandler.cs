using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyPay.Application.Services.Auth;

public class BotApiKeyHandler : AuthenticationHandler<BotApiKeySchemeOptions>
{
    private readonly IConfiguration _configuration;
    
    public BotApiKeyHandler(
        IOptionsMonitor<BotApiKeySchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration configuration)
        : base(options, logger, encoder, clock)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Bot-Api-Key", out var extractedApiKey))
        { 
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var apiKey = _configuration["ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("ApiKey is missing"));
        }

        if (!string.Equals(apiKey, extractedApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("ApiKey is invalid"));
        }

        var claims = new List<Claim>(){ new Claim(ClaimTypes.Name, "TelegramBot") };
        var claimsIdentity = new ClaimsIdentity(claims, "BotApiKey");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var ticket = new AuthenticationTicket(claimsPrincipal, "BotApiKey");
        
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}