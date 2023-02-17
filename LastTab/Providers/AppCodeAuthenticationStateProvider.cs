using LastTab.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LastTab.Providers
{
    public class AppCodeAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILogger<AppCodeAuthenticationStateProvider> logger;
        private readonly IConfiguration configuration;
        private readonly ProtectedLocalStorage localstorage;

        public AppCodeAuthenticationStateProvider(IServiceProvider services)
        {
            logger = services.GetService<ILogger<AppCodeAuthenticationStateProvider>>();
            configuration = services.GetRequiredService<IConfiguration>();
            localstorage = services.GetRequiredService<ProtectedLocalStorage>();
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await localstorage.GetAsync<AppCodeToken>("Token");
            var authenticated = token.Success && AppCodeToken.IsMatch(token.Value,configuration);
            var identity = new ClaimsIdentity(authenticated ? "Basic" : string.Empty);            
            if (authenticated)
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, token.Value.User));
            }            
            var state = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
            NotifyAuthenticationStateChanged(state);
            return await state;
        }
    }
}
