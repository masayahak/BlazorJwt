using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace BlazorJwt.Web.Auth
{
    // ============================================================
    // API CALL�p 
    // ------------------------------------------------------------
    // httpClient.GetFromJsonAsync�Ȃǂ����b�v��
    // �����I�Ƀ��[�J���X�g���[�W�ɕۑ�����JWT��t�^����
    // ============================================================
    public class ApiClient(
            HttpClient httpClient,
            ProtectedLocalStorage localStorage
        ) : System.Net.Http.HttpClient
    {

        // JWT��t�^
        private async Task SetAuthorizeHeader()
        {
            var token = (await localStorage.GetAsync<string>("token")).Value;
            if (token is null) return;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // ------------------ �ȉ��AGET/POST/PUT/DELETE -----------------------
        public async Task<T?> GetFromJsonAsync<T>(string path)
        {
            await SetAuthorizeHeader();
            return await httpClient.GetFromJsonAsync<T>(path);
        }
        public async Task<T1?> PostAsync<T1, T2>(string path, T2 postModel)
        {
            await SetAuthorizeHeader();
            var res = await httpClient.PostAsJsonAsync(path, postModel);
            if (res != null && res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                if (typeof(T1) == typeof(string))
                {
                    // ���ڕ�����Ƃ��ĕԂ�
                    return (T1)(object)content;
                }
                else
                {
                    // �w�肳�ꂽ�^�Ƀf�V���A���C�Y
                    return JsonConvert.DeserializeObject<T1>(content);
                }
            }
            return default;
        }

        public async Task<T1?> PutAsync<T1, T2>(string path, T2 postModel)
        {
            await SetAuthorizeHeader();
            var res = await httpClient.PutAsJsonAsync(path, postModel);
            if (res != null && res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                if (typeof(T1) == typeof(string))
                {
                    // ���ڕ�����Ƃ��ĕԂ�
                    return (T1)(object)content;
                }
                else
                {
                    // �w�肳�ꂽ�^�Ƀf�V���A���C�Y
                    return JsonConvert.DeserializeObject<T1>(content);
                }
            }
            return default;
        }
        public async Task<T?> DeleteAsync<T>(string path)
        {
            await SetAuthorizeHeader();
            return await httpClient.DeleteFromJsonAsync<T>(path);
        }
    }
}


