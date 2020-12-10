using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.DomainModels.CustomValidators
{
  public class LaterThanToday : ValidationAttribute
  {
    
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value != null)
      {
        DateTime inputDate = (DateTime)value;
        //data e marre si input duhet te jete me e madhe se data e sotme
        if (DateTime.Compare(inputDate, DateTime.Now) <= 0)
          return new ValidationResult(this.ErrorMessage);
        else
          return ValidationResult.Success;
      }
      return ValidationResult.Success;

    }
  }
}
