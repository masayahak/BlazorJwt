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
    public class CustomAuthStateProvider(ProtectedLocalStorage localStorage) 
        : Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider
    {
        private const string LOCAL_STORAGE_KEY = "token";

        // ローカルストレージのJWTをもとに認証状態を取得する
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = string.Empty;
            try
            {
                var tokenResult = await localStorage.GetAsync<string>(LOCAL_STORAGE_KEY);
                token = tokenResult.Value ?? string.Empty;
            }

            // tokenの書き換えなどなんらかの異常を検知
            catch (Exception)
            {
                try
                {
                    // ローカルストレージを消去
                    await localStorage.DeleteAsync(LOCAL_STORAGE_KEY);
                }
                catch { }
            }

            var identity = string.IsNullOrEmpty(token)
                ? new ClaimsIdentity()
                : GetClaimsIdentity(token);

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        private ClaimsIdentity GetClaimsIdentity(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims;
            return new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
        }

        // ログイン成功時にJWTを受け取り、ローカルストレージへJWTを書き込み認証状態を反映する。
        public async Task MarkUserAsAuthenticated(string token)
        {
            await localStorage.SetAsync(LOCAL_STORAGE_KEY, token);
            var identity = GetClaimsIdentity(token);
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        // ログアウト時にローカルストレージのJWTを削除し、認証状態を反映する。
        public async Task MarkUserAsLoggedOut()
        {
            await localStorage.DeleteAsync(LOCAL_STORAGE_KEY);

            // 認証者なしにする
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}
