using IkubInternship.DomainModels.CustomValidators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.DomainModels
{
  public class Event
  {
    public int EventId { get; set; }
    [Required]
    [StringLength(40)]
    public string Title { get; set; }
    [MaxNrOfWords(50, ErrorMessage ="Description can not have more than 50 words.")]
    public string Description { get; set; }
    [Required]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; }
    [Display(Name = "Finish Date")]
    public DateTime FinishDate { get; set; }
  }
}
