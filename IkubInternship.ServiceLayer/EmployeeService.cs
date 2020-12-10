using IkubInternship.DomainModels;
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
  public class EmployeeService:IEmployeeService
  {
    IEmployeeRepository repository;
    ExceptionDbLogger exDbLogger;

    private static readonly log4net.ILog log
   = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public EmployeeService(IEmployeeRepository r)
    {
      exDbLogger = new ExceptionDbLogger();
      repository = r;
    }

    public MultiResult<ApplicationUser> GetEmployees(bool areDeleted)
    {
      try
      {
        var employees= repository.GetEmployees(areDeleted);
        return new MultiResult<ApplicationUser>(employees, false, string.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new MultiResult<ApplicationUser>(null, true, ex);
      }
    }

    public async Task<bool> InsertEmployee(ApplicationUser e)
    {
      return await repository.InsertEmployee(e);
    }

    public Result<bool> UpdateEmployee(ApplicationUser e)
    {
      try
      {
        var updated = repository.UpdateEmployee(e);
        if (updated == true)
          return new Result<bool>(true, false, string.Empty);
        return new Result<bool>(false, true, "There is already an employee with the same name or Email.");
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    public Result<bool> DeleteOrRestoreEmployee(string uid)
    {
      try
      {
        var result = repository.DeleteOrRestoreEmployee(uid);
        if (result)
          return new Result<bool>(result, false, string.Empty);
        var msg = "Tried to Delete OR Restore a non existing employee";
        log.Error(msg);
        exDbLogger.InsertDbException(msg, DateTime.Now);
        return new Result<bool>(result, true, msg);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }


    //ne rast se punonjesi eshte fshire heren e pare, pra DeleteStatus i tij eshte bere true
    //ath ai mund te fshihet perfundimisht nga db
    public Result<bool> CanBeDeletedPermanently(string employeeId)
    {
      try
      {
        ApplicationUser employee = repository.GetEmployeeById(employeeId);
        if (employee != null)
          return new Result<bool>(employee.DeleteStatus, false, string.Empty);
        var msg = "Can not delete premanently a non existing employee.";
        log.Error(msg);
        exDbLogger.InsertDbException(msg, DateTime.Now);
        return new Result<bool>(false, true, msg);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    public Result<bool> PermanentlyDeleteEmployee(string uid)
    {
      try
      {
        var deleteResult=repository.PermanentlyDeleteEmployee(uid);
        if (deleteResult)
          return new Result<bool>(true, false, string.Empty);
        return new Result<bool>(false, true, "Can not perform deletion");
      }
      catch (Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }
    public Result<ApplicationUser> GetEmployeeById(string uid)
    {
      try
      {
        var employee=repository.GetEmployeeById(uid);
        if (employee != null)
          return new Result<ApplicationUser>(employee, false, string.Empty);
        var msg = "Tried to access a non existing employee";
        log.Error(msg);
        exDbLogger.InsertDbException(msg, DateTime.Now);
        return new Result<ApplicationUser>(null, true, msg);
      }
      catch (Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<ApplicationUser>(null, true, ex);
      }
    }



    public MultiResult<ApplicationUser> GetLockedOutEmployees()
    {
      try
      {
        var result = repository.GetLockedOutEmployees();
        return new MultiResult<ApplicationUser>(result, false, string.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new MultiResult<ApplicationUser>(null, true, ex);
      }
    }

    public MultiResult<ApplicationUser> GetEmployeesForSupervisor(string supervisorId)
    {
      try
      {
        var result = repository.GetEmployeesForSupervisor(supervisorId);
        return new MultiResult<ApplicationUser>(result, false, string.Empty);
      }
      catch (Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new MultiResult<ApplicationUser>(null, true, ex);
      }
    }
  }
}
