using IkubInternship.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.RepositoryContracts
{
  public interface IDepartamentRepository
  {
    List<Departament> GetDepartaments();
    bool InsertDepartament(Departament d);
    bool UpdateDepartament(Departament d);
    bool DeleteDepartament(Departament d);
    List<Departament> AvailableParentsFor(int depId);
    Departament GetDepartamentById(int id);
    string SupervisorId(int depId);
    string SupervisorName(int depId);
    List<ApplicationUser> EmployeesOfDepartament(int depId);
    bool HasSupervisor(int depId);
    bool CanUserBeSupervisorOfDepartament(string uid, int depId);
    void MarkAsSupervisorOfDepartament(string uid, int depd);
  }
}
