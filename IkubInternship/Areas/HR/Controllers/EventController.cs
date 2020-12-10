using IkubInternship.DomainModels;
using IkubInternship.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IkubInternship.Areas.HR.Controllers
{
    public class EventController : Controller
    {
    IEventService eventService;
    public EventController(IEventService ser)
    {
      this.eventService = ser;
    }

    public ActionResult Index()
    {
      return View();
    }

    public JsonResult GetEvents()
    {
      var result = eventService.GetEvents();
      return new JsonResult { Data = result.ReturnValue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
    }

    [HttpPost]
    public JsonResult SaveOrUpdateEvent(Event e)
    {
      Result<bool> result;
        if (e.EventId > 0)
        {
          //Update the event
          result=eventService.UpdateEvent(e);
        }
        else
        {
          result = eventService.InsertEvent(e);
        }
      if (result.ReturnValue == true) 
         return new JsonResult { Data = new { status = true } };
      else
        return new JsonResult { Data = new { status = false } };
    }

    [HttpPost]
    public JsonResult Delete(int eventID)
    {
      Result<bool> result = eventService.DeleteEvent(eventID);
      //vlera e kthimit eshte true nqs eshte bere fshirja dh false ne te kundert
      bool status = result.ReturnValue;

      return new JsonResult { Data = new { status = status } };
    }
  }
}