using IkubInternship.DataLayer;
using IkubInternship.DomainModels;
using IkubInternship.RepositoryContracts;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.RepositoryLayer
{
  public class DepartamentRepository:IDepartamentRepository
  {
    ProjectDbContext db;
    ApplicationUserStore userStore;
    ApplicationUserManager userManager;


    public DepartamentRepository()
    {
      db = new ProjectDbContext();
      userStore = new ApplicationUserStore(db);
      userManager = new ApplicationUserManager(userStore);
    }

    public List<Departament> GetDepartaments()
    {
      return db.Departaments.ToList();
    }

    public bool InsertDepartament(Departament d)
    {
      List<Departament> sameNameDep = db.Departaments.Where(x => x.Name == d.Name).ToList();
      if (sameNameDep.Count > 0)
        return false;
      else
      {
        db.Departaments.Add(d);
        db.SaveChanges();
        return true;
      }
    }

    //departamentet duhet te kene emra unik
    public bool UpdateDepartament(Departament d)
    {
      List<Departament> sameNameDep = db.Departaments.Where(x => x.Name == d.Name && x.DepartamentId!=d.DepartamentId).ToList();
      if (sameNameDep.Count >0)
        return false;
      else
      {
        Departament existingDepartament = db.Departaments.Where(x => x.DepartamentId == d.DepartamentId).FirstOrDefault();
        existingDepartament.Name = d.Name;
        existingDepartament.ParentDepId = d.ParentDepId;
        db.SaveChanges();
        return true;
      }
    }

    public bool DeleteDepartament(Departament d)
    {
      Departament existingDepartament = db.Departaments.Where(x => x.DepartamentId == d.DepartamentId).FirstOrDefault();
      if (existingDepartament == null)
        return false;
      else
      {
        //nqs dep qe po fshihet ka prind ath punonjesit e ketij dep do te behen punonjes te dep prind
        //ne te kundert do te fshihen(jo perfundimisht)
        int? futureParentId = existingDepartament.ParentDepId;
        List<ApplicationUser> departamentEmployees = db.Users.Where(x => x.DepId == existingDepartament.DepartamentId).ToList();

        if (futureParentId != null)
        {
          foreach (var employee in departamentEmployees)
          {
            if (employee.isSupervisor == true)
              employee.isSupervisor = false;
            employee.DepId = futureParentId;
          }
        }
        else
        {
          foreach (var employee in departamentEmployees)
          {
            if (employee.isSupervisor == true)
              employee.isSupervisor = false;
            employee.DeleteStatus = true;
            employee.DepId = futureParentId;
          }
        }
       
        db.Departaments.Remove(existingDepartament);
        db.SaveChanges();
        return true;
      }
    }

    public List<Departament> AvailableParentsFor(int depId)
    {
      //nje dep nuk mund te kete si prind vetveten apo ndonje dep bij te tij
      return db.Departaments.Where(x=>x.DepartamentId!=depId && x.ParentDepId!=depId).ToList();
    }

    public Departament GetDepartamentById(int id)
    {
      return db.Departaments.Where(x => x.DepartamentId == id).FirstOrDefault();
    }

    //nqs nje departament ka nje pergjegjes kthehet id e pergjegjegjesit ne te kundert kthehet nje string bosh
    public string SupervisorId(int depId)
    {
      ApplicationUser supervisor= db.Users.Where(x => x.DepId == depId && x.isSupervisor == true).FirstOrDefault();
      if (supervisor != null)
        return supervisor.Id;
      else
        return String.Empty;
    }

    public string SupervisorName(int depId)
    {
      ApplicationUser supervisor = db.Users.Where(x => x.DepId == depId && x.isSupervisor == true).FirstOrDefault();
      if (supervisor != null)
        return supervisor.Name+" "+supervisor.LastName;
      else
        return String.Empty;
    }
    
    public List<ApplicationUser> EmployeesOfDepartament(int depId)
    {
      List<ApplicationUser> employees = db.Users.Where(x => x.DepId == depId && x.DeleteStatus==false).ToList();
      return employees;
     
    }

    public bool HasSupervisor(int depId)
    {
      var supervisor = db.Users.Where(x => x.DepId == depId && x.isSupervisor == true).FirstOrDefault();
      if (supervisor == null)
        return false;
      else
        return true;
    }

    //nje user mund te jete supervisor i nje dep nqs ai vete eshte punonjes i atij dep
    public bool CanUserBeSupervisorOfDepartament(string uid, int depId)
    {
      var employee = db.Users.Where(x => x.Id == uid && x.DepId == depId).FirstOrDefault();
      if (employee == null)
        return false;
      else
        return true;
    }

    public void MarkAsSupervisorOfDepartament(string uid, int depId)
    {
      var oldSupervisor = db.Users.Where(x => x.DepId == depId && x.isSupervisor == true).FirstOrDefault();
      //ne kete rast dep duhet te lihet pa pergjegjes
      if (string.IsNullOrEmpty(uid))
      {
        //ne rast se dep ka nje pergjegjes ai hiqet nga ky rol
        if (oldSupervisor != null)
        {
          oldSupervisor.isSupervisor = false;
          userManager.RemoveFromRole(oldSupervisor.Id, "Supervisor");
          db.SaveChanges();
        }
      }
      else
      {
        //nqs dep nuk ka nje pergjegjes ose pergjegjesi qe po tentohet te caktohet eshte i ndryshem nga ai qe eshte momentalish=>do te caktohet pergjegjesi
        if (oldSupervisor == null || oldSupervisor.Id != uid)
        {
          var newSupervisor = db.Users.Where(x => x.Id == uid && x.DepId == depId).FirstOrDefault();
          if (newSupervisor != null)
          {

            //pergjegjesit te vjeter do ti hiqet roli i Supervisor dhe do ti caktohet pergjegjesit te ri
            if (oldSupervisor != null)
            {
              oldSupervisor.isSupervisor = false;
              userManager.RemoveFromRole(oldSupervisor.Id, "Supervisor");
            }
            newSupervisor.isSupervisor = true;
            userManager.AddToRole(newSupervisor.Id, "Supervisor");
            db.SaveChanges();
          }

        }
      }
    
      
    }
  }
}
