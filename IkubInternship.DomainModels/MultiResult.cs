using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.DomainModels
{
  public class MultiResult<M>
  {
    public MultiResult(List<M> r, bool isError, string message)
    {
      ReturnValue = r;
      HasError = isError;
      MessageResult = message;
    }

    public MultiResult(List<M> r, bool isError, Exception ex)
    {
      ReturnValue = r;
      HasError = isError;
      MessageResult = ex.Message;
    }

    public bool HasError { get; }

    public string MessageResult { get; }

    public List<M> ReturnValue { get; }
  }
}
