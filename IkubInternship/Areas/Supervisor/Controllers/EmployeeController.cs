using IkubInternship.Extensions;
using IkubInternship.ServiceContracts;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IkubInternship.Areas.Supervisor.Controllers
{
    [Authorize(Roles ="Supervisor")]
    public class EmployeeController : Controller
    {
        IEmployeeService employeeService;
        public EmployeeController(IEmployeeService serv)
        {
          employeeService = serv;
        }
        //kthen listen me punonjesit qe jane ne te njejtin dep me supervizorin ose qe 
        //bejne pjese ne departamente bije qe nuk kane pergjegjes
        public ActionResult Index()
        {
            var result = employeeService.GetEmployeesForSupervisor(User.Identity.GetUserId());
            if (result.HasError)
              this.AddNotification(result.MessageResult, NotificationType.ERROR);

            return View(result.ReturnValue);
        }
    }
}