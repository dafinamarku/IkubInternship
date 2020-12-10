using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.DomainModels.CustomValidators
{
  public class DateLessThan:ValidationAttribute
  {
    private readonly string _comparisonProperty;

    public DateLessThan(string comparisonProperty)
    {
      this._comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value != null)
      {
        DateTime currentDate = (DateTime)value;
     
        var cmpProperty = validationContext.ObjectType.GetProperty(_comparisonProperty);

        if (cmpProperty == null)
          throw new ArgumentException("Property with this name not found");

        var comparisonValue = (DateTime)cmpProperty.GetValue(validationContext.ObjectInstance);

        //currentDate duhet te jete me e madhe se vlera e atributit me te cilen po krahasohet 
        if (DateTime.Compare(currentDate, comparisonValue) > 0)
          return new ValidationResult(ErrorMessage);
        else
          return ValidationResult.Success;
      }
      return ValidationResult.Success;

    }
  }
}
