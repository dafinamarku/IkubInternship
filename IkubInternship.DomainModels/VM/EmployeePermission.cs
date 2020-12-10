using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IkubInternship.DomainModels.VM
{
  public class EmployeePermission
  {
    [Display(Name ="Full Name")]
    public string FullName { get; set; }
    public int Remaining { get; set; }
    public int Approved { get; set; }
    public int Refused { get; set; }
    public int Asked { get; set; }
    public int Canceled { get; set; }
   

  }
}