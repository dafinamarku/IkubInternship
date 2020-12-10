using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.DomainModels
{
  public class Result<R>
  {
    private readonly bool hasError;
    private readonly string message;
    private readonly R res;

    public Result(R r, bool isError, string m)
    {
      res = r;
      hasError = isError;
      message = m;
    }

    public Result(R r, bool isError, Exception ex)
    {
      res = r;
      hasError = isError;
      message = ex.Message;
    }

    public bool HasError
    {
      get { return hasError; }
    }

    public string MessageResult
    {
      get { return message; }
    }

    public R ReturnValue
    {
      get { return res; }
    }
  }
}
