using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.DomainModels.VM
{
  public class PermissionReportViewModel
  {
    [Display(Name ="Full Name")]
    public string FullName { get; set; }
    public string Departament { get; set; }
    [Display(Name ="Permission Date")]
    public DateTime PermissionDate { get; set; }
    [Display(Name = "Permission Status")]
    public string PermissionStatus { get; set; }
  }
}
