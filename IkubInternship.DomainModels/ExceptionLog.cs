using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.DomainModels
{
  public class ExceptionLog
  {
    public int Id { get; set; }
    public string Message { get; set; }
    public DateTime LogTime { get; set; }
  }
}
