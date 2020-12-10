using IkubInternship.DomainModels.CustomValidators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IkubInternship.ViewModels
{
  public class RefusePermissionViewModel
  {
    public int Id { get; set; }
    [Required]
    [MaxNrOfWords(100, ErrorMessage ="Reason For Refusal can not contain more than 100 words")]
    [Display(Name = "Reason For Refusal")]
    public string ReasonForRefusal { get; set; }
  }
}