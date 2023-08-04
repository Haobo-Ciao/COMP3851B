using CMS.Auth;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Implementation;

namespace CMS.AdminComponents
{
    public class AjaxService
    {
        private readonly IJSRuntime jSRuntime;

        public AjaxService(IJSRuntime jSRuntime)
        {
            this.jSRuntime = jSRuntime;
        }

        const string  path = "/js/ajax.js";

        IJSObjectReference jSObjectReference { get; set; }

        public async Task<T> Execute<T>(AjaxOption option)
        {
            jSObjectReference ??= await jSRuntime!.InvokeAsync<IJSObjectReference>(identifier: "import", $"{path}");

            var result = await jSObjectReference!.InvokeAsync<System.Text.Json.JsonDocument>("execute", option);

            return System.Text.Json.JsonSerializer.Deserialize<T>(result?.RootElement.ToString())??default(T);
        }

        public async Task Execute(AjaxOption option)
        {
            jSObjectReference ??= await jSRuntime!.InvokeAsync<IJSObjectReference>(identifier: "import", $"{path}");
            await jSObjectReference!.InvokeAsync<System.Text.Json.JsonDocument>("execute", option);
        }
    }
}
