using System.Threading.Tasks;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;

namespace Alloy.Sample
{
    /// <summary>
    /// Provision the database for easier development by:
    ///  * Enabling project mode
    ///  * Adding some default users
    ///
    /// This file is preferably deployed in the App_Code folder, where it will be picked up and executed automatically.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ProvisionDatabase : IInitializableModule
    {
        private static readonly ILogger _log = EPiServer.Logging.LogManager.GetLogger(typeof(ProvisionDatabase));

        public void Initialize(InitializationEngine context)
        {
            _log.Log(Level.Information, "Starting to provision users and groups");

            AddUsersAndRoles(context.Locate.Advanced.GetInstance<IContentSecurityRepository>());
        }

        public void Uninitialize(InitializationEngine context) { }

        #region Users and Roles

        private async void AddUsersAndRoles(IContentSecurityRepository securityRepository)
        {
            const string password = "sparr0wHawk!";

            await AddRole("WebAdmins", AccessLevel.FullAccess, securityRepository);
            await AddRole("WebEditors", AccessLevel.FullAccess ^ AccessLevel.Administer, securityRepository);

            await AddUser("cmsadmin", "Administrator Administrator", password, new[] { "WebEditors", "WebAdmins" });
            await AddUser("abbie", "Abbie Andrea", password, new[] { "WebEditors", "WebAdmins" });
            await AddUser("eddie", "Eddie Elridge", password, new[] { "WebEditors" });
            await AddUser("erin", "Erin Ehrhardt", password, new[] { "WebEditors" });
            await AddUser("reid", "Reid Rezac", password, new[] { "WebEditors" });
        }

        private async Task AddUser(string userName, string fullName, string passWord, string[] roleNames)
        {
            _log.Log(Level.Information, $"Adding user {userName}");

            if (await UIUserProvider.GetUserAsync(userName) != null)
            {
                _log.Log(Level.Information, $"User {userName} already exists");
                return;
            }

            var email = $"epic-{userName}@mailinator.com";
            await UIUserProvider.CreateUserAsync(userName, passWord, email, null, null, true);
            await UIRoleProvider.AddUserToRolesAsync(userName, roleNames);
        }

        private async Task AddRole(string roleName, AccessLevel accessLevel, IContentSecurityRepository securityRepository)
        {
            _log.Log(Level.Information, $"Adding role {roleName}");

            if (await UIRoleProvider.RoleExistsAsync(roleName))
            {
                _log.Log(Level.Information, $"Role {roleName} already exists");
                return;
            }

            await UIRoleProvider.CreateRoleAsync(roleName);

            var permissions = (IContentSecurityDescriptor)securityRepository.Get(ContentReference.RootPage).CreateWritableClone();
            permissions.AddEntry(new AccessControlEntry(roleName, accessLevel));

            securityRepository.Save(ContentReference.RootPage, permissions, SecuritySaveType.Replace);
            securityRepository.Save(ContentReference.WasteBasket, permissions, SecuritySaveType.Replace);
        }

        UIUserProvider UIUserProvider => ServiceLocator.Current.GetInstance<UIUserProvider>();

        UIRoleProvider UIRoleProvider => ServiceLocator.Current.GetInstance<UIRoleProvider>();

        #endregion
    }
}
