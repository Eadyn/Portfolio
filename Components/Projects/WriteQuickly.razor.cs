using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Timers;

namespace WriteQuickly.Pages.Projects
{
    public enum currentState
    {
        GREEN,
        YELLOW,
        RED
    }

    public partial class WriteQuicklyBase : ComponentBase
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Parameter] public string Text { get; set; } = string.Empty;

        private double defaultTime = 7.0; // 7 seconds between sections by default
        private double time = 7.0; // remaining time

        private currentState state = currentState.GREEN;

        private System.Timers.Timer timer;

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

        protected async override void OnInitialized()
        {

            await JSRuntime.InvokeVoidAsync("alert", "The Most Dangerous Writing App inspired application that encourages you to write a first draft quickly. After 7 seconds without typing, you reach Yellow mode, and after another 7 seconds, you reach Red mode. Try to stay in Green!");
            // JSRuntime.InvokeVoidAsync("bootstrap.Toast.getOrCreateInstance(document.getElementById('toast')).show()");
            timer = new System.Timers.Timer(100);
            timer.Elapsed += SwitchSettings;
            timer.AutoReset = true;
            timer.Start();
        }

        private async void SwitchSettings(object? sender, ElapsedEventArgs e)
        {
            time -= 0.1;

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });

            if (time <= 0)
            {
                switch (state)
                {
                    case currentState.GREEN:
                        state = currentState.YELLOW;
                        time = defaultTime;
                        break;
                    case currentState.YELLOW:
                        state = currentState.RED;
                        time = defaultTime;
                        break;
                    case currentState.RED:
                        break;
                }
            }
        }

        public string GetBackgroundColor()
        {
            switch (state)
            {
                case currentState.GREEN:
                    return "#28a745";
                case currentState.YELLOW:
                    // Slow blinking effect
                    if ((int)(time * 10) % 8< 4)
                    {
                        return "#ffc107";
                    }
                    else
                    {
                        return "#ff9c08";
                    }
                case currentState.RED:
                    // Fast blinking effect
                    if ((int) (time*10) % 4 < 2)
                    {
                        return "#dc3545";
                    }
                    else 
                    {
                        return "#ff0000";
                    }

                default:
                    return "white";
            }
        }

        public void ResetState()
        {
            state = currentState.GREEN;
            time = defaultTime;
        }
    }
}
