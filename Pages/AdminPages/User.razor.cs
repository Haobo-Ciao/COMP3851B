using BlazorComponent;
using CMS.Models;
using Force.DeepCloner;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace CMS.Pages.AdminPages
{
    public partial class User
    {
        [Inject]
        IFreeSql FreeSql { get; set; }

        [Inject]
        IHttpContextAccessor HttpContextAccessor { get; set; }

        [Inject]
        public IPopupService? PopupService { get; set; }

        List<DataTableHeader<users>> Headers { get; set; } = new List<DataTableHeader<users>>()
        {
            new(){Text="User Name",Value="UserName"},
            new(){Text="Name",Value="Name"},
            new(){Text="Sex",Value="Sex"},
            new(){Text="Add Date",Value="AddDate"},
            new (){ Text= "Actions", Value= "actions",Sortable=false,Width="60px",Align=DataTableHeaderAlign.Center, }
        };

        bool EditDialog { get; set; }

        List<users> Items { get; set; } = new List<users>();

        users Searcher { get; set; }=new users() { Sex=-1 };
        users Editor { get; set; }=new users();

        protected override async Task OnInitializedAsync()
        {
            await OnQuery();
        }

        async Task OnQuery()
        {
            Items = await FreeSql.Select<users>()
                .WhereIf(!string.IsNullOrEmpty(Searcher.Name), a => a.Name.Contains(Searcher.Name))
                .WhereIf(!string.IsNullOrEmpty(Searcher.UserName), a => a.Name.Contains(Searcher.UserName))
                .WhereIf(Searcher.Sex!=-1, a => a.Sex==Searcher.Sex)
                .ToListAsync();
            await InvokeAsync(StateHasChanged);
        }

        void OnEditUser(users users)
        {
            Editor = users.DeepClone();
            EditDialog = true;
        }

        async Task OnDeleteUser(users users)
        {
            await FreeSql.Update<users>()
                .Where(a => a.ID == users.ID)
                .Set(a => a.IsDelete == true)
                .ExecuteAffrowsAsync();

            await OnQuery();
        }

        async Task OnSave()
        {
            await FreeSql.InsertOrUpdate<users>()
                .SetSource(Editor) 
                .ExecuteAffrowsAsync();

            await OnQuery();
            EditDialog = false;

        }
    }
}
