using IkubInternship.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.RepositoryContracts
{
  public interface IEventRepository
  {
    void InsertEvent(Event e);
    bool UpdateEvent(Event e);
    bool DeleteEvent(int id);
    Event GetEventById(int id);
    List<Event> GetEvents();
    List<Event> GetVacations(int year);
  }
}
