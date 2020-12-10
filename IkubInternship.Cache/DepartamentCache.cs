using IkubInternship.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.IO;

namespace IkubInternship.Cache
{
  public class DepartamentCache
  {
    ObjectCache cache = MemoryCache.Default;

    public void UpdateCache(List<Departament> deps)
    {
      CacheItemPolicy policy = new CacheItemPolicy();
      policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(100.0);
      cache.Set("departaments", deps, policy);
    }

    public void AddOrRemoveEntryFromCache(Departament dep, List<Departament> model, bool remove)
    {
      if (remove == false)
        model.Add(dep);
      else
        model.Remove(dep);
      CacheItemPolicy policy = new CacheItemPolicy();
      policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(100.0);
      cache.Set("departaments", model, policy);
    }

    public void UpdateChacheRecord(Departament dep, List<Departament> model)
    {
      var recToUpdate = model.Where(x => x.DepartamentId == dep.DepartamentId).FirstOrDefault();
      model.Remove(recToUpdate);
      recToUpdate.Name = dep.Name;
      recToUpdate.ParentDepId = dep.ParentDepId;
      model.Add(recToUpdate);
      CacheItemPolicy policy = new CacheItemPolicy();
      policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(100.0);
      cache.Set("departaments", model, policy);
    }

    public Departament GetDepartamentFromCache(int depId)
    {
      var depCache= cache["departaments"] as List<Departament>;
      if (depCache != null)
        return depCache.FirstOrDefault(x => x.DepartamentId == depId);
      else
        return null;
    }
  }
}
