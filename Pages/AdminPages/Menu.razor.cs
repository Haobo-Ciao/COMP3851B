using BlazorComponent;
using CMS.Models;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace CMS.Pages.AdminPages
{
    public partial class Menu
    {
        [Inject]
        IFreeSql FreeSql { get; set; }

        [Inject]
        IHttpContextAccessor ContextAccessor { get; set; }  

        [Inject]
        NavigationManager NavigationManager { get; set; }

        bool EditDialog { get; set; }

        menus Searcher { get; set; } = new menus();

        menus MenuEditor { get; set; } = new menus();

        List<DataTableHeader<menus>> Headers { get; set; } = new List<DataTableHeader<menus>>()
        {
            new (){ Text="",Value="data-table-expand"},
            new DataTableHeader<menus>{Text="Menu",Value="Menu",Sortable=false,Filterable=false},
            new DataTableHeader<menus>{Text="Sort",Value="Sort"},
            new DataTableHeader<menus>{Text="Add Date",Value="AddDate"},
            new DataTableHeader<menus>{Text="Add User",Value="AddUser"},
            new DataTableHeader<menus>{Text="Modify Date",Value="ModifyDate"},
            new DataTableHeader<menus>{Text="Modify User",Value="ModifyUser"},
            new (){ Text= "Actions", Value= "actions",Sortable=false,Width="100px",Align=DataTableHeaderAlign.Center, }
        };

        List<menus> Items { get; set; }=new List<menus>();

        protected override async Task OnInitializedAsync()
        {
            await QueryMenus();
        }

        async Task QueryMenus()
        {
            Items = await FreeSql.Select<menus>()
                .WhereIf(!string.IsNullOrEmpty(Searcher.Menu),a=>a.Menu.Contains(Searcher.Menu))
                .Where(a => !a.IsDelete)
                .OrderBy(a => a.Sort)
                .ToListAsync();
            await InvokeAsync(StateHasChanged);
        }

        void OnEditContent(menus menu) => NavigationManager.NavigateTo($"/admin/content/{menu.Menu}/{menu.ID}");

        void OnEditMenu(menus selector)
        {
            MenuEditor = selector;
            EditDialog = true;
        }

        async Task OnSaveMenu()
        {
            if(MenuEditor.ID==0)
            {
                MenuEditor.AddDate = DateTime.Now;
                MenuEditor.AddUser = ContextAccessor.HttpContext?.User?.Identity?.Name ?? "0";
            }
            MenuEditor.ModifyDate = DateTime.Now;
            MenuEditor.ModifyUser = ContextAccessor.HttpContext?.User?.Identity?.Name ?? "0";
            await FreeSql.InsertOrUpdate<menus>()
                .SetSource(MenuEditor)
                .ExecuteAffrowsAsync();
            EditDialog = false;
            await QueryMenus();
        }

        async Task OnDeleteMenu(menus selector)
        {
            await FreeSql.Update<menus>()
                .Set(a=>a.IsDelete==true)
                .Where(selector).ExecuteAffrowsAsync();
            await QueryMenus();
        }
    }
}
