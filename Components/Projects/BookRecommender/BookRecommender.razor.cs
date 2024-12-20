using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace portfolio.Components.Projects.BookRecommender
{
    public partial class BookRecommender : ComponentBase
    {
        [Inject]
        private BookService bookService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private string BookTitle { get; set; }
        private List<string> CorrectedTitles { get; set; }
        private List<string> SimilarBooks { get; set; }

        private string SelectedTitle { get; set; }

        private async Task CorrectTitle()
        {
            try
            {
                var response = await bookService.GetCorrectedTitle(BookTitle);
                if (response.Any() && response[0].Contains("Title"))
                {
                    CorrectedTitles = response;
                    CorrectedTitles = CorrectedTitles.ConvertAll(d => IsolateTitle(d));
                }
                else
                {
                    JSRuntime.InvokeVoidAsync("alert", "No possible completions found.");   
                    
                    foreach (var title in response)
                    {
                        Console.WriteLine(title);
                    }

                }
            }
            catch (Exception e)
            {
                JSRuntime.InvokeVoidAsync("alert", e.Message);
            }
        }
        private async Task GetRecommendations()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(SelectedTitle))
                {
                    SimilarBooks = await bookService.GetSimilarBooks(SelectedTitle);
                    SimilarBooks = SimilarBooks.ConvertAll(d => IsolateTitle(d));
                }
            }
            catch (Exception e)
            {
                JSRuntime.InvokeVoidAsync("alert", e.Message);
            }
        }

        private void SelectTitle(string title)
        {
            SelectedTitle = title;
        }

        private bool IsSelected(string title)
        {
            return title == SelectedTitle;
        }
        private static string IsolateTitle(string title)
        {
            // return title.Split(":")[1].Remove(0, 1).Remove(title.Split(":")[1].Length - 2, 2);
            var match = Regex.Match(title, "\"Title\":\"(.*?)\"");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}