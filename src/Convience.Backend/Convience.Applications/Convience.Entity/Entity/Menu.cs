
using Convience.Entity.Data;
using Convience.EntityFrameWork.Infrastructure;

using System.ComponentModel;

namespace Convience.Entity.Entity
{
    [Entity(DbContextType = typeof(SystemIdentityDbContext))]
    public class Menu
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Description("前端識別")]
        public string Identification { get; set; }

        [Description("後端權限")]
        public string Permission { get; set; }

        public MenuType Type { get; set; }

        public string Route { get; set; }

        public int Sort { get; set; }

        public Menu() { }

        public Menu(int id, string name, string identification,
            string permission, int type, string route, int sort)
        {
            Id = id;
            Name = name;
            Identification = identification;
            Permission = permission;
            Type = (MenuType)type;
            Route = route;
            Sort = sort;
        }
    }

    public enum MenuType
    {
        未知 = 0,
        菜單 = 1,
        按鈕 = 2,
        鏈結 = 3,
    }

}
