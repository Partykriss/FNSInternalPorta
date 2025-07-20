using System.DirectoryServices.AccountManagement;

namespace FNS.Main.Services
{
    public interface IUserIdentityService
    {
        string GetUserFullName(string userName);
    }

    public class UserIdentityService : IUserIdentityService
    {
        public string GetUserFullName(string userName)
        {
            using (var context = new PrincipalContext(ContextType.Domain))
            {
                var userPrincipal = UserPrincipal.FindByIdentity(context, userName);
                return userPrincipal != null ? userPrincipal.DisplayName : null;
            }
        }
    }

    public class MockUserIdentityService : IUserIdentityService
    {
        public string GetUserFullName(string userName)
        {
            return "Test User";
        }
    }
}