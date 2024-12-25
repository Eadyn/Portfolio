using System.Net.Http.Json;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static portfolio.Components.Projects.BookRecommender.BookCard;

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
        var jsonDocument = JArray.Parse(content);
        
        List<string> neighbors = new List<string>();
        try
        {
            var firstElement = jsonDocument[0] as JArray;
            if (firstElement != null && firstElement.Count > 0)
            {
                var neighborsObject = firstElement[0];
                if (neighborsObject["neighbors"] != null)
                {
                    string neighborsRaw = neighborsObject["neighbors"].ToString();

                    // Check if the string contains both single and double quotes
                    if (neighborsRaw.Contains("'") && neighborsRaw.Contains("\""))
                    {
                        // Parse the JSON string directly without replacing single quotes
                        neighbors = JsonConvert.DeserializeObject<List<string>>(neighborsRaw);
                    }
                    else
                    {
                        // Replace single quotes with double quotes and parse the string
                        neighborsRaw = neighborsRaw.Replace("'", "\"");
                        neighbors = JsonConvert.DeserializeObject<List<string>>(neighborsRaw);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error parsing neighbors: " + ex.Message);
            Console.WriteLine("Content: " + content);
        }
        return neighbors;
    }

    public async Task<Book> GetBookDetails(Book book)
    {
        // Get book details from the Open Library API using the book's title
        var response = _httpClient.GetAsync($"https://openlibrary.org/search.json?title={Uri.EscapeDataString(book.Title)}&jscmd=details").Result;
        response.EnsureSuccessStatusCode();

        var content = response.Content.ReadAsStringAsync().Result;
        Console.WriteLine(content);

        var jsonDocument = JsonDocument.Parse(content);

        var root = jsonDocument.RootElement;

        // Get the book image url and description
        var docs = root.GetProperty("docs");

        if (docs.GetArrayLength() > 0)
        {
            var firstDoc = docs[0];
            if (firstDoc.TryGetProperty("cover_i", out var coverId))
            {
                book.CoverImageUrl = $"https://covers.openlibrary.org/b/id/{coverId}.jpg";
            }
            else
            {
                book.CoverImageUrl = "/images/generic_book.png";
            }

            if (firstDoc.TryGetProperty("description", out var description))
            {
                book.Description = description.GetString();
            }
            else
            {
                book.Description = "No description available.";
            }
        }

        return book;
    }
}
