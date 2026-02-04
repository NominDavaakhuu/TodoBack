using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.DTOs.Auth;
using Shared.DTOs.TodoItem;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

public class TodoBackApiClient
{
    private readonly Uri _baseUri;
    private readonly CookieContainer _cookies;

    public TodoBackApiClient(string baseUrl, CookieContainer cookies)
    {
        _baseUri = new Uri(baseUrl, UriKind.Absolute);
        _cookies = cookies ?? new CookieContainer();
    }

    private HttpClient CreateClient()
    {
        var handler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = _cookies,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        var client = new HttpClient(handler)
        {
            BaseAddress = _baseUri,
            Timeout = TimeSpan.FromSeconds(30)
        };

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        return client;
    }

    private static StringContent JsonBody(object obj) =>
        new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

    // ---------------- AUTH ----------------

    public async Task<(bool ok, string error)> LoginAsync(LoginDto dto)
    {
        using (var client = CreateClient())
        {
            var res = await client.PostAsync("api/auth/login", JsonBody(dto));

            if (res.IsSuccessStatusCode)
                return (true, null);

            var error = await ReadApiErrorAsync(res);
            return (false, error);
        }
    }

    public async Task<(bool ok, string error)> RegisterAsync(RegisterDto dto)
    {
        using (var client = CreateClient())
        {
            var res = await client.PostAsync("api/auth/register", JsonBody(dto));

            if (res.IsSuccessStatusCode)
                return (true, null);

            var error = await ReadApiErrorAsync(res);
            return (false, error);
        }
    }

    public async Task LogoutAsync()
    {
        using (var client = CreateClient())
        {
            await client.PostAsync("api/auth/logout", JsonBody(new { }));
        }
    }

    public async Task<(bool loggedIn, long? userId, string username)> MeAsync()
    {
        using (var client = CreateClient())
        {
            var res = await client.GetAsync("api/auth/me");
            if (res.StatusCode == HttpStatusCode.Unauthorized)
                return (false, null, null);

            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();

            dynamic me = JsonConvert.DeserializeObject(json);
            // Try to read as long (fall back to int if needed)
            long? userId = null;
            try { userId = (long)me.UserId; }
            catch { try { userId = (int)me.UserId; } catch { userId = null; } }

            string username = (string)me.Username;
            return (true, userId, username);
        }
    }

    // ---------------- TODOS ----------------
    // NOTE: These match your updated controller which returns raw DTOs (not envelopes)

    // GET api/todoItem
    public async Task<List<TodoDto>> GetMyTodosAsync()
    {
        using (var client = CreateClient())
        {
            var res = await client.GetAsync("api/todoItem");

            if (res.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("API session missing. Please login.");

            if (!res.IsSuccessStatusCode)
                throw new InvalidOperationException(await ReadApiErrorAsync(res));

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<TodoDto>>(json) ?? new List<TodoDto>();
        }
    }

    // GET api/todoItem/{id}
    public async Task<TodoDto> GetTodoByIdAsync(long id)
    {
        using (var client = CreateClient())
        {
            var res = await client.GetAsync($"api/todoItem/{id}");

            if (res.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("API session missing. Please login.");

            if (res.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!res.IsSuccessStatusCode)
                throw new InvalidOperationException(await ReadApiErrorAsync(res));

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TodoDto>(json);
        }
    }

    // POST api/todoItem
    public async Task<TodoDto> CreateTodoAsync(TodoCreateDto dto)
    {
        using (var client = CreateClient())
        {
            var res = await client.PostAsync("api/todoItem", JsonBody(dto));

            if (res.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("API session missing. Please login.");

            if (!res.IsSuccessStatusCode)
                throw new InvalidOperationException(await ReadApiErrorAsync(res));

            // Controller returns CreatedAtRoute with the DTO in the body
            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TodoDto>(json);
        }
    }

    // PUT api/todoItem/{id}
    public async Task<TodoDto> UpdateTodoAsync(long id, UpdateTodoDto dto)
    {
        using (var client = CreateClient())
        {
            var res = await client.PutAsync($"api/todoItem/{id}", JsonBody(dto));

            if (res.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("API session missing. Please login.");

            if (res.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!res.IsSuccessStatusCode)
                throw new InvalidOperationException(await ReadApiErrorAsync(res));

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TodoDto>(json);
        }
    }

    // PATCH api/todoItem/{id}/completed
    public async Task<TodoDto> MarkCompletedAsync(long id)
    {
        using (var client = CreateClient())
        {
            var req = new HttpRequestMessage(new HttpMethod("PATCH"), $"api/todoItem/{id}/completed");
            var res = await client.SendAsync(req);

            if (res.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("API session missing. Please login.");

            if (res.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!res.IsSuccessStatusCode)
                throw new InvalidOperationException(await ReadApiErrorAsync(res));

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TodoDto>(json);
        }
    }

    // DELETE api/todoItem/{id}
    public async Task<bool> DeleteTodoAsync(long id)
    {
        using (var client = CreateClient())
        {
            var res = await client.DeleteAsync($"api/todoItem/{id}");

            if (res.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("API session missing. Please login.");

            if (res.StatusCode == HttpStatusCode.NotFound)
                return false;

            if (res.StatusCode == HttpStatusCode.NoContent)
                return true;

            if (!res.IsSuccessStatusCode)
                throw new InvalidOperationException(await ReadApiErrorAsync(res));

            return true;
        }
    }

    // DELETE api/todoItem (delete all for current user)
    public async Task<bool> DeleteMyTodosAsync()
    {
        using (var client = CreateClient())
        {
            var res = await client.DeleteAsync("api/todoItem");

            if (res.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("API session missing. Please login.");

            if (res.StatusCode == HttpStatusCode.NotFound)
                return false;

            if (res.StatusCode == HttpStatusCode.NoContent)
                return true;

            if (!res.IsSuccessStatusCode)
                throw new InvalidOperationException(await ReadApiErrorAsync(res));

            return true;
        }
    }

    // ---------------- helpers ----------------

    private static async Task<string> ReadApiErrorAsync(HttpResponseMessage res)
    {
        var body = await res.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(body))
            return $"{(int)res.StatusCode} {res.ReasonPhrase}";

        try
        {
            var token = JToken.Parse(body);

            if (token.Type == JTokenType.Object)
            {
                var obj = (JObject)token;

                var error = (string)(obj["error"] ?? obj["Error"]);
                if (!string.IsNullOrWhiteSpace(error)) return error;

                var message = (string)(obj["message"] ?? obj["Message"]);
                if (!string.IsNullOrWhiteSpace(message)) return message;

                foreach (var prop in obj.Properties())
                {
                    if (prop.Value.Type == JTokenType.Array)
                    {
                        var first = prop.Value.First?.ToString();
                        if (!string.IsNullOrWhiteSpace(first)) return first;
                    }
                    else if (prop.Value.Type == JTokenType.String)
                    {
                        var str = prop.Value.ToString();
                        if (!string.IsNullOrWhiteSpace(str)) return str;
                    }
                }
            }

            return body; // unknown JSON shape
        }
        catch
        {
            return body; // not JSON
        }
    }
}
