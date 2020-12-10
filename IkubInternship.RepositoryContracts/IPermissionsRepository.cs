using IkubInternship.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.RepositoryContracts
{
  public interface IPermissionsRepository
  {
    ApplicationUser GetUser(string uid);
    Permission GetPermissionById(int id);
    List<Permission> GetPermissionsForHr();
    List<Permission> GetPermissionsForSupervisor(string supervisorId);
    List<Permission> GetPermissionsForEmployee(string employeeId);
    bool InsertPermission(Permission p);
    bool UpdatePermission(Permission p);
    bool DeletePermission(int id);
    void SetNrOfPermissionsPerYear(int nr);
    PermissionsPerYear GetPermissionsPerYear(int year);
    bool IsVacation(DateTime date);
    bool HasSupervisor(int depId);

  }
}
