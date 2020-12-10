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
  public class EventService:IEventService
  {
    IEventRepository repository;
    ExceptionDbLogger excDbLogger;

    private static readonly log4net.ILog log
    = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public EventService(IEventRepository rep)
    {
      this.repository = rep;
      this.excDbLogger = new ExceptionDbLogger();
    }

    public MultiResult<Event> GetEvents()
    {
      try
      {
        var res = repository.GetEvents();
        return new MultiResult<Event>(res, false, String.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        excDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new MultiResult<Event>(null, true, ex);
      }
    }

    public Result<Event> GetEventById(int id)
    {
      try
      {
        var result = repository.GetEventById(id);
        if (result == null)
        {
          log.Error("Tried to access a non existing departament.");
          excDbLogger.InsertDbException("Tried to access a non existing departament.", DateTime.Now);
          return new Result<Event>(null, true, "There is no event with Id=" + id.ToString());
        }
        return new Result<Event>(result, false, String.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        excDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<Event>(null, true, ex);
      }
    }

    public Result<bool> InsertEvent(Event e)
    {
      try
      {
        repository.InsertEvent(e);
        return new Result<bool>(true, false, String.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        excDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    public Result<bool> UpdateEvent(Event e)
    {
      try
      {
        var result = repository.UpdateEvent(e);
        if (result == true)
          return new Result<bool>(true, false, String.Empty);
        else
        {
          //nje event mund te modifikohet vtm nqs ai ekziston
          string mess = "Tried to update an event that does not exist.";
          log.Error(mess);
          excDbLogger.InsertDbException(mess, DateTime.Now);
          return new Result<bool>(false, true, mess);
        }
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        excDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    public Result<bool> DeleteEvent(int id)
    {
      try
      {
        var result = repository.DeleteEvent(id);
        if (result == true)
          return new Result<bool>(true, false, string.Empty);
        else
        {
          string mess = "Tried to delete a non existing event.";
          log.Error(mess);
          excDbLogger.InsertDbException(mess, DateTime.Now);
          return new Result<bool>(false, true, mess);
        }
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        excDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    public MultiResult<Event> GetVacations(int year)
    {
      try
      {
        var list = repository.GetVacations(year);
        return new MultiResult<Event>(list, false, string.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        excDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new MultiResult<Event>(null, true, ex);
      }
    }

  }
}
