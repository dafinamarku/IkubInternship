using IkubInternship.DataLayer;
using IkubInternship.DomainModels;
using IkubInternship.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.RepositoryLayer
{
  public class PermissionsRepository:IPermissionsRepository
  {
    ProjectDbContext db;

    public PermissionsRepository()
    {
      this.db = new ProjectDbContext();
    }


    public ApplicationUser GetUser(string uid)
    {
      return db.Users.Where(x => x.Id == uid).FirstOrDefault();
    }


    public Permission GetPermissionById(int id)
    {
      return db.Permissions.Where(x => x.PermissionId == id).FirstOrDefault();
    }
    public List<Permission> GetPermissionsForHr()
    {
      //liste me id-te e pergjegjesve te departamenteve
      List<string> supervisorIds = db.Users.Where(x => x.isSupervisor == true && x.DeleteStatus==false).Select(x => x.Id).ToList();
      //kthehet lista e lejeve qe jane kerkuar nga pergjegjesit e departamenteve
      return db.Permissions.Where(x => supervisorIds.Contains(x.EmployeeId)).ToList();
    }

    public List<Permission> GetPermissionsForSupervisor(string supervisorId)
    {
      ApplicationUser theSupervisor = db.Users.Where(x => x.Id == supervisorId && x.isSupervisor==true).FirstOrDefault();

      //lista e id te departamenteve bije te departamentit te pergjegjesit dhe qe nuk kane pergjegjes
      List<int> depIds = db.Departaments
                    .Where(x => x.ParentDepId == theSupervisor.DepId && x.Employees.Any(e => e.isSupervisor==true) == false)
                    .Select(x => x.DepartamentId).ToList();
      //Id-te e punonjesve te dep qe drejton supervizori
      List<string> employeesId = new List<string>();
      if (depIds.Count == 0)
        employeesId = db.Users.Where(x => x.DepId == theSupervisor.DepId && x.Id != theSupervisor.Id && x.DeleteStatus==false)
                      .Select(x => x.Id).ToList();
      else
        employeesId = db.Users.Where(x => (x.DepId == theSupervisor.DepId && x.Id != theSupervisor.Id && x.DeleteStatus == false)
                                      || (depIds.Contains((int)x.DepId) == true && x.DeleteStatus == false)).Select(x => x.Id).ToList();

      //lejet e kerkuara nga punonjes te departamentit
      return db.Permissions.Where(x => employeesId.Contains(x.EmployeeId)).ToList();

    }

    public List<Permission> GetPermissionsForEmployee(string employeeId)
    {
      return db.Permissions.Where(x => x.EmployeeId == employeeId && x.Status!="Canceled").ToList();
    }

    //ndalohet qe nje perdorues te kerkoje leje per ndonje date qe tashme ka kerkuar leje
    public bool InsertPermission(Permission p)
    {
      bool hasAskedThisPermissionBefore = db.Database.SqlQuery<bool>(@"select dbo.HasAskedPermissionBefore(@date,@employeeId)", new SqlParameter("@date", p.PermissionDate), new SqlParameter("@employeeId", p.EmployeeId)).First();
      if (hasAskedThisPermissionBefore==true)
        return false;
      else
      {
        db.Permissions.Add(p);
        var askedBy = db.Users.Where(x => x.Id == p.EmployeeId).FirstOrDefault();
        askedBy.RemainingPermissions--;
        db.SaveChanges();
        return true;
      }
     
    }

    public bool UpdatePermission(Permission p)
    {
      var currentPermission = db.Permissions.Where(x => x.PermissionId == p.PermissionId).FirstOrDefault();
     
      if (currentPermission != null)
      {
        //ne kete rast po refuzohet ose anulohet nje kerkese dhe nr i lejeve te mbeturate punonjesit qe ka bere kerkesen do te rritet me 1
        if (p.Status == "Refused" || p.Status=="Canceled")
        {
          var employee = db.Users.Where(x => x.Id == currentPermission.EmployeeId).FirstOrDefault();
          employee.RemainingPermissions++;
        }
        //ndalohet qe nje perdorues te kerkoje leje per data, per te cilat tashme ka kerkuar leje
        var existingPermissions = db.Permissions.Where(x => x.EmployeeId == p.EmployeeId &&
                                    x.PermissionDate==p.PermissionDate && x.Status!="Canceled").ToList();
        if (existingPermissions.Count() > 1)
          return false;
        
        currentPermission.PermissionDate = p.PermissionDate;
        currentPermission.ReasonForAsking = p.ReasonForAsking;
        currentPermission.ReasonForRefusal = p.ReasonForRefusal;
        currentPermission.Status = p.Status;
        db.SaveChanges();
        return true;
      }
      return false;
    }

    public bool DeletePermission(int id)
    {
      var permissionToDelete = db.Permissions.Where(x => x.PermissionId == id).FirstOrDefault();
      if (permissionToDelete != null)
      {
        db.Permissions.Remove(permissionToDelete);
        db.SaveChanges();
        return true;
      }
      return false;
    }

    public void SetNrOfPermissionsPerYear(int nr)
    {
      int currentYear = DateTime.Now.Year;
      var currentPermissions = db.NrPermissionsPerYear.Where(x => x.Year == currentYear).FirstOrDefault();
    
      var roleId = db.Roles.Where(x => x.Name == "Employee").Select(x => x.Id).FirstOrDefault();
      var employees = db.Users.Where(x => x.Roles.Any(r => r.RoleId == roleId) == true).ToList();
      if (currentPermissions == null) //nr i lejeve percaktohet per here te pare
      {
        PermissionsPerYear newNrPermissions = new PermissionsPerYear()
        {
          NrOfPermissions = nr,
          Year = currentYear
        };
        db.NrPermissionsPerYear.Add(newNrPermissions);
        //mbishkruajme nr e lejeve te mbetura per cdo punonjes
        foreach (var emp in employees)
        {
          emp.RemainingPermissions = nr;
        }
        db.SaveChanges();
      }
      else //ne kete rast po editohet nr i lejeve
      {
        var currentNrOfPermissions = currentPermissions.NrOfPermissions;
        var dif = nr - currentNrOfPermissions;
        if (dif != 0)
        {
          currentPermissions.NrOfPermissions = nr;
          foreach(var emp in employees)
          {
            //shembull: nr lejeve vjetore per 2020 eshte 4 dhe HR do ta editoje dhe ta beje 2 (nr=2)=>(dif= (-2) )
            //ndonje punonjes mund te kete konsumuar 3 leje (ka RemainingPermissions=1)...nqs do ti shtonim dif
            //nr te lejeve te mbetura te tij atehere do te dilnin negative (per kete arsye behen 0)
            if (emp.RemainingPermissions + dif < 0)
              emp.RemainingPermissions = 0;
            else
              emp.RemainingPermissions += dif;
          }
          db.SaveChanges();
        }
      }
    }

    public PermissionsPerYear GetPermissionsPerYear(int year)
    {
      return db.NrPermissionsPerYear.Where(x => x.Year == year).FirstOrDefault();
    }

    public bool IsVacation(DateTime date)
    {
      bool vacation = db.Database.SqlQuery<bool>(@"select dbo.IsVacation(@theDate)", new SqlParameter("@theDate", date)).First(); 
      if (vacation == false)
        return false;
      else
        return true;
    }

    public bool HasSupervisor(int depId)
    {
      var supervisor = db.Users.Where(x => x.DepId == depId && x.isSupervisor == true).FirstOrDefault();
      if (supervisor == null)
        return false;
      else
        return true;
    }
  }
}
