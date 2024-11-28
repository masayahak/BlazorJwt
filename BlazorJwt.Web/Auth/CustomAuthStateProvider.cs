using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorJwt.Web.Auth
{
    // ============================================================
    // Blazor上でのログイン状態をJWT取得結果から判定＋格納する
    // AuthenticationStateProviderを継承することで、
    // Blazor特有の<AuthorizeView>タグなのに対応させる。
    // ============================================================
    public class CustomAuthStateProvider(ProtectedLocalStorage localStorage) : AuthenticationStateProvider
    {
        private const string LOCAL_STORAGE_KEY = "token";

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var tokenResult = await localStorage.GetAsync<string>(LOCAL_STORAGE_KEY);
            string token = tokenResult.Value ?? string.Empty;

            var identity = string.IsNullOrEmpty(token)
                ? new ClaimsIdentity()
                : GetClaimsIdentity(token);

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            await localStorage.SetAsync(LOCAL_STORAGE_KEY, token);
            var identity = GetClaimsIdentity(token);
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private ClaimsIdentity GetClaimsIdentity(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims;
            return new ClaimsIdentity(claims, "jwt");
        }

        public async Task MarkUserAsLoggedOut()
        {
            await localStorage.DeleteAsync(LOCAL_STORAGE_KEY);
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}
