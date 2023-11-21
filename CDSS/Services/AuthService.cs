using System.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using CDSS.Services;

namespace CDSS.Services;

public interface IAuthService
{
    bool Authenticate(string sqlLogin, string username, string password, out ClaimsPrincipal? principal);
}

public class AuthService : IAuthService
{
    private readonly IDbService _dbSvc;
    public AuthService(IDbService dbSvc)
    {
        _dbSvc = dbSvc;
    }

    public bool Authenticate(string sqlLogin, string username, string password, out ClaimsPrincipal? principal)
    {
        principal = null;
        DataTable ds = _dbSvc.GetTable(sqlLogin, username, password);
        if (ds.Rows.Count == 1)
        {
            principal =
               new ClaimsPrincipal(
                  new ClaimsIdentity(
                     new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, 
                                  // 1st Column is User Id
                                  ds.Rows[0][0].ToString()!),
                        new Claim(ClaimTypes.Name, 
                                  // 2nd Column is User Name   
                                  ds.Rows[0][1].ToString()!),
                        new Claim(ClaimTypes.Role, 
                                  // 3rd Column is User Role
                                  ds.Rows[0][3].ToString()!)
                     }, "Basic"
                  )
               );
            return true;
        }
        return false;
    }

}
