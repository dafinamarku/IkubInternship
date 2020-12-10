using IkubInternship.DomainModels;
using IkubInternship.DomainModels.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.ServiceContracts
{
  public interface IReportsService
  {
    Dictionary<string, int> NrOfPermissionsForeachStatus();
    MultiResult<EmployeePermission> SupervisorsPermissions();
    MultiResult<EmployeePermission> EmployeesPermissions(string supervisorId);
    MultiResult<PermissionReportViewModel> HrEmployeesPermissions(string depName, DateTime? fromDate, DateTime? toDate, string employeeName);
  }
}
