using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IkubInternship.ViewModels
{
  public class CreateEmployeeViewModel
  {
    [Required]
    [RegularExpression("^[a-z A-Z]*$", ErrorMessage = "{0} must contain only letters (space is allowed).")]
    [StringLength(40)]
    public string Name { get; set; }

    [Display(Name = "Last Name")]
    [Required]
    [RegularExpression("^[a-z A-Z]*$", ErrorMessage = "{0} must contain only letters (space is allowed).")]
    [StringLength(40)]
    public string LastName { get; set; }

    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [Required]
    [RegularExpression("^[A-Za-z0-9]*$", ErrorMessage = "{0} must contain only letters and digits.")]
    public string Username { get; set; }

    [Required]
    public int DepartamentId { get; set; }

    [Required]
    [StringLength(50,MinimumLength =5)]
    public string Password { get; set; }

    [Display(Name="Confirm Password")]
    [Required]
    [Compare("Password", ErrorMessage = "Password and confirm password do not match")]
    public string ConfirmPassword { get; set; }
  }
}