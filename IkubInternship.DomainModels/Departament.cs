using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.DomainModels
{
  public class Departament
  {
    public int DepartamentId { get; set; }
    [Required]
    [RegularExpression("^[a-z A-Z]*$", ErrorMessage = "{0} must contain only letters (space is allowed).")]
    [StringLength(30, MinimumLength =2, ErrorMessage ="{0} must have 2 to 30 characters.")]
    [Display(Name = "Departament Name")]
    public string Name { get; set; }
    [ForeignKey("ParentDep")]
    [Display(Name="Parent Departament")]
    public Nullable<int> ParentDepId { get; set; }

    public virtual Departament ParentDep { get; set; }
    public virtual ICollection<ApplicationUser> Employees { get; set; }
  }
}
