using IkubInternship.DomainModels;
using IkubInternship.Extensions;
using IkubInternship.ServiceContracts;
using IkubInternship.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IkubInternship.Controllers
{
  [Authorize]
  public class PermissionController : Controller
  {
    IPermissionService permissionService;

    public PermissionController(IPermissionService ser)
    {
      this.permissionService = ser;
    }

    [Authorize(Roles = "Supervisor, Employee")]
    public ActionResult AskPermission()
    {
      return View();
    }

    [Authorize(Roles = "Supervisor, Employee")]
    [HttpPost]
    public ActionResult AskPermission([Bind(Include = "PermissionDate, ReasonForAsking")]Permission model)
    {
      if (ModelState.IsValid)
      {
        model.EmployeeId = User.Identity.GetUserId();
        model.Status = "Asked";
        Result<bool> result = permissionService.InsertPermission(model);

        if (result.HasError)
        {
          ModelState.AddModelError("Err", result.MessageResult);
          return View(model);
        }
        else
        {
          this.AddNotification("Permission request was sent successfully.", NotificationType.SUCCESS);
          return RedirectToAction("MyPermissions");
        }
      }
      else
      {
        return View(model);
      }
    }

    [Authorize(Roles = "Supervisor, Employee")]
    public ActionResult MyPermissions()
    {
      string uid = User.Identity.GetUserId();
      var result = permissionService.GetPermissionsForEmployee(uid);
      if (result.HasError == true)
        this.AddNotification(result.MessageResult, NotificationType.ERROR);

      return View(result.ReturnValue);
    }

    [Authorize(Roles = "Supervisor, HR")]
    public ActionResult ManagePermissions()
    {
      MultiResult<Permission> result;

      if (User.IsInRole("Supervisor"))
        result = permissionService.GetPermissionsForSupervisor(User.Identity.GetUserId());
      else
        result = permissionService.GetPermissionsForHr();

      if (result.HasError)
        this.AddNotification(result.MessageResult, NotificationType.ERROR);

      return View(result.ReturnValue);
    }


    [Authorize(Roles = "Supervisor, HR")]
    public ActionResult RefusePermission(int id, int? details, int? manage)
    {
      string userRole;
      if (User.IsInRole("HR"))
        userRole = "HR";
      else
        userRole = "Supervisor";
      var can = permissionService.CanBeRefusedOrApprovedBy(User.Identity.GetUserId(), userRole, id);
      if (can.HasError)
      {
        this.AddNotification(can.MessageResult, NotificationType.ERROR);
        if (details == null)
          return RedirectToAction("ManagePermissions");
        else
          return RedirectToAction("Details", new { @id = id, @manage=manage });
      }
      var res = permissionService.GetPermissionById(id);
      var permissionToRefuse = res.ReturnValue;
      RefusePermissionViewModel model = new RefusePermissionViewModel()
      {
        Id = permissionToRefuse.PermissionId,
        ReasonForRefusal = permissionToRefuse.ReasonForRefusal
      };
      ViewBag.details = details;
      ViewBag.manage = manage;
      return View(model);
    }

    [Authorize(Roles = "Supervisor, HR")]
    [HttpPost]
    public ActionResult RefusePermission(RefusePermissionViewModel model, int? details, int? manage)
    {
      if (ModelState.IsValid)
      {
        string userRole;
        if (User.IsInRole("HR"))
          userRole = "HR";
        else
          userRole = "Supervisor";
        var can = permissionService.CanBeRefusedOrApprovedBy(User.Identity.GetUserId(), userRole, model.Id);
        if (can.HasError)
        {
          this.AddNotification(can.MessageResult, NotificationType.ERROR);
          if (details != null)
            return RedirectToAction("Details", new { @id = model.Id, @manage = manage });
          else
            return RedirectToAction("ManagePermissions");
        }

        var res1 = permissionService.GetPermissionById(model.Id);
        if (res1.HasError)
        {
          this.AddNotification(res1.MessageResult, NotificationType.ERROR);
        }
        else
        {
          var permissionToRefuse = res1.ReturnValue;
          permissionToRefuse.Status = "Refused";
          permissionToRefuse.ReasonForRefusal = model.ReasonForRefusal;
          Result<bool> result = permissionService.UpdatePermission(permissionToRefuse);
          if (result.HasError)
          {
            this.AddNotification(result.MessageResult, NotificationType.ERROR);
          }
          else
            this.AddNotification("Permission refused.", NotificationType.SUCCESS);
        }
        if (details != null)
          return RedirectToAction("Details", new { @id = model.Id, @manage = manage });
        else
          return RedirectToAction("ManagePermissions");
      }
      else
      {
        return View(model);
      }
    }


    [Authorize(Roles = "HR, Supervisor")]
    [HttpPost]
    public ActionResult ApprovePermission(int id, int? details, int? manage)
    {
        string userRole;
        if (User.IsInRole("HR"))
          userRole = "HR";
        else
          userRole = "Supervisor";
        var can = permissionService.CanBeRefusedOrApprovedBy(User.Identity.GetUserId(), userRole, id);
        if (can.HasError)
        {
          this.AddNotification(can.MessageResult, NotificationType.ERROR);
          if (details == null)
            return RedirectToAction("ManagePermissions");
          else
            return RedirectToAction("Details", new { @id = id, @manage=manage });
        }

        var res1 = permissionService.GetPermissionById(id);
        if (res1.HasError)
        {
          this.AddNotification(res1.MessageResult, NotificationType.ERROR);
        }
        else
        {
          var permissionToRefuse = res1.ReturnValue;
          permissionToRefuse.Status = "Approved";
          Result<bool> result = permissionService.UpdatePermission(permissionToRefuse);
          if (result.HasError)
          {
            this.AddNotification(result.MessageResult, NotificationType.ERROR);
          }
          else
            this.AddNotification("Permission approved.", NotificationType.SUCCESS);
        }
        if (details == null)
          return RedirectToAction("ManagePermissions");
        else
          return RedirectToAction("Details", new { @id = id, @manage=manage });
    }

    //manage percakton faqen ku do te kthehet useri nqs ndodh nje gabim (Manage Permissions ose MyPermissions)
    public ActionResult Details(int id, int? manage)
    {
      string uid = User.Identity.GetUserId();
      string userRole;
      if (User.IsInRole("Supervisor"))
        userRole = "Supervisor";
      else
      {
        if (User.IsInRole("HR"))
          userRole = "HR";
        else
          userRole = "Employee";
      }

      Result<Permission> res = permissionService.CanUserViewDetailsOf(uid, userRole, id);
      if (res.HasError)
      {
        this.AddNotification(res.MessageResult, NotificationType.ERROR);
        if(User.IsInRole("HR") || (User.IsInRole("Supervisor") && manage != null))
          return RedirectToAction("ManagePermissions");
        else
          return RedirectToAction("MyPermissions");
      }
      else
      {
        ViewBag.manage = manage;
        return View(res.ReturnValue);
      }
    }


    [Authorize(Roles = "HR")]
    public ActionResult NrOfPermissions()
    {
      var res = permissionService.GetPermissionsPerYear(DateTime.Now.Year);
      if (res.HasError)
      {
        this.AddNotification(res.MessageResult, NotificationType.ERROR);
      }
      if (res.ReturnValue == null) //ne kete rast nuk eshte caktuar nr i lejeve per vitin aktual
        return RedirectToAction("SetNrOfPermissions");
      else
        return View(res.ReturnValue);
    }

    [Authorize(Roles ="HR")]
    public ActionResult SetNrOfPermissions()
    {
      var currentYear = DateTime.Now.Year;
      var model = permissionService.GetPermissionsPerYear(currentYear);
      if (model.HasError)
      {
        this.AddNotification(model.MessageResult, NotificationType.ERROR);
        return RedirectToAction("NrOfPermissions"); ///////////////////////
      }
      var ReturnVal = model.ReturnValue;
      if(ReturnVal==null)//nuk eshte percaktuar akoma nr i lejeve per kete vit
      {
        var model1 = new PermissionsPerYear()
        {
          Year = currentYear,
          NrOfPermissions = 0
        };

        return View(model1);
      }
      return View(ReturnVal);
    }


    [Authorize(Roles = "HR")]
    [HttpPost]
    public ActionResult SetNrOfPermissions(PermissionsPerYear model)
    {
      if (ModelState.IsValid)
      {
        var result = permissionService.SetNrOfPermissionsPerYear(model.NrOfPermissions);
        if (result.HasError)
        {
          ModelState.AddModelError("Err", result.MessageResult);
          return View(model);
        }
        else
        {
          this.AddNotification("Nr of Permissions was set.", NotificationType.SUCCESS);
          return RedirectToAction("NrOfPermissions");
        }
      }
      else
        return View(model);
    }

    [Authorize(Roles ="Supervisor, Employee")]
    [HttpPost]
    public ActionResult CancelPermission(int id)
    {
      var res = permissionService.CanUserCancelPermission(User.Identity.GetUserId(), id);
      if (res.HasError)
      {
        this.AddNotification(res.MessageResult, NotificationType.ERROR);
        return RedirectToAction("MyPermissions");
      }
      var getPermission = permissionService.GetPermissionById(id);
      var permissionToCancel = getPermission.ReturnValue;
      permissionToCancel.Status = "Canceled";
      var update = permissionService.UpdatePermission(permissionToCancel);
      if (update.HasError)
        this.AddNotification(update.MessageResult, NotificationType.ERROR);
      else
        this.AddNotification("Permission canceled.", NotificationType.SUCCESS);
      return RedirectToAction("MyPermissions");
    }

    [Authorize(Roles="HR, Supervisor")]
    [HttpPost]
    public ActionResult Delete(int id)
    {
      string role;
      if (User.IsInRole("HR"))
        role = "HR";
      else
        role = "Supervisor";
      string uid = User.Identity.GetUserId();
      var result = permissionService.CanUserDeletePermission(uid, role, id);
      if (result.HasError)
      {
        this.AddNotification(result.MessageResult, NotificationType.ERROR);
      }
      else
      {
        var deleteRes = permissionService.DeletePermission(id);
        if (deleteRes.HasError)
          this.AddNotification(deleteRes.MessageResult, NotificationType.ERROR);
        else
          //fshirja eshte kryer me sukses
          this.AddNotification("Permission deleted.", NotificationType.SUCCESS);
      }
      return RedirectToAction("ManagePermissions");

    }
  }
}