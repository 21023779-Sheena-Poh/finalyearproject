using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CDSS.Services;
using CDSS.Models;

namespace CDSS.Controllers;

public class MedicalStaffsController : Controller
{
    private const string REDIRECT_CNTR = "Home";
    private const string REDIRECT_ACTN = "Index";
    private const string LOGIN_VIEW = "Login";

    private readonly AppDbContext _dbCtx;
    private readonly IDbService _dbSvc;
    private readonly IAuthService _authSvc;

    public MedicalStaffsController(AppDbContext dbContext, IDbService dbSvc, IAuthService authSvc)
    {
        _dbCtx = dbContext;
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

    [Authorize]
    public IActionResult ChangePassword()
    {
        return View();
    }

    // TODO L05 TASK 4C Implement HttpPost ChangePassword Action  
    [Authorize]
    [HttpPost]
    public IActionResult ChangePassword(PasswordDTO pwd)
    {
        var Id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        if (_dbCtx.Database.ExecuteSqlInterpolated(
            $@"UPDATE MedicalStaff
                  SET Password = 
                       {pwd.NewPwd}
            WHERE MedicalStaffId = {Id}
             AND Password =
                  {pwd.CurrentPwd}"
             ) == 1)
            ViewData["Msg"] = "Password Updated";
        else
            ViewData["Msg"] = "Failed to Update Password";
        return View();
    }

    // TODO L05 TASK 4A Check User Current Password is Correct 
    // Use FromSqlInterpolated to retrieve AppUser with userid and password
    [Authorize]
    public JsonResult VerifyCurrentPassword(string CurrentPwd)
    {
        var Id =
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        MedicalStaff? user = _dbCtx.MedicalStaff
            .FromSqlInterpolated(
            $@"SELECT * FROM MedicalStaff
               WHERE MedicalStaffId = {Id}
                AND Password = 
                    {CurrentPwd}")
            .FirstOrDefault();

        if (user != null)
            return Json(true);
        else
            return Json(false);
    }

    // TODO L05 TASK 4B Check User New Password is Valid
    // Similar to VerifyCurrentPassword but return true and false in reverse condition
    [Authorize]
    public JsonResult VerifyNewPassword(string NewPwd)
    {
        var Id =
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        MedicalStaff? user = _dbCtx.MedicalStaff
            .FromSqlInterpolated(
            $@"SELECT * FROM MedicalStaff
               WHERE MedicalStaffId = {Id}
                AND Password = {NewPwd}")
            .FirstOrDefault();

        if (user == null)
            return Json(true);
        else
            return Json(false);
    }

    // Add this method for NurseAdmin role to view all users
    [Authorize(Roles = "NurseAdmin")]
    public IActionResult ViewUsers()
    {
        var users = _dbSvc.GetList<MedicalStaff>("SELECT * FROM MedicalStaff");

        // Check for success message
        if (TempData.ContainsKey("SuccessMessage"))
        {
            ViewData["SuccessMessage"] = TempData["SuccessMessage"];
        }

        return View(users);
    }


    // Add this method for NurseAdmin role to create a new user
    [HttpGet]
    [Authorize(Roles = "NurseAdmin")]
    public IActionResult CreateUser()
    {
        ViewBag.Roles = new SelectList(new[] { "Doctor", "Nurse", "NurseAdmin", "Pharmacist" });
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "NurseAdmin")]
    public IActionResult CreateUser(MedicalStaff user)
    {
        // Check for duplicate username
        if (UsernameExists(user.Username))
        {
            ModelState.AddModelError("Username", "Username already exists. Please choose a different one.");
            ViewBag.Roles = new SelectList(new[] { "Doctor", "Nurse", "NurseAdmin", "Pharmacist" });
            return View(user);
        }

        if (ModelState.IsValid)
        {
            // Perform user creation logic
            // For simplicity, let's assume a generic SQL insert statement
            string insertSql = "INSERT INTO MedicalStaff (Username, Password, FirstName, LastName, Role) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')";
            _dbSvc.ExecSQL(insertSql, user.Username, user.Password, user.FirstName, user.LastName, user.Role);

            // Set success message in TempData
            TempData["SuccessMessage"] = "User created successfully.";

            return RedirectToAction("ViewUsers");
        }

        ViewBag.Roles = new SelectList(new[] { "Doctor", "Nurse", "NurseAdmin", "Pharmacist" });
        return View(user);
    }


    // Helper method to check if the username already exists
    private bool UsernameExists(string username)
    {
        // Query your database to check if the username already exists
        var existingUser = _dbSvc.GetList<MedicalStaff>($"SELECT * FROM MedicalStaff WHERE Username = '{username}'").FirstOrDefault();

        // If the username already exists, return true; otherwise, return false
        return existingUser != null;
    }


    // Add this method for NurseAdmin role to update user roles
    [HttpGet]
    [Authorize(Roles = "NurseAdmin")]
    public IActionResult UpdateUserRole(int id)
    {
        var user = _dbSvc.GetList<MedicalStaff>($"SELECT * FROM MedicalStaff WHERE MedicalStaffId = {id}").FirstOrDefault();

        if (user == null)
        {
            return NotFound();
        }

        ViewBag.Roles = new SelectList(new[] { "Doctor", "Nurse", "NurseAdmin", "Pharmacist" });
        return View(user);
    }

    [HttpPost]
    [Authorize(Roles = "NurseAdmin")]
    public IActionResult UpdateUserRole(MedicalStaff user)
    {
        if (ModelState.IsValid)
        {
            // Perform user role update logic
            // For simplicity, let's assume a generic SQL update statement
            string updateSql = "UPDATE MedicalStaff SET Role = '{0}' WHERE MedicalStaffId = {1}";
            _dbSvc.ExecSQL(updateSql, user.Role, user.MedicalStaffId);

            // Set success message in TempData
            TempData["SuccessMessage"] = "User role updated successfully.";

            return RedirectToAction("ViewUsers");
        }

        ViewBag.Roles = new SelectList(new[] { "Doctor", "Nurse", "NurseAdmin", "Pharmacist" });
        return View(user);
    }

    [Authorize(Roles = "NurseAdmin")]
    public IActionResult DeleteUser(int id)
    {
        // Perform user deletion logic
        // For simplicity, let's assume a generic SQL delete statement
        string deleteSql = $"DELETE FROM MedicalStaff WHERE MedicalStaffId = {id}";
        _dbSvc.ExecSQL(deleteSql);

        // Set success message in TempData
        TempData["SuccessMessage"] = "User deleted successfully.";

        return RedirectToAction("ViewUsers");
    }


}
