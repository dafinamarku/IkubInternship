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
  public class DepartamentService:IDepartamentService
  {
    IDepartamentRepository repository;
    ExceptionDbLogger exDbLogger;

    private static readonly log4net.ILog log
    = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public DepartamentService(IDepartamentRepository r)
    {
      this.repository = r;
      this.exDbLogger = new ExceptionDbLogger();
    }

    public MultiResult<Departament> GetDepartaments()
    {
      try
      {
        var deps=repository.GetDepartaments();
        return new MultiResult<Departament>(deps, false, string.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new MultiResult<Departament>(null, true, ex);
      }
    }

    public Result<bool> InsertDepartament(Departament d)
    {
      try
      {
        var result=repository.InsertDepartament(d);
        if (result)
          return new Result<bool>(true, false, string.Empty);
        var msg = "Triend to insert a deprtament that already exists";
        log.Error(msg);
        exDbLogger.InsertDbException(msg, DateTime.Now);
        return new Result<bool>(false, true, "This departament already exists.");
      }
      catch (Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    public Result<bool> UpdateDepartament(Departament d)
    {
      try
      {
        var result=repository.UpdateDepartament(d);
        if (result)
          return new Result<bool>(true, false, string.Empty);
        var msg = "Tried to update a deprtament by setting a Departament name that already exists.";
        log.Error(msg);
        exDbLogger.InsertDbException(msg, DateTime.Now);
        return new Result<bool>(result, true, "There is already a departament with the same name.");
      }
      catch (Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    public Result<bool> DeleteDepartament(Departament d)
    {
      try
      {
        var deletion=repository.DeleteDepartament(d);
        if (deletion)
          return new Result<bool>(deletion, false, string.Empty);
        var msg = "Tried to delete a non existing departament.";
        log.Error(msg);
        exDbLogger.InsertDbException(msg, DateTime.Now);
        return new Result<bool>(deletion, true, msg);
      }
      catch (Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    //perdoret kur behet update-imi i dep per te shmangur qe vete dep qe po modifikohet te shfaqet ne 
    //listen e departamenteve qe mund te behen parent te tij
    public MultiResult<Departament> AvailableParentsFor(int depId)
    {
      try
      {
        var availableParents = repository.AvailableParentsFor(depId);
        return new MultiResult<Departament>(availableParents, false, string.Empty);
      }
      catch (Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new MultiResult<Departament>(null, true, ex);
      }
    }

    public Result<Departament> GetDepartamentById(int id)
    {
      try
      {
        var dep=repository.GetDepartamentById(id);
        if (dep != null)
          return new Result<Departament>(dep, false, string.Empty);
        var msg = "Tried to acces a non existing departament";
        log.Error(msg);
        exDbLogger.InsertDbException(msg, DateTime.Now);
        return new Result<Departament>(null, true, msg);
      }
      catch (Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<Departament>(null, true, ex);
      }
    }

    public string SupervisorId(int depId)
    {
      return repository.SupervisorId(depId);
    }

    public string SupervisorName(int depId)
    {
      return repository.SupervisorName(depId);
    }

    public MultiResult<ApplicationUser> EmployeesOfDepartament(int depId)
    {
      try
      {
        if (repository.GetDepartamentById(depId) == null)
        {
          string message = "Tried to access employee list of a non existing departament.";
          log.Error(message);
          exDbLogger.InsertDbException(message, DateTime.Now);
          return new MultiResult<ApplicationUser>(null, true, message);
        }
        else
        {
          return new MultiResult<ApplicationUser>(repository.EmployeesOfDepartament(depId), false, string.Empty);
        }
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new MultiResult<ApplicationUser>(null, true, ex);
      }
    }

    public Result<bool> HasSupervisor(int depId)
    {
      try
      {
        var result = repository.HasSupervisor(depId);
        return new Result<bool>(result, false, string.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    public Result<bool> CanUserBeSupervisorOfDepartament(string uid, int depId)
    {
      try
      {
        bool can = repository.CanUserBeSupervisorOfDepartament(uid, depId);
        if (can)
          return new Result<bool>(true, false, string.Empty);
        else
        {
          string err = "User with Id:" + uid + " can not be supervisor of the departament with Id:" + depId.ToString();
          log.Error(err);
          exDbLogger.InsertDbException(err, DateTime.Now);
          return new Result<bool>(false, true, err);
        }
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    public Result<bool> MarkAsSupervisorOfDepartament(string uid, int depId)
    {
      try
      {
        repository.MarkAsSupervisorOfDepartament(uid, depId);
        return new Result<bool>(true, false, string.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }
  }
}
