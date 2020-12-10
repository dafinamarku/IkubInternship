using IkubInternship.DataLayer;
using IkubInternship.DomainModels;
using IkubInternship.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.RepositoryLayer
{
  public class EventRepository:IEventRepository
  {
    ProjectDbContext db;

    public EventRepository()
    {
      this.db = new ProjectDbContext();
    }

    public List<Event> GetEvents()
    {
      return db.Events.ToList();
    }

    public Event GetEventById(int id)
    {
      return db.Events.Where(x => x.EventId == id).FirstOrDefault();
    }

    public void InsertEvent(Event e)
    {
      db.Events.Add(e);
      db.SaveChanges();
    }

    public bool UpdateEvent(Event e)
    {
      var currentEvent = this.GetEventById(e.EventId);
      if (currentEvent == null)
        return false;
      currentEvent.Title = e.Title;
      currentEvent.Description = e.Description;
      currentEvent.StartDate = e.StartDate;
      currentEvent.FinishDate = e.FinishDate;
      db.SaveChanges();
      return true;
    }

    public bool DeleteEvent(int id)
    {
      var eventToDelete = this.GetEventById(id);
      if (eventToDelete == null)
        return false;
      else
      {
        db.Events.Remove(eventToDelete);
        db.SaveChanges();
        return true;
      }
    }

    //kthen listen e pushimeve kombetare
    public List<Event> GetVacations(int year)
    {
      return db.Events.Where(x => x.Title.ToUpper() != "DITE PUSHIMI" && x.StartDate.Year==year).ToList();
    }
  }
}
