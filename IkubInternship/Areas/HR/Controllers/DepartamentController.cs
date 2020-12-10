using System.Runtime.Caching;
using System.IO;
using IkubInternship.DomainModels;
using IkubInternship.Extensions;
using IkubInternship.ServiceContracts;
using IkubInternship.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.HtmlControls;
using IkubInternship.Cache;

namespace IkubInternship.Areas.HR.Controllers
{
    [Authorize(Roles ="HR")]
    public class DepartamentController : Controller
    {
    IDepartamentService depService;
    ObjectCache cache = MemoryCache.Default;
    DepartamentCache depCache;

    public DepartamentController(IDepartamentService ds)
    {
      this.depService = ds;
      depCache = new DepartamentCache();
    }
    // GET: Departament
    public ActionResult Departaments()
    {
      List<Departament> model = cache["departaments"] as List<Departament>;
      if (model == null)
      {
        model = depService.GetDepartaments().ReturnValue;
        depCache.UpdateCache(model);
      }
      ViewBag.RenderDepartaments = this.RenderDepartaments(model).ToString();
      return View();
    }

    public StringBuilder RenderDepartaments(List<Departament> departaments)
    {
      if (departaments == null)
        return null;
      var ul = new StringBuilder();
      ul.Append("<ul class='list-group'>");
      foreach (var dep in departaments)
      {
        if (dep.ParentDepId == null)
        {
          ul.Append("<li class='list-group-item'>");
          ul.Append("<div>" + dep.Name +
            "<div class='float-right'>" +
            "<a href='/HR/Departament/Edit/" + dep.DepartamentId.ToString() + "'>Edit</a>&nbsp;&nbsp;" +
            "<a href='/HR/Departament/Details/" + dep.DepartamentId.ToString() + "'>Details</a>&nbsp;&nbsp;" +
            "  <button type='button' onclick=\"AddFormAction('/HR/Departament/Delete/" + dep.DepartamentId + "')\" class='btn btn-link bg-transparent' data-toggle='modal' data-target='#deleteDepartamentModal'>Delete</button> " +
            "</div></div><br />");
          ul.Append(AddChildDepartaments(departaments, dep));

          ul.Append("</li>");
        }
       
      }
      ul.Append("</ul>");
      return ul;
    }
    public StringBuilder AddChildDepartaments(List<Departament> deps, Departament parentDep)
    {
      if (deps == null)
        return null;
      var childs = deps.Where(x => x.ParentDepId == parentDep.DepartamentId).ToList();
      var ul = new StringBuilder();
      ul.Append("<ul class='list-group-collapse'>");
      foreach (var dep in childs)
      {
        ul.Append("<li class='list-group-item'>");
        ul.Append("<div>" + dep.Name +
          "<div class='float-right'>" +
          "<a href='/HR/Departament/Edit/" + dep.DepartamentId.ToString() + "'>Edit</a>&nbsp;&nbsp;" +
          "<a href='/HR/Departament/Details/" + dep.DepartamentId.ToString() + "'>Details</a>&nbsp;&nbsp;" +
          "  <button type='button' onclick=\"AddFormAction('/HR/Departament/Delete/" + dep.DepartamentId + "')\" class='btn btn-link bg-transparent' data-toggle='modal' data-target='#deleteDepartamentModal'>Delete</button> " +
          "</div></div><br />");
        var children = childs.Where(x => x.ParentDepId == dep.DepartamentId);
        if (children != null)
        {
          ul.Append(AddChildDepartaments(deps, dep));
        }

        ul.Append("</li>");
      }
      ul.Append("</ul>");
      return ul;
    }
    public ActionResult Create()
    {
      List<Departament> parentList = depService.GetDepartaments().ReturnValue;
      parentList.Insert(0, new Departament { Name = "No Parent Departament", DepartamentId = -1 });
      ViewBag.ParentDepId = new SelectList(parentList, "DepartamentId", "Name");
      return View();
    }

    [HttpPost]
    public ActionResult Create([Bind(Include = "DepartamentId, Name,ParentDepId")]Departament d)
    {
      if (ModelState.IsValid)
      {
        if (d.ParentDepId == -1)
          d.ParentDepId = null;
        var inserted = depService.InsertDepartament(d);
        //departamenti nuk eshte shtuar per shkak se ekziston nje kategori tjeter me te njejtin emer
        if (inserted.HasError)
        {
          ModelState.AddModelError("Err", inserted.MessageResult);
          List<Departament> parentList = depService.GetDepartaments().ReturnValue;
          parentList.Insert(0, new Departament { Name = "No Parent Departament", DepartamentId = -1 });
          ViewBag.ParentDepId = new SelectList(parentList, "DepartamentId", "Name",d.ParentDep);
          return View(d);
        }
        else
        {
          this.AddNotification("Created " + d.Name + " Departament", NotificationType.SUCCESS);
          //updateojme cache-ne
          List<Departament> model = cache["departaments"] as List<Departament>;
          if (model == null)
          {
            var deps = depService.GetDepartaments().ReturnValue;
            depCache.UpdateCache(deps);
          }
          else
          {
            //nqs parametri i 3 do te ishte true ateher do te hiqej rekordi nga cache
            depCache.AddOrRemoveEntryFromCache(d, model, false);
          }
          //cache.Add("departaments", d, DateTimeOffset.Now.AddSeconds(60.0));
          return RedirectToAction("Departaments");
        }
      }
      else
      {
        List<Departament> parentList = depService.GetDepartaments().ReturnValue;
        parentList.Insert(0, new Departament { Name = "No Parent Departament", DepartamentId = -1 });
        ViewBag.ParentDepId = new SelectList(parentList, "DepartamentId", "Name", d.ParentDep);
        return View(d);
      }
    }

    public ActionResult Edit(int id)
    {
      List<Departament> model = cache["departaments"] as List<Departament>;
      Departament depToUpdate;
      if (model == null)
      {
        model = depService.GetDepartaments().ReturnValue;
        depCache.UpdateCache(model);
        depToUpdate=depCache.GetDepartamentFromCache(id);
      }
      else //marrim dep nga cache
      {
        depToUpdate = depCache.GetDepartamentFromCache(id);
      }
      if (depToUpdate == null)
        return new HttpNotFoundResult();
      List<Departament> parentList = depService.AvailableParentsFor(id).ReturnValue;
      parentList.Insert(0, new Departament { Name = "No Parent Departament", DepartamentId = -1 });
      ViewBag.ParentDepId = new SelectList(parentList, "DepartamentId", "Name", depToUpdate.ParentDep);
      return View(depToUpdate);
    }

    [HttpPost]
    public ActionResult Edit([Bind(Include ="DepartamentId,Name,ParentDepId")]Departament depToEdit)
    {
      if (ModelState.IsValid)
      {
        if (depToEdit.DepartamentId == depToEdit.ParentDepId)
        {
          ModelState.AddModelError("Err", "A departament can not have itself as a parent.");
          List<Departament> parentList = depService.AvailableParentsFor(depToEdit.DepartamentId).ReturnValue;
          parentList.Insert(0, new Departament { Name = "No Parent Departament", DepartamentId = -1 });
          ViewBag.ParentDepId = new SelectList(parentList, "DepartamentId", "Name", depToEdit.ParentDep);
          return View(depToEdit);
        }
        if (depToEdit.ParentDepId == -1)
          depToEdit.ParentDepId = null;
        var editResult=depService.UpdateDepartament(depToEdit);
        if (editResult.HasError)
        {
          ModelState.AddModelError("Err", editResult.MessageResult);
          List<Departament> parentList = depService.AvailableParentsFor(depToEdit.DepartamentId).ReturnValue;
          parentList.Insert(0, new Departament { Name = "No Parent Departament", DepartamentId = -1 });
          ViewBag.ParentDepId = new SelectList(parentList, "DepartamentId", "Name", depToEdit.ParentDep);
          return View(depToEdit);
        }
        else
        {
          this.AddNotification("Departament was edited.", NotificationType.SUCCESS);
          //updateojme cache
          List<Departament> model = cache["departaments"] as List<Departament>;
          if (model == null)
          {
            model = depService.GetDepartaments().ReturnValue;
            depCache.UpdateCache(model);
          }
          else //modifikojme dep e edituar 
          {
            depCache.UpdateChacheRecord(depToEdit, model);
          }
          return RedirectToAction("Departaments");
        }
      }
      else
      {
        List<Departament> parentList = depService.AvailableParentsFor(depToEdit.DepartamentId).ReturnValue;
        parentList.Insert(0, new Departament { Name = "No Parent Departament", DepartamentId = -1 });
        ViewBag.ParentDepId = new SelectList(parentList, "DepartamentId", "Name", depToEdit.ParentDep);
        return View(depToEdit);
      }
    }

    public ActionResult Details(int id)
    {
      List<Departament> model = cache["departaments"] as List<Departament>;
      Departament departament;
      if (model == null)
      {
        model = depService.GetDepartaments().ReturnValue;
        depCache.UpdateCache(model);
        departament = depCache.GetDepartamentFromCache(id);
      }
      else //marrim dep nga cache
      {
        departament = depCache.GetDepartamentFromCache(id);
      }
      if (departament == null)
        return new HttpNotFoundResult();
      else
      {
        string supervisorId = depService.SupervisorId(id);
        if (String.IsNullOrEmpty(supervisorId))
          ViewBag.HasSupervisor = false;
        else
        {
          ViewBag.HasSupervisor = true;
          ViewBag.SupervisorName = depService.SupervisorName(id);
        }
          
        return View(departament);
      }  
    }

    [HttpPost]
    public ActionResult Delete(int id)
    {
      List<Departament> model = cache["departaments"] as List<Departament>;
      Departament depToDelete;
      if (model == null)
      {
        model = depService.GetDepartaments().ReturnValue;
        depCache.UpdateCache(model);
        depToDelete = depCache.GetDepartamentFromCache(id);
      }
      else //marrim dep nga cache
      {
        depToDelete = depCache.GetDepartamentFromCache(id);
      }
      if (depToDelete == null)
        return new HttpNotFoundResult();
      else
      {
        var deletion = depService.DeleteDepartament(depToDelete);
        if (!deletion.HasError)
        {
          this.AddNotification("Departament was deleted", NotificationType.SUCCESS);
          //pas fshirjes behet updateimi i cache
          if (model == null)
          {
            model= depService.GetDepartaments().ReturnValue;
            depCache.UpdateCache(model);
          }
          else
          {
            depCache.AddOrRemoveEntryFromCache(depToDelete, model, true);
          }
        }
        else
          this.AddNotification(deletion.MessageResult, NotificationType.ERROR);
        return RedirectToAction("Departaments");
      }
    }

    public ActionResult EmployeesOfDepartament(int id)
    {
      MultiResult<ApplicationUser> result = depService.EmployeesOfDepartament(id);
      if (result.HasError)
      {
        this.AddNotification(result.MessageResult, NotificationType.ERROR);
        return RedirectToAction("Departaments");
      }
      ViewBag.DepName = depService.GetDepartamentById(id).ReturnValue.Name;
      ViewBag.DepId = id;
      return View(result.ReturnValue);
    }

    public ActionResult PickupSupervisorForDepartament(int id)
    {
      var dep=depService.GetDepartamentById(id).ReturnValue;
      if (dep == null)
      {
        this.AddNotification("There is no departament with Id:" + id.ToString(), NotificationType.ERROR);
        return RedirectToAction("Departaments");
      }
      else
      {
          string supervisorId = depService.SupervisorId(id);
          PickSupervisorViewModel model = new PickSupervisorViewModel() { EmployeesOfDepartament = new List<Employee>() };
          var employees = depService.EmployeesOfDepartament(id).ReturnValue;
        if (string.IsNullOrEmpty(supervisorId))
        {
          foreach (var e in employees)
          {
            model.EmployeesOfDepartament.Add(new Employee { FullName = e.Name + " " + e.LastName, Id = e.Id, IsSelected = false });
          }
        }
        else
        {
          foreach(var e in employees)
          {
            if (e.Id == supervisorId)
              model.EmployeesOfDepartament.Add(new Employee { FullName = e.Name + " " + e.LastName, Id = e.Id, IsSelected = true });
            else
              model.EmployeesOfDepartament.Add(new Employee { FullName = e.Name + " " + e.LastName, Id = e.Id, IsSelected = false });
          }
        }
          ViewBag.DepartamentName = dep.Name;
          ViewBag.DepartamentId = dep.DepartamentId;
          return View(model); 
      }
    }

    [HttpPost]
    public ActionResult PickupSupervisorForDepartament(PickSupervisorViewModel model,int depId, string depName)
    {
      if (ModelState.IsValid)
      {
        var newSupervisor = model.EmployeesOfDepartament.Where(x => x.IsSelected == true).FirstOrDefault();
        
        if (newSupervisor != null)
        {
          var canBeSupervisor = depService.CanUserBeSupervisorOfDepartament(newSupervisor.Id, depId);
          if (canBeSupervisor.HasError)
            this.AddNotification(canBeSupervisor.MessageResult, NotificationType.ERROR);
          else
          {
            var result = depService.MarkAsSupervisorOfDepartament(newSupervisor.Id, depId);
            if (result.HasError == false)
            {
              this.AddNotification("Supervisor changed.", NotificationType.SUCCESS);
              return RedirectToAction("Details", new { @id = depId });
            }
            else
              this.AddNotification(result.MessageResult, NotificationType.ERROR);
          }
        }
        else
        {
          //ne kete rast departamenti do te lihet pa pergjegjes
          var result = depService.MarkAsSupervisorOfDepartament(string.Empty, depId);
          if (result.HasError == false)
          {
            this.AddNotification("Departament now has no supervisor.", NotificationType.SUCCESS);
            return RedirectToAction("Details", new { @id = depId });
          }
          else
            this.AddNotification(result.MessageResult, NotificationType.ERROR);
        }
      }
      ViewBag.DepartamentName = depName;
      ViewBag.DepartamentId = depId;
      return View(model);
    }
  }
}