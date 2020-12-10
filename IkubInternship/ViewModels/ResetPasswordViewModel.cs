using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IkubInternship.ViewModels
{
  public class ResetPasswordViewModel
  {
    [Required]
    [Display(Name ="New Password")]
    [StringLength(50, MinimumLength = 5)]
    public string NewPassword { get; set; }
    [Required]
    [Display(Name ="Confirm New Password")]
    [Compare("NewPassword", ErrorMessage ="New Password and Confirm New Password do not match.")]
    public string ConfirmNewPassword { get; set; }
  }
}