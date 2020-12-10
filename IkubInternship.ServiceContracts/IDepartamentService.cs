using IkubInternship.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.ServiceContracts
{
  public interface IDepartamentService
  {
    MultiResult<Departament> GetDepartaments();
    Result<bool> InsertDepartament(Departament d);
    Result<bool> UpdateDepartament(Departament d);
    Result<bool> DeleteDepartament(Departament d);
    MultiResult<Departament> AvailableParentsFor(int depId);
    Result<Departament> GetDepartamentById(int id);
    string SupervisorId(int depId);
    string SupervisorName(int depId);
    MultiResult<ApplicationUser> EmployeesOfDepartament(int depId);
    Result<bool> HasSupervisor(int depId);
    Result<bool> CanUserBeSupervisorOfDepartament(string uid, int depId);
    Result<bool> MarkAsSupervisorOfDepartament(string uid, int depId);
  }
}
