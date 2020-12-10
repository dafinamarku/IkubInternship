using IkubInternship.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.ServiceContracts
{
  public interface IEventService
  {
    MultiResult<Event> GetEvents();
    Result<Event> GetEventById(int id);
    Result<bool> InsertEvent(Event e);
    Result<bool> UpdateEvent(Event e);
    Result<bool> DeleteEvent(int id);
    MultiResult<Event> GetVacations(int year);
  }
}
