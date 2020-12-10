using IkubInternship.DataLayer;
using IkubInternship.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.RepositoryLayer
{
  public class ExceptionDbLogger
  {
    ProjectDbContext db = new ProjectDbContext();

    public void InsertDbException(string message, DateTime time)
    {
      ExceptionLog exLog = new ExceptionLog()
      {
        Message = message,
        LogTime = time
      };
      db.ExceptionLogs.Add(exLog);
      db.SaveChanges();
    }
  }
}
