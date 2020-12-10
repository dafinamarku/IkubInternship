
using IkubInternship.DataLayer;
using IkubInternship.DomainModels;
using IkubInternship.DomainModels.VM;
using IkubInternship.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.RepositoryLayer
{
  public class ReportsRepository:IReportsRepository
  {
    ProjectDbContext db;

    public ReportsRepository()
    {
      db = new ProjectDbContext();
    }

    //kthen nr e lejeve per cdo status(Asked, Approved, Refused, Canceled)
    public Dictionary<string, int> NrOfPermissionsForeachStatus()
    {
      return db.Permissions.GroupBy(x=> new { x.Status }).ToDictionary(g=>g.Key.Status, g=>g.Count());

    }

    public List<EmployeePermission> SupervisorsPermissions()
    {
      return db.Users.Where(x=>x.isSupervisor==true).GroupJoin(db.Permissions, 
                              u => u.Id, 
                              p => p.EmployeeId,    
                              (u, userPermissions) => new EmployeePermission
                                {
                                FullName = u.Name+" "+u.LastName,
                                Remaining = u.RemainingPermissions,
                                Approved = userPermissions.Count(x => x.Status == "Approved"),
                                Refused = userPermissions.Count(x => x.Status == "Refused"),
                                Asked = userPermissions.Count(x => x.Status == "Asked"),
                                Canceled = userPermissions.Count(x => x.Status == "Canceled")
                              }).ToList();
    }

    //lista e punonjesve te dep dhe dep bije qe nuk kane pergjegjes dhe lejet e tyre
    public List<EmployeePermission> EmployeesPermissions(string supervisorId)
    {
      ApplicationUser theSupervisor = db.Users.Where(x => x.isSupervisor == true && x.Id==supervisorId).FirstOrDefault();
      int depId = (int)theSupervisor.DepId;
      //lista e id te departamenteve bije te departamentit te pergjegjesit dhe qe nuk kane pergjegjes
      List<int> depIds = db.Departaments
                    .Where(x => x.ParentDepId == theSupervisor.DepId && x.Employees.Any(e => e.isSupervisor == true) == false)
                    .Select(x => x.DepartamentId).ToList();
      //Id-te e punonjesve te dep qe drejton supervizori
      var result=new List<EmployeePermission>();
      if (depIds.Count == 0)
        result = db.Users.Where(x => x.DepId == theSupervisor.DepId && x.Id != theSupervisor.Id && x.DeleteStatus == false)
                      .GroupJoin(db.Permissions,
                              u => u.Id,
                              p => p.EmployeeId,
                              (u, userPermissions) => new EmployeePermission
                              {
                                FullName = u.Name + " " + u.LastName,
                                Remaining = u.RemainingPermissions,
                                Approved = userPermissions.Count(x => x.Status == "Approved"),
                                Refused = userPermissions.Count(x => x.Status == "Refused"),
                                Asked = userPermissions.Count(x => x.Status == "Asked"),
                                Canceled = userPermissions.Count(x => x.Status == "Canceled")
                              }).ToList();
      else
        result = db.Users.Where(x => (x.DepId == theSupervisor.DepId && x.Id != theSupervisor.Id && x.DeleteStatus == false)
                                      || (depIds.Contains((int)x.DepId) == true && x.DeleteStatus == false))
                                      .GroupJoin(db.Permissions,
                                          u => u.Id,
                                          p => p.EmployeeId,
                                          (u, userPermissions) => new EmployeePermission
                                          {
                                            FullName = u.Name + " " + u.LastName,
                                            Remaining = u.RemainingPermissions,
                                            Approved = userPermissions.Count(x => x.Status == "Approved"),
                                            Refused = userPermissions.Count(x => x.Status == "Refused"),
                                            Asked = userPermissions.Count(x => x.Status == "Asked"),
                                            Canceled = userPermissions.Count(x => x.Status == "Canceled")
                                          }).ToList();
      return result;
    }

    //kthen nje liste me lejte
    public List<PermissionReportViewModel> HrEmployeesPermissions(string depName, DateTime? fromDate, DateTime? toDate, string employeeName)
    {

      if (string.IsNullOrEmpty(depName) && string.IsNullOrEmpty(employeeName))
        return db.Permissions.Where(x => x.Employee.DeleteStatus==false &&
                        x.PermissionDate >= fromDate && x.PermissionDate <= toDate)
                        .Select(GetViewModel).ToList();

      if(string.IsNullOrEmpty(depName)==false && string.IsNullOrEmpty(employeeName)==false)
        return db.Permissions.Where(x => x.Employee.DeleteStatus == false &&
                      (x.Employee.Name+" "+x.Employee.LastName).ToUpper().Contains(employeeName.ToUpper()) &&
                       x.Employee.EmployeeDep.Name.ToUpper().Contains(depName.ToUpper()) &&
                       x.PermissionDate >= fromDate && x.PermissionDate <= toDate)
                       .Select(GetViewModel).ToList();

      if (string.IsNullOrEmpty(depName) && string.IsNullOrEmpty(employeeName) == false)
        return db.Permissions.Where(x => x.Employee.DeleteStatus == false &&
                       (x.Employee.Name + " " + x.Employee.LastName).ToUpper().Contains(employeeName.ToUpper()) &&
                       x.PermissionDate >= fromDate && x.PermissionDate <= toDate)
                       .Select(GetViewModel).ToList();

     // if (string.IsNullOrEmpty(depName) == false && string.IsNullOrEmpty(employeeName))
        return db.Permissions.Where(x => x.Employee.DeleteStatus == false &&
                       x.Employee.EmployeeDep.Name.ToUpper().Contains(depName.ToUpper()) &&
                       x.PermissionDate >= fromDate && x.PermissionDate <= toDate)
                       .Select(GetViewModel).ToList();
    }

    private PermissionReportViewModel GetViewModel(Permission permisssion) {
      return new PermissionReportViewModel
      {
        FullName = permisssion.Employee.Name + " " + permisssion.Employee.LastName,
        PermissionDate = permisssion.PermissionDate,
        PermissionStatus = permisssion.Status,
        Departament = permisssion.Employee.EmployeeDep.Name
      };
      }
  }
}
