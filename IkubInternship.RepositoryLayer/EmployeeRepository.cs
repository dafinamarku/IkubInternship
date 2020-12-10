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
  public class EmployeeRepository : IEmployeeRepository
  {
    ProjectDbContext db;
    ApplicationUserStore userStore;
    ApplicationUserManager userManager;

    public EmployeeRepository()
    {
      this.db = new ProjectDbContext();
      this.userStore = new ApplicationUserStore(db);
      this.userManager = new ApplicationUserManager(userStore);
    }

    //ky funksion ne varesi te parametrit qe merr(true ose false) kthen perkatesisht ose listen e 
    //punonjesve te fshire ose listen e punjonjesve qe nuk jane fshire
    public List<ApplicationUser> GetEmployees(bool areDeleted)
    {
      var employeeRoleId = db.Roles.Where(x => x.Name == "Employee").Select(x => x.Id).FirstOrDefault();
      var employees = db.Users.Where(x => x.Roles.Any(r => r.RoleId == employeeRoleId) == true && x.DeleteStatus==areDeleted).ToList();

      return employees;
    }

    public async Task<bool> InsertEmployee(ApplicationUser e)
    {
      ApplicationUser sameEmailUser = userManager.FindByEmail(e.Email);
      ApplicationUser sameUname = db.Users.Where(x => x.UserName == e.UserName).FirstOrDefault();
      //nuk lejohen usera me te njejtin email ose username
      if (sameEmailUser != null || sameUname != null)
        return false;
      else
      {
        IdentityResult result = await userManager.CreateAsync(e);
        if (result.Succeeded)
        {
          userManager.AddToRole(e.Id, "Employee");
          return true;
        }
        else
          return false;
      }
    }

    public bool UpdateEmployee(ApplicationUser e)
    {
      List<ApplicationUser> sameEmailOrUnameUsers = db.Users.Where(x => x.Email == e.Email || x.UserName == e.UserName).ToList();
      //nje user nuk mund te kete email ose username te njejte me nje tjeter
      if (sameEmailOrUnameUsers.Count() > 1)
        return false;
      else
      {
        ApplicationUser existingUser = userManager.FindById(e.Id);
        existingUser.UserName = e.UserName;
        existingUser.Email = e.Email;
        existingUser.PasswordHash = e.PasswordHash;
        existingUser.DepId = e.DepId;
        existingUser.Name = e.Name;
        existingUser.LastName = e.LastName;
        existingUser.LockoutEndDateUtc = e.LockoutEndDateUtc;
        IdentityResult result = userManager.Update(existingUser);
        if (result.Succeeded)
          return true;
        else
          return false;
      }
    } 

    //kur nje punonjes fshihet(jo perfundimisht) DeleteStatus behet true, kur behet restore DeleteStatus behet 
    //false
    public bool DeleteOrRestoreEmployee(string uid)
    {
      ApplicationUser existingEmployee = db.Users.Where(x => x.Id == uid).FirstOrDefault();
      if (existingEmployee != null)
      {
        //ne kete rast punonjesi do te fshihet dhe nqs ai eshte pergjegjes duhet ta heqim ate nga ky rol
        if (existingEmployee.DeleteStatus == false && existingEmployee.isSupervisor==true)
        {
          existingEmployee.isSupervisor = false;
          userManager.RemoveFromRole(existingEmployee.Id, "Supervisor");
        }
        existingEmployee.DeleteStatus = !existingEmployee.DeleteStatus;
        db.SaveChanges();
        return true;
      }
      else
        return false;
    }

    //punonjesi fshihet perfundimisht nga db
    public bool PermanentlyDeleteEmployee(string uid)
    {
      ApplicationUser existingEmployee = db.Users.Where(x => x.Id == uid).FirstOrDefault();
      IdentityResult result = userManager.Delete(existingEmployee);
      if (result.Succeeded)
        return true;
      else
        return false;
    }

    public ApplicationUser GetEmployeeById(string uid)
    {
      return db.Users.Where(x => x.Id == uid).FirstOrDefault();
    }



    public List<ApplicationUser> GetLockedOutEmployees()
    {
      //zbritet nje ore sepse kur behet bllokimi i accountit ne db koha(e zhbllokimit) eshte 1 ore mbrapa se
      //koha aktuale
      var currentDate = DateTime.Now.AddHours(-1);
      return db.Users.Where(x => x.LockoutEndDateUtc > currentDate).ToList();
    }

    public List<ApplicationUser> GetEmployeesForSupervisor(string supervisorId)
    {
      ApplicationUser theSupervisor = db.Users.Where(x => x.Id == supervisorId && x.isSupervisor == true).FirstOrDefault();

      //lista e id te departamenteve bije te departamentit te pergjegjesit dhe qe nuk kane pergjegjes
      List<int> depIds = db.Departaments
                    .Where(x => x.ParentDepId == theSupervisor.DepId && x.Employees.Any(e => e.isSupervisor) == false)
                    .Select(x => x.DepartamentId).ToList();
      //punonjesit qe drejton supervizori
      if (depIds.Count == 0)
        return db.Users.Where(x => x.DepId == theSupervisor.DepId && x.Id != theSupervisor.Id && x.DeleteStatus == false)
                      .ToList();
      else
        return db.Users.Where(x => (x.DepId == theSupervisor.DepId && x.Id != theSupervisor.Id && x.DeleteStatus == false)
                                      || (depIds.Contains((int)x.DepId) == true && x.DeleteStatus == false)).ToList();
    }
  }
}
