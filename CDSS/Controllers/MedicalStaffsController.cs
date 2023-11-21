using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CDSS.Services;

namespace CDSS.Controllers;

public class MedicalStaffsController : Controller
{
    private const string REDIRECT_CNTR = "Home";
    private const string REDIRECT_ACTN = "Index";
    private const string LOGIN_VIEW = "Login";

    private readonly IDbService _dbSvc;
    private readonly IAuthService _authSvc;

    public MedicalStaffsController(IDbService dbSvc, IAuthService authSvc)
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
            @"SELECT MedicalStaffId, Username, Password, Role FROM MedicalStaff 
               WHERE Username = '{0}'";
                 


        if (!_authSvc.Authenticate(sqlLogin, user.Username, user.Password, out ClaimsPrincipal? principal))
        {
            ViewData["Message"] = "Incorrect Username or Password";
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
        // Sign out the current user
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Redirect to the specified URL after logoff or to a default action
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            // Redirect to a default action after logoff
            return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
        }
    }

    [AllowAnonymous]
    public IActionResult Forbidden()
    {
        return View();
    }

}
