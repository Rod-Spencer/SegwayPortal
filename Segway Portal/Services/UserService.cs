using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Segway.EF.SegwayCntxt;

namespace Segway_Portal.Services;

public class UserService : UserService_Interface
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Injections

    public SegwayContext? SegDB { get; set; } = new SegwayContext();


    #endregion Injections
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    public UserService()
    {
    }

    public async Task<Portal_User?> ValidateUserAsync(string username, string password)
    {
        //var user = SegDB?.PortalUsers?.FirstOrDefault(u => u.User_Name == username);
        var user = SegDB?.PortalUsers?.FirstOrDefault(x => x.User_Name == username);

        if (user == null) return null;
        if (user.User_Password_Hash == null) return null;

        // Convert byte[] hash to Base64 string for PasswordHasher
        var hashedPasswordString = Convert.ToBase64String(user.User_Password_Hash);

        // Using ASP.NET Core Identity PasswordHasher
        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Portal_User>();
        var result = passwordHasher.VerifyHashedPassword(user, hashedPasswordString, password);
        if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
        {
            if (String.Compare(user.User_Password, password) != 0) return null;
        }

        return user;
    }

    public async Task<String?> UserAccessAsync(Guid ID)
    {
        var access = SegDB?.PortalUsersAccess?.FirstOrDefault(x => x.ID == ID);
        if (access is null) return null;
        return access.Description;
    }

}
