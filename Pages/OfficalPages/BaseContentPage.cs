using CMS.Models;
using Microsoft.AspNetCore.Components;

namespace CMS.Pages.OfficalPages
{
    public class BaseContentPage:ComponentBase
    {
        private readonly string menu;

        public BaseContentPage(string menu)
        {
            this.menu = menu;
        }

        [Inject]
        IFreeSql freeSql { get; set; }

        protected List<contents> Contents { get; set; }=new List<contents>();

        protected string? GetContent(string part,int type=0)
        {
            return Contents.FirstOrDefault(a => a.ModuleName == part&&a.Type==type)?.Content;
        }

        protected string? GetTitle(string part,string remark="title")
        {
            return Contents.FirstOrDefault(a => a.ModuleName == part && a.Remark== remark)?.Content;
        }

        protected string? GetImg(string part, int type = 1)
        {
            return Contents.FirstOrDefault(a => a.ModuleName == part && a.Type == type)?.Content;
        }

        protected IEnumerable<contents> GetContents(string part,int type=0)
        {
            return Contents.Where(a => a.ModuleName == part && a.Type == type);                
        }

        protected IEnumerable<IGrouping<string,contents>> GetPartGroupContents(string part="part")
        {
            return Contents.Where(a => a.ModuleName.Contains(part)).GroupBy(a => a.ModuleName);
        }

        protected override async Task OnInitializedAsync()
        {
            Contents = await freeSql.Select<menus,contents>()
                .LeftJoin((a,b)=>a.ID==b.MenuID)
                .Where((a,b)=>a.Menu== menu && b.IsDelete==false)
                .ToListAsync((a,b)=>b);
        }
    }
}
