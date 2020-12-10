using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.DomainModels
{
  [Table("NrPermissionsPerYear")]
  public class PermissionsPerYear
  {
    public int Id { get; set; }
    public int Year { get; set; }
    [Required]
    [Display(Name ="Nr Of Permissions")]
    [Range(1, Int64.MaxValue, ErrorMessage ="Nr Of Permissions must be greater than 0")]
    public int NrOfPermissions { get; set; }
  }
}
