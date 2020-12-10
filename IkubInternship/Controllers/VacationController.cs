using IkubInternship.Extensions;
using IkubInternship.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IkubInternship.Controllers
{
    public class VacationController : Controller
    {
        IEventService eventService;
        public VacationController(IEventService serv)
        {
          this.eventService = serv;
        }

       
        public ActionResult Vacations()
        {
            var result = eventService.GetVacations(DateTime.Now.Year);
            if (result.HasError)
              this.AddNotification(result.MessageResult, NotificationType.ERROR);
            return View(result.ReturnValue);
        }
    }
}