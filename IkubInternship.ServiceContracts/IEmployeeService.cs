using IkubInternship.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.ServiceContracts
{
  public interface IEmployeeService
  {
    MultiResult<ApplicationUser> GetEmployees(bool areDeleted);
    Task<bool> InsertEmployee(ApplicationUser e);
    Result<bool> UpdateEmployee(ApplicationUser e);
    Result<bool> DeleteOrRestoreEmployee(string uid);
    Result<bool> CanBeDeletedPermanently(string employeeId);
    Result<bool> PermanentlyDeleteEmployee(string uid);
    Result<ApplicationUser> GetEmployeeById(string uid);
    MultiResult<ApplicationUser> GetLockedOutEmployees();
    MultiResult<ApplicationUser> GetEmployeesForSupervisor(string supervisorId);
  }
}
