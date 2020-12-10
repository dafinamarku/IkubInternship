using IkubInternship.DomainModels.VM;
using IkubInternship.RepositoryLayer;
using IkubInternship.ServiceContracts;
using IkubInternship.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IkubInternship.ApiControllers
{
    public class ApiReportController : ApiController
    {
    static ReportsRepository repository = new ReportsRepository();
    ReportsService rService=new ReportsService(repository);

    public List<PermissionReportViewModel> Get(string depName, DateTime? fromDate, DateTime? toDate, string employeeName)
    {
      var result = rService.HrEmployeesPermissions(depName, fromDate, toDate, employeeName);
      if (result.HasError)
        return new List<PermissionReportViewModel>();
      else
        return result.ReturnValue;
    }

    }
}
