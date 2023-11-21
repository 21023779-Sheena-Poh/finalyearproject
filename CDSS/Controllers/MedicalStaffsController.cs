/*using Microsoft.AspNetCore.Mvc;

namespace MedicalStaff.Controllers;

public class MedicalStaffController : Controller
{
    private const string REDIRECT_CNTR = "Home";
    private const string REDIRECT_ACTN = "CDSS";
    private const string LOGIN_VIEW = "Login";

    private readonly IDbService _dbSvc;
    private readonly IAuthService _authSvc;

    public MedicalStaffController(IDbService dbSvc, IAuthService authSvc)
    {
        _dbSvc = dbSvc;
        _authSvc = authSvc;
    }

    [AllowAnonymous]
    public IActionResult Login(string returnUrl = null!)
    {
        TempData["ReturnUrl"] = returnUrl;
        return View(LOGIN_VIEW);
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Login(LoginUser user)
    {
        const string sqlLogin =
            @"SELECT Id, Username, UserRole FROM AppUser 
               WHERE Id = '{0}' 
                 AND UserPass = HASHBYTES('SHA1', '{1}')";


        if (!_authSvc.Authenticate(sqlLogin, user.UserId, user.Password,
                                   out ClaimsPrincipal? principal))
        {
            ViewData["Message"] = "Incorrect Email or Password";
            ViewData["MsgType"] = "warning";
            return View(LOGIN_VIEW);
        }
        else
        {
            HttpContext.SignInAsync(
               CookieAuthenticationDefaults.AuthenticationScheme, // Default Scheme
               principal!,
               new AuthenticationProperties
               {
                   IsPersistent = true
               });

            if (TempData["returnUrl"] != null)
            {
                string returnUrl = TempData["returnUrl"]!.ToString()!;
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
            }

            return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
        }
    }

    [Authorize]
    public IActionResult Logoff(string returnUrl = null!)
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
    }

    [AllowAnonymous]
    public IActionResult Forbidden()
    {
        return View();
    }

}
*/