using IkubInternship.DomainModels;
using IkubInternship.DomainModels.VM;
using IkubInternship.RepositoryContracts;
using IkubInternship.RepositoryLayer;
using IkubInternship.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.ServiceLayer
{
  public class ReportsService:IReportsService
  {
    IReportsRepository repository;
    ExceptionDbLogger exDbLogger;

    private static readonly log4net.ILog log
    = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public ReportsService(IReportsRepository rep)
    {
      repository = rep;
      exDbLogger = new ExceptionDbLogger();
    }

    public Dictionary<string, int> NrOfPermissionsForeachStatus()
    {
      return repository.NrOfPermissionsForeachStatus();
    }

    public MultiResult<EmployeePermission> SupervisorsPermissions()
    {
      try
      {
        var result = repository.SupervisorsPermissions();
        return new MultiResult<EmployeePermission>(result, false, string.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new MultiResult<EmployeePermission>(null, true, ex);
      }
    }

    public MultiResult<EmployeePermission> EmployeesPermissions(string supervisorId)
    {
      try
      {
        var result = repository.EmployeesPermissions(supervisorId);
        return new MultiResult<EmployeePermission>(result, false, string.Empty);
      }
      catch (Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new MultiResult<EmployeePermission>(null, true, ex);
      }
    }

    public MultiResult<PermissionReportViewModel> HrEmployeesPermissions(string depName, DateTime? fromDate, DateTime? toDate, string employeeName)
    {
      try
      {
        if (fromDate == null)
          fromDate = new DateTime(DateTime.Now.Year, 1, 1);
        if (toDate == null)
          toDate = new DateTime(DateTime.Now.Year, 12, 31);
        var result=repository.HrEmployeesPermissions(depName, fromDate, toDate, employeeName);
        return new MultiResult<PermissionReportViewModel>(result, false, string.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new MultiResult<PermissionReportViewModel>(null, true, ex);
      }
    }
  }
}
