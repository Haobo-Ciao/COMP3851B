namespace CMS;

public static class NavHelper
{
    public static List<NavModel> Navs { get; } = new()
    {
        new NavModel{ Title="Menu",Href="/admin/menu",Icon="mdi-trending-up"},
        //new NavModel{ Title="Content",Href="/admin/content/",Icon="mdi-trending-up"},
        new NavModel{ Title="User",Href="/admin/user",Icon="mdi-trending-up"},
    };
    public static List<PageTabItem> PageTabItems { get; } = new();
}

public record PageTabItem(string Title, string Href, string Icon);

public class NavModel
{
    public int Id { get; set; }

    public int ParentId { get; set; }

    public string? Href { get; set; }

    public string? Icon { get; set; }

    public string? ParentIcon { get; set; }

    public string? Title { get; set; }

    public string? FullTitle { get; set; }

    public bool Hide { get; set; }

    public bool Active { get; set; }

    public string? Target { get; set; }

    public NavModel[]? Children { get; set; }

}