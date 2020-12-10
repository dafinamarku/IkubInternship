using IkubInternship.DomainModels.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.RepositoryContracts
{
  public interface IReportsRepository
  {
    Dictionary<string, int> NrOfPermissionsForeachStatus();
    List<EmployeePermission> SupervisorsPermissions();
    List<EmployeePermission> EmployeesPermissions(string supervisorId);
    List<PermissionReportViewModel> HrEmployeesPermissions(string depName, DateTime? fromDate, DateTime? toDate, string employeeName);
  }
}
