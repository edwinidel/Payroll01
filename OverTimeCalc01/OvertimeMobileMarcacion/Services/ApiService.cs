using System.Net.Http.Json;
using OvertimeMobileMarcacion.Models;

namespace OvertimeMobileMarcacion.Services;

public class ApiService
{
    private readonly HttpClient _httpClient = new HttpClient();

    public async Task<bool> SendMarcacion(MarcacionDto marcacion, string apiUrl, string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync($"{apiUrl}/api/marcacion", marcacion);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}