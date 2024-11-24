using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace WriteQuickly.Pages.Projects
{
    public partial class WriteQuicklyBase : ComponentBase
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Parameter] public string Text { get; set; } = string.Empty;

        public async Task CopyToClipboard()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error copying text to clipboard: " + ex.Message);
                await JSRuntime.InvokeVoidAsync("alert", "Error copying text to clipboard: " + ex.Message);
            }
        }

        public void ClearText()
        {
            Text = string.Empty;
        }
    }
}
