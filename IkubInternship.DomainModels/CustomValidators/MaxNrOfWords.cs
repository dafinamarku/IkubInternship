using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.DomainModels.CustomValidators
{
  public class MaxNrOfWords:ValidationAttribute
  {
    int max;

    public MaxNrOfWords(int m)
    {
      this.max = m;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value!=null)
      {
        string currentString = (string)value;
     
        string[] arr = currentString.Split(' ');
        if (arr.Length > max)
          return new ValidationResult(this.ErrorMessage);
        else
          return ValidationResult.Success;
      }
      else
        return ValidationResult.Success;
      
    }
  }
}
