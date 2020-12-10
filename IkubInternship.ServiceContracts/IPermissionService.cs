using IkubInternship.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.ServiceContracts
{
  public interface IPermissionService
  {
    MultiResult<Permission> GetPermissionsForHr();
    MultiResult<Permission> GetPermissionsForSupervisor(string supervisorId);
    MultiResult<Permission> GetPermissionsForEmployee(string employeeId);
    Result<Permission> GetPermissionById(int id);
    Result<bool> InsertPermission(Permission p);
    Result<bool> UpdatePermission(Permission p);
    Result<bool> DeletePermission(int id);
    Result<bool> CanBeRefusedOrApprovedBy(string uid, string userRole, int permissionId);
    Result<Permission> CanUserViewDetailsOf(string uid, string userRole, int permissionId);
    Result<bool> SetNrOfPermissionsPerYear(int nr);
    Result<PermissionsPerYear> GetPermissionsPerYear(int year);
    //anulimi mund te behet vtm nga personi qe ka kerkuar lejen
    Result<bool> CanUserCancelPermission(string uid, int permissionId);
    Result<bool> CanUserDeletePermission(string uid, string userRole, int permissionId);
  }
}
