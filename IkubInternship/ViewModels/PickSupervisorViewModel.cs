using IkubInternship.CustomValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IkubInternship.ViewModels
{
  public class PickSupervisorViewModel
  {
    [NotMoreThanOneSelected(ErrorMessage ="You can not select more than one supervisor.")]
    public List<Employee> EmployeesOfDepartament { get; set; }
  }
}