using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Components
{
    public static class NavigationManagerExtensions
    {
        public static ValueTask<object> NavigateTo(this NavigationManager navigationManager, IJSRuntime jsRuntime, string uri, bool newWindow = false)
        {
            return jsRuntime.InvokeAsync<object>("open", uri, "_blank");
        }
    }
}