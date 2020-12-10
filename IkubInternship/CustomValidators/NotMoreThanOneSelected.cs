using IkubInternship.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IkubInternship.CustomValidators
{
  public class NotMoreThanOneSelected : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value != null)
      {
        List<Employee> currentList = (List<Employee>)value;
        if (currentList.Where(x => x.IsSelected == true).Count() > 1)
          return new ValidationResult(this.ErrorMessage);
        else
          return ValidationResult.Success;
      }
      return new ValidationResult(this.ErrorMessage);
    }
  }
}