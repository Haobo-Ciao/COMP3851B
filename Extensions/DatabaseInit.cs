using CMS.Models;
using Newtonsoft.Json;
using System.Reflection;

namespace CMS.Extensions
{
    public class DatabaseInit
    {
        public static async Task OnDatabaseInit(IFreeSql freeSql,WebApplicationBuilder builder)
        {
            //var a = await freeSql.Select<contents>().Where(a => true).ToListAsync();
            //await File.WriteAllTextAsync(Path.Combine(builder.Environment.WebRootPath, "data", "contents.json"), JsonConvert.SerializeObject(a));
            //var b = await freeSql.Select<menus>().Where(a => true).ToListAsync();
            //await File.WriteAllTextAsync(Path.Combine(builder.Environment.WebRootPath, "data", "menus.json"), JsonConvert.SerializeObject(b));

            var models = Assembly.GetExecutingAssembly().GetTypes().Where(a => a.Namespace?.StartsWith("CMS.Models")??false);

            foreach (var model in models)
            {
                if (!freeSql.DbFirst.ExistsTable(model.Name))
                {
                    // add data tables
                    freeSql.CodeFirst.SyncStructure(model);

                    // add admin user
                    if (model.Name == nameof(users))
                    {
                        // add admin user
                        var admin = new users
                        {
                            Name = "admin",
                            Password = "admin",
                            UserName = "admin"
                        };
                        await freeSql.Insert(admin).ExecuteAffrowsAsync();
                    }
                }
            }

            var contents_file = new FileInfo(Path.Combine(builder.Environment.WebRootPath, "data","contents.json"));
            var menus_file = new FileInfo(Path.Combine(builder.Environment.WebRootPath, "data","menus.json"));
            if (menus_file.Exists)
            {
                if(!await freeSql.Select<menus>().AnyAsync())
                {
                    var content_data = JsonConvert.DeserializeObject<List<contents>>(await File.ReadAllTextAsync(contents_file.FullName));

                    var menus = JsonConvert.DeserializeObject<List<menus>>(await File.ReadAllTextAsync(menus_file.FullName));
                    //await freeSql.Insert(contents).ExecuteAffrowsAsync();
                    foreach (var menu in menus!)
                    {
                        var id = await freeSql.Insert(menu).ExecuteIdentityAsync();
                        var contetns = content_data!.Where(a=>a.MenuID==menu.ID).ToList();
                        contetns.AsParallel().ForAll(a => a.MenuID = (int)id);
                        await freeSql.Insert(contetns).ExecuteAffrowsAsync();
                    }
                }
            }
        }
    }
}
