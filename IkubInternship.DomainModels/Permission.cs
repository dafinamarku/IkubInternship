using IkubInternship.DomainModels.CustomValidators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.DomainModels
{
  public class Permission
  {
    public int PermissionId { get; set; }

    [Display(Name ="Permission Date")]
    [Required]
    [LaterThanToday(ErrorMessage ="Start Date must be later than today.")]
    public DateTime PermissionDate { get; set; }

    [Display(Name = "Reason For Asking")]
    [MaxNrOfWords(100, ErrorMessage = "Reason For Asking can't contain more than 100 words.")]
    //punonjesi kur kerkon leje jep edhe nje arsye per kerkesen qe po ben
    public string ReasonForAsking { get; set; }

    //mund te marre vlerat: Approved, Refused, Asked
    public string Status { get; set; }

    [Display(Name = "Reason For Refusal")]
    //ne rast se leja refuzohet ReasonForRefusal do te jete jo boshe
    public string ReasonForRefusal { get; set; }

    public string EmployeeId { get; set; }
    
    public virtual ApplicationUser Employee { get; set; }
  }
}
