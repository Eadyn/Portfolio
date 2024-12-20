using System.Net.Http.Json;
using System.Text.Json;

public class BookService
{
    private readonly HttpClient _httpClient;

    public BookService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<string>> GetCorrectedTitle(string title)
    {
        var response = await _httpClient.GetAsync($"https://api.eadynt.com/search?title={Uri.EscapeDataString(title)}");
        response.EnsureSuccessStatusCode();
        var intermediate = await response.Content.ReadAsStringAsync();
        return intermediate.Split(',').ToList();
    }

    public async Task<List<string>> GetSimilarBooks(string correctedTitle)
    {
        Console.WriteLine(correctedTitle);
        var response = await _httpClient.GetAsync($"https://api.eadynt.com/similar?title={Uri.EscapeDataString(correctedTitle)}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        // return JsonSerializer.Deserialize<List<string>>(content);
        return content.Split(',').ToList();
    }
}
