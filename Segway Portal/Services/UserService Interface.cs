using Segway.EF.SegwayCntxt;

namespace Segway_Portal.Services;

public interface UserService_Interface
{
    Task<Portal_User?> ValidateUserAsync(String username, String password);
    Task<String?> UserAccessAsync(Guid ID);
}
