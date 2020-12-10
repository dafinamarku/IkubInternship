using IkubInternship.DataLayer;
using IkubInternship.Extensions;
using IkubInternship.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace IkubInternship.Controllers
{
  public class AccountController : Controller
  {

    private ApplicationSignInManager _signInManager;
    private ApplicationUserManager _userManager;
    private ProjectDbContext db = new ProjectDbContext();


    public ApplicationSignInManager SignInManager
    {
      get
      {
        return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
      }
      private set
      {
        _signInManager = value;
      }
    }

    public ApplicationUserManager UserManager
    {
      get
      {
        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
      }
      private set
      {
        _userManager = value;
      }
    }
    // GET: Account/Login
    public ActionResult Login()
    {
      return View();
    }

    // POST: Account/Login
    [HttpPost]
    public async Task<ActionResult> Login(LoginViewModel lvm)
    {
      if (ModelState.IsValid)
      {
        //login
        var userStore = new ApplicationUserStore(db);
        var userManager = new ApplicationUserManager(userStore);
        var user = userManager.Find(lvm.Username, lvm.Password);
        //userat qe jane fshire(jo perfundimisht) nuk mund te logohen
        if (user != null && user.DeleteStatus)
        {
          ModelState.AddModelError("myerror", "Invalid username or password");
          return View();
        }
          //login
          var authenticationManager = HttpContext.GetOwinContext().Authentication;
          //var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
          //authenticationManager.SignIn(new AuthenticationProperties(), userIdentity);
          SignInStatus result = await SignInManager.PasswordSignInAsync(lvm.Username, lvm.Password, true, shouldLockout: true);
      
        switch (result)
        {
          case SignInStatus.Success:
            return RedirectToAction("Index", "Home");
          case SignInStatus.LockedOut:
            return View("Lockout");
          case SignInStatus.Failure:
            ModelState.AddModelError("signInError", "Invalid username or password");
            return View();
          default:
            ModelState.AddModelError("signInError", "Invalid username or password.");
            return View(lvm);
        }
           
            //userManager.AccessFailed(user.Id);
            //await userManager.AccessFailedAsync(user.Id);
       
        }
      ModelState.AddModelError("myerror", "");
      return View();

    }

    // GET: Account/Logout
    public ActionResult Logout()
    {
      var authenticationManager = HttpContext.GetOwinContext().Authentication;
      authenticationManager.SignOut();
      return RedirectToAction("Index", "Home");
    }

    public ActionResult Profile()
    {
      if (Request.IsAuthenticated)
      {
        string uid = User.Identity.GetUserId();
        var model = db.Users.Where(u => u.Id == uid).FirstOrDefault();
        return View(model);
      }
      return RedirectToAction("Index", "Home");
    }
    [Authorize]
    public ActionResult ChangePassword()
    {
      ViewBag.UserId=User.Identity.GetUserId();
      ResetPasswordViewModel model = new ResetPasswordViewModel();
      return View(model);
    }

    [HttpPost]
    [Authorize]
    public ActionResult ChangePassword(ResetPasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        string uid = User.Identity.GetUserId();
        var user = db.Users.FirstOrDefault(x => x.Id == uid);
        user.PasswordHash = Crypto.HashPassword(model.NewPassword);
        db.SaveChanges();
        this.AddNotification("Password changed succesfully", NotificationType.SUCCESS);
        return RedirectToAction("Profile");
      }
      return View(model);
    }

  }
}