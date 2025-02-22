using System.Collections.Generic;
using EPiServer.Shell;
using EPiServer.Shell.Navigation;

namespace ContentGeneratorAddon;

[MenuProvider]
public class NewGeneratorAdminMenuProvider : IMenuProvider
{
    public IEnumerable<MenuItem> GetMenuItems()
    {
        var urlMenuItem1 = new UrlMenuItem("Generate content", MenuPaths.Global + "/cms/admin/contentGeneratorAddon",
            Paths.ToResource(GetType(), "ContentGeneratorAddon/Index"))
        {
            IsAvailable = context => true,
            SortIndex = 100,
        };

        return new List<MenuItem>(1)
        {
            urlMenuItem1
        };
    }
}
