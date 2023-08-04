using BlazorComponent;
using CMS.Models;
using Force.DeepCloner;
using Masa.Blazor;
using Masa.Blazor.Presets;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Util.Reflection.Expressions;

namespace CMS.Pages.AdminPages
{
    public partial class Content
    {
        [Parameter]
        public string Menu { get; set; }

        [Parameter]
        public int Id { get; set; }

        [Inject]
        IFreeSql FreeSql { get; set; }

        [Inject]
        IHttpContextAccessor HttpContextAccessor { get; set; }

        [Inject]
        public IPopupService? PopupService { get; set; }

        [Inject]
        IWebHostEnvironment WebHostEnvironment { get; set; }

        [CascadingParameter]
        private IPageTabsProvider? PageTabsProvider { get; set; }
        const string PreModuelName = "part";
        List<contents> Items { get; set; }=new List<contents>();

        List<DataTableHeader<contents>> Headers { get; set; } = new List<DataTableHeader<contents>>()
        {
            new(){Text="ModuleName",Value="ModuleName"},
            new(){Text="Type",Value="Type"},
            new(){Text="Content",Value="Content"},
            new(){Text="Remark",Value="Remark"},
            new (){ Text= "Actions", Value= "actions",Sortable=false,Width="60px",Align=DataTableHeaderAlign.Center, }
        };

        List<ContentTypeSelect> TypeSelector = new List<ContentTypeSelect>()
        {
            new ContentTypeSelect{Text="Text",Value=0},
            new ContentTypeSelect{Text="Image",Value=1},
        };

        contents ContentEditor { get; set; }=new contents();

        contents Searcher { get; set; }=new contents() { Type=-1 };
        IBrowserFile BrowserFile { get; set; }
        bool EditDialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            PageTabsProvider?.UpdateTabTitle($"/admin/content/{Menu}/{Id}", $"Edit {Menu}");
            await OnQuery();
        }

        async Task OnQuery()
        {
            Items = await FreeSql.Select<contents>()
                .WhereIf(!string.IsNullOrEmpty(Searcher.ModuleName),a=>a.ModuleName.Contains(Searcher.ModuleName))
                .WhereIf(!string.IsNullOrEmpty(Searcher.Content),a=>a.Content.Contains(Searcher.Content))
                .WhereIf(Searcher.Type!=-1, a => a.Type==Searcher.Type)
                .Where(a => !a.IsDelete && a.MenuID == Id)
                .ToListAsync();
            await InvokeAsync(StateHasChanged);
        }

        void OnEditContent(contents content)
        {
            ContentEditor = content.DeepClone();
            EditDialog = true;
            BrowserFile = null;
        }

        async Task OnSaveContent()
        {
            if (ContentEditor.ID == 0)
            {
                ContentEditor.AddDate = DateTime.Now;
                ContentEditor.AddUser = HttpContextAccessor.HttpContext?.User?.Identity?.Name ?? "0";
            }
            ContentEditor.ModifyDate = DateTime.Now;
            ContentEditor.ModifyUser = HttpContextAccessor.HttpContext?.User?.Identity?.Name ?? "0";

            // save text
            if(ContentEditor.Type==0)
            {
                if(string.IsNullOrEmpty( ContentEditor.Content))
                {
                    await PopupService!.EnqueueSnackbarAsync("empty content is not allow!", AlertTypes.Warning);
                    return;
                }
            }
            // save img
            else
            {
                if(BrowserFile==null)
                {
                    await PopupService!.EnqueueSnackbarAsync("empty file is not allow!", AlertTypes.Warning);
                    return;
                }
                var file_type = BrowserFile.Name.Split(".")[1];
                var img_type = new string[] {"png","svg","jpg","jpeg" };
                if(!img_type.Any(a=>a==file_type.ToLower()))
                {
                    await PopupService!.EnqueueSnackbarAsync($"only img format file! ({string.Join(",",img_type)})", AlertTypes.Warning);
                    return;
                }

                var filename = BrowserFile.Name;

                var max_size = 10;
                var unit = 1024 * 1024;
                if(BrowserFile.Size > max_size* unit)
                {
                    await PopupService!.EnqueueSnackbarAsync($"file size can't not large than {max_size}M !", AlertTypes.Warning);
                    return;
                }

                var stream = BrowserFile.OpenReadStream(max_size* unit);

                var folder = Path.Combine(WebHostEnvironment.WebRootPath,"imgs",Menu);
                if(!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                var root_path = Path.Combine(folder, filename);
                if(File.Exists(root_path))
                    File.Delete(root_path);

                // wwwroot/imgs/{menu}/filename
                var releative_path = Path.Combine("imgs", Menu, filename);
                using var fs = File.Create(root_path);
                await stream.CopyToAsync(fs);
                await fs.FlushAsync();
                stream.Close();
                // save file and set the releative path to content
                ContentEditor.Content = releative_path.Replace("\\","/");
            }


            await FreeSql.InsertOrUpdate<contents>()
                .SetSource(ContentEditor)
                .ExecuteAffrowsAsync();

            EditDialog = false;
            await OnQuery();
        }

        async Task OnDeleteContent(contents content)
        {
            await FreeSql.Update<contents>(content)
                .Set(a => a.IsDelete == true)
                .ExecuteAffrowsAsync();
            await OnQuery();
        }

    }

    public class ContentTypeSelect
    {
        public int Value { get; set; }

        public string Text { get; set; }
    }
}
