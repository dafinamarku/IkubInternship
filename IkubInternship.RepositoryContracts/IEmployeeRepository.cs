using IkubInternship.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.RepositoryContracts
{
  public interface IEmployeeRepository
  {
    List<ApplicationUser> GetEmployees(bool areDeleted);
    Task<bool> InsertEmployee(ApplicationUser e);
    bool UpdateEmployee(ApplicationUser e);
    bool DeleteOrRestoreEmployee(string uid);
    bool PermanentlyDeleteEmployee(string uid);
    ApplicationUser GetEmployeeById(string uid);
    List<ApplicationUser> GetLockedOutEmployees();
    List<ApplicationUser> GetEmployeesForSupervisor(string supervisorId);
  }
}
