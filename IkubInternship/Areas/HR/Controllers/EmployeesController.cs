using IkubInternship.DataLayer;
using IkubInternship.DomainModels;
using IkubInternship.Extensions;
using IkubInternship.ServiceContracts;
using IkubInternship.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace IkubInternship.Areas.HR.Controllers
{
    [Authorize(Roles ="HR")]
    public class EmployeesController : Controller
    {
      IEmployeeService employeeService;
      ProjectDbContext db;
      public EmployeesController(IEmployeeService serv)
      {
        this.employeeService = serv;
        this.db = new ProjectDbContext();
      }

      public ActionResult AllEmployees()
      {//kthen punjesit qe nuk jane fshire
        var result = employeeService.GetEmployees(false);
        if (result.HasError)
          this.AddNotification(result.MessageResult, NotificationType.ERROR);
          return View(result.ReturnValue);
      }

      public ActionResult Create()
      {
        ViewBag.DepartamentId = new SelectList(db.Departaments, "DepartamentId", "Name");
        return View();
      }

      [HttpPost]
      public async Task<ActionResult> Create(CreateEmployeeViewModel model)
      {
          if (ModelState.IsValid)
          {
            int currentYear = DateTime.Now.Year;
            int permissions = db.NrPermissionsPerYear.Where(x => x.Year == currentYear)
                              .Select(x=>x.NrOfPermissions).FirstOrDefault();
            ApplicationUser newEmployee = new ApplicationUser
            {
              UserName = model.Username,
              Name=model.Name,
              LastName=model.LastName,
              Email = model.Email,
              PasswordHash = Crypto.HashPassword(model.Password),
              DepId=model.DepartamentId,
              RemainingPermissions=permissions,
              isSupervisor=false, 
              DeleteStatus=false

            };
            bool result = await employeeService.InsertEmployee(newEmployee);
            if (result == false)//useri nuk eshte shtuar ne db
            {
              ModelState.AddModelError("My Error", "A user with the same Username OR Email already exists.");
              ViewBag.DepartamentId = new SelectList(db.Departaments, "DepartamentId", "Name", db.Departaments.Where(x=>x.DepartamentId==model.DepartamentId).FirstOrDefault());

              return View(model);
            }
            else
            {
              this.AddNotification("Employee created.", NotificationType.SUCCESS);
              return RedirectToAction("AllEmployees");
            }
          }
          else
          {
            ViewBag.DepartamentId = new SelectList(db.Departaments, "DepartamentId", "Name", db.Departaments.Where(x => x.DepartamentId == model.DepartamentId).FirstOrDefault());
            return View(model);
          }
      }

      public ActionResult Details(string id, int? depId)
      {
        var employeeResult = employeeService.GetEmployeeById(id);
        List<string> roles = new List<string>();
        if (employeeResult.ReturnValue != null)
        {
          if (depId == null)
            ViewBag.depId = null;
          else ViewBag.depId = depId;
          return View(employeeResult.ReturnValue);
        }
        else
          return new HttpNotFoundResult();
      }

      public ActionResult Edit(string id, int? depId)
      {
        ApplicationUser employee = employeeService.GetEmployeeById(id).ReturnValue;
        if (employee == null)
          return new HttpNotFoundResult();
        //ne kete rast punonjesi eshte fshire dhe HR-it duhet ti mohohet e drejta per ta edituar
        if (employeeService.CanBeDeletedPermanently(id).ReturnValue)
        {
          this.AddNotification("Can not perform this action.", NotificationType.ERROR);
          if (depId == null)
            return RedirectToAction("AllEmployees");
          else
            return RedirectToAction("EmployeesOfDepartament", "Departament", new { @id = depId });
        }

        EditEmployeeViewModel model = new EditEmployeeViewModel()
        {
          Name=employee.Name,
          LastName=employee.LastName,
          Email=employee.Email,
          Username=employee.UserName,
          DepartamentId=(int)employee.DepId,
          Id=employee.Id
        };
        //perdoret per te kontrolluar nqs dep eshte edituar ose jo ne metoden Edit(Post)
        //nqs departamenti eshte edituar ath atributi isSupervisor behet false pas editimit
        TempData["DepId"] = employee.DepId;
      if (depId != null)
        //do te sherbeje per ridrejtimin e userit pas submitit te formes post
        ViewBag.depId = depId;
      else ViewBag.depId = "";
        ViewBag.DepartamentId = new SelectList(db.Departaments, "DepartamentId", "Name", db.Departaments.Where(x => x.DepartamentId == employee.DepId).FirstOrDefault());
        return View(model);
        }

      [HttpPost]
      public ActionResult Edit(EditEmployeeViewModel model, int? depId)
      {
        if (ModelState.IsValid)
        {
        //ne kete rast punonjesi eshte fshire dhe HR-it duhet ti mohohet e drejta per ta edituar
        var canBeDeletedPermanently = employeeService.CanBeDeletedPermanently(model.Id);
          if (canBeDeletedPermanently.ReturnValue || canBeDeletedPermanently.HasError)
          {
            this.AddNotification("You can not edit this employee, because he/she is deleted OR it doesn't exist.", NotificationType.ERROR);
            if (depId == null)
              return RedirectToAction("AllEmployees");
            else
              return RedirectToAction("EmployeesOfDepartament", "Departament", new { @id = depId });
          }
          ApplicationUser employeeToEdit = employeeService.GetEmployeeById(model.Id).ReturnValue;
          //nqs fusha e passwordit eshte bosh ai nuk ndryshon
          if (!string.IsNullOrEmpty(model.Password))
          {
            var passwordHash = Crypto.HashPassword(model.Password);
            employeeToEdit.PasswordHash = passwordHash;
          }

          employeeToEdit.Email = model.Email;
          employeeToEdit.UserName = model.Username;
          employeeToEdit.Name = model.Name;
          employeeToEdit.LastName = model.LastName;
          employeeToEdit.DepId = model.DepartamentId;
          //ne kete rast eshte ndryshuar departamenti ku punon punonjesi 
          //per te shmangur perplasje atributi isSupervisor behet false, nje perplasje do te ishte p.sh
          //nje punonjes i dep te marketingut(i cili eshte edhe pergjegjes i ketij dep) kalohet ne nje dep tj, i cili
          //ka nje pergjegjes tj (kjo do te krijonte perplasje pasi nje dep mund te kete vetem 1 pergjegjes)
          if (model.DepartamentId != (int)TempData["DepId"])
          {
            employeeToEdit.isSupervisor = false;
          }
          var updateResult = employeeService.UpdateEmployee(employeeToEdit);
          if (updateResult.HasError)
          {
            ModelState.AddModelError("Err", updateResult.MessageResult);
            ViewBag.DepartamentId = new SelectList(db.Departaments, "DepartamentId", "Name", db.Departaments.Where(x => x.DepartamentId == model.DepartamentId).FirstOrDefault());
            return View(model);
          }
          this.AddNotification("Employee edited.", NotificationType.SUCCESS);
          if (depId == null)
            return RedirectToAction("AllEmployees");
          else
          //ne kete rast ridrejtohet tek lista e punonjesve te dep ku punonjesi ben pjese
            return RedirectToAction("EmployeesOfDepartament", "Departament", new { @id = model.DepartamentId });
        }
        ViewBag.DepartamentId = new SelectList(db.Departaments, "DepartamentId", "Name", db.Departaments.Where(x => x.DepartamentId == model.DepartamentId).FirstOrDefault());
        return View(model);
      }

      [HttpPost]
      public ActionResult Delete(string id, int? depId)
      {
        ApplicationUser employee = employeeService.GetEmployeeById(id).ReturnValue;
       //ne kete rast punonjesi eshte ne listen e punonjesve te fshire
        if (employeeService.CanBeDeletedPermanently(id).ReturnValue)
        {
          this.AddNotification("This employee is already deleted.", NotificationType.ERROR);
        }
        else
        {
          if (employee == null)
            return new HttpNotFoundResult();
          var result = employeeService.DeleteOrRestoreEmployee(id);
          if (!result.HasError)
            this.AddNotification(employee.Name + " " + employee.LastName + " was deleted.", NotificationType.SUCCESS);
          else
            this.AddNotification("Couldn't delete " + employee.Name + " " + employee.LastName, NotificationType.ERROR);
        }
        //ne kete rast metoda po therritet nga lista e te gjithe punonjesve dhe pas fshirjes do te ridrejtohet atje
        if (depId == null)
          return RedirectToAction("AllEmployees");
        else
          return RedirectToAction("EmployeesOfDepartament", "Departament", new { @id = depId });
      }

      public ActionResult DeletedEmployees()
      {
        //kthen punonjesit qe jane fshire
        var result = employeeService.GetEmployees(true);
        if (result.HasError)
          this.AddNotification(result.MessageResult, NotificationType.ERROR);
        DeletedEmployeesViewModel model = new DeletedEmployeesViewModel()
          {
            DeletedEmployees = result.ReturnValue
          };
        return View(model);
      }

      [HttpPost]
      public ActionResult PermanentlyDelete(string id)
      {
        //mund te fshihen perfundimisht vtm ata punonjes qe ekzistojne dhe qe e kane DeleteStatus true
        if (!employeeService.CanBeDeletedPermanently(id).ReturnValue)
        {
          this.AddNotification("Can't perform this action!", NotificationType.ERROR);
        }
        else
        {
          ApplicationUser employee = employeeService.GetEmployeeById(id).ReturnValue;
          var result = employeeService.PermanentlyDeleteEmployee(id);
          if(!result.HasError)
            this.AddNotification(employee.Name + " " + employee.LastName + " was deleted.", NotificationType.SUCCESS);
          else
            this.AddNotification(result.MessageResult, NotificationType.ERROR);
        }

        return RedirectToAction("DeletedEmployees");
      }
    

      public ActionResult Restore(string id)
      {
        //nqs mund te fshihet(perfundimisht) ateher edhe mund te behet restore
        if (employeeService.CanBeDeletedPermanently(id).ReturnValue)
        {
            var employeeToRestore = employeeService.GetEmployeeById(id).ReturnValue;
            //ne kete rast behet resore tek dep qe ka
            if (employeeToRestore.DepId!=null)
            {
              var restoreResult = employeeService.DeleteOrRestoreEmployee(id);
              if (!restoreResult.HasError)
                this.AddNotification(employeeToRestore.Name + " " + employeeToRestore.LastName + " was restored.", NotificationType.SUCCESS);
              else
                this.AddNotification("Couldn't perform the action.", NotificationType.ERROR);
            }
            else
            {
              //ne kete rast ridrejtohet ne nje view ku do te perzgjedh dep ku do ta bej resore
              ViewBag.DepartamentId = new SelectList(db.Departaments, "DepartamentId", "Name");
              return View(employeeToRestore);
            }
          
        }
        else
          this.AddNotification("Action can not be performed (because employe with the specified Id doesn't exist or employee is not deleted)!", NotificationType.ERROR);

        return RedirectToAction("DeletedEmployees");
      }

      [HttpPost]
      public ActionResult Restore(string id, ApplicationUser model)
      {
        //nqs mund te fshihet(perfundimisht) ateher edhe mund te behet restore
        if (employeeService.CanBeDeletedPermanently(id).ReturnValue)
        {
          var employeeToRestore = employeeService.GetEmployeeById(id).ReturnValue;
          employeeToRestore.DepId = model.DepId;
          var updateResult=employeeService.UpdateEmployee(employeeToRestore);
          if (updateResult.HasError)
          {
            this.AddNotification(updateResult.MessageResult, NotificationType.ERROR);
            //ne kete rast ridrejtohet ne nje view ku do te perzgjedh dep ku do ta bej resore
            ViewBag.DepartamentId = new SelectList(db.Departaments, "DepartamentId", "Name");
            return View(model);
          }
          var restoreResult = employeeService.DeleteOrRestoreEmployee(id);
          if (!restoreResult.HasError)
            this.AddNotification(employeeToRestore.Name + " " + employeeToRestore.LastName + " was restored.", NotificationType.SUCCESS);
          else
            this.AddNotification("Couldn't perform the action.", NotificationType.ERROR);
        }
        else
        {
          this.AddNotification("Action can not be performed (because employe with the specified Id doesn't exist or employee is not deleted)!", NotificationType.ERROR);

        }
        return RedirectToAction("DeletedEmployees");
      }

      public ActionResult Cancel(int? depId)
      {
        if (depId == null)
          return RedirectToAction("AllEmployees");
        else
          return RedirectToAction("EmployeesOfDepartament", "Departament", new { @id = depId });
      }

      public ActionResult LockedOutEmployees()
      {
        var result = employeeService.GetLockedOutEmployees();
        if (result.HasError)
          this.AddNotification(result.MessageResult, NotificationType.ERROR);

        return View(result.ReturnValue);
      }

      public ActionResult UnlockEmployee(string id)
      {
        var employee = employeeService.GetEmployeeById(id).ReturnValue;
        //view e kesaj action method nuk mund te aksesohet nese accounti i punonjesit nuk eshte i bllokuar
        if (employee == null||employee.LockoutEndDateUtc<DateTime.Now.AddHours(-1))
          return new HttpNotFoundResult();
        ViewBag.EmployeeId = employee.Id;
        ViewBag.EmployeeName = employee.Name + " " + employee.LastName;
        return View();
      }

      [HttpPost]
      public ActionResult UnlockEmployee(ResetPasswordViewModel model, string id)
      {
        if (ModelState.IsValid)
        {
          var employeeToUnlock = employeeService.GetEmployeeById(id).ReturnValue;
          if (employeeToUnlock == null)
            return new HttpNotFoundResult();
          if (employeeToUnlock == null || employeeToUnlock.LockoutEndDateUtc < DateTime.Now.AddHours(-1))
          {
            ModelState.AddModelError("Error", "Can not unlock a user with Id: " + id.ToString());
            return View();
          }
          employeeToUnlock.LockoutEndDateUtc = null;
          employeeToUnlock.PasswordHash = Crypto.HashPassword(model.NewPassword);
          var updateResult=employeeService.UpdateEmployee(employeeToUnlock);
          if (updateResult.HasError)
          {
            ModelState.AddModelError("Error", updateResult.MessageResult);
            return View();
          }
          this.AddNotification(employeeToUnlock.Name + " " + employeeToUnlock.LastName + " unlocked.", NotificationType.SUCCESS);
          return RedirectToAction("LockedOutEmployees", new { @id = id });
        }
        return View();
      }

    }

 
}