using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.DomainModels
{
  public class ApplicationUser:IdentityUser
  {
    public string Name { get; set; }
    [Display(Name="Last Name")]
    public string LastName { get; set; }
    [Display(Name ="Remaining Permissions")]
    public int RemainingPermissions { get; set; }
    [ForeignKey("EmployeeDep")]
    public Nullable<int> DepId { get; set; }
    //percakton nqs punonjesi eshte pergjegjes ne dep ku punon ose jo
    public bool isSupervisor { get; set; }
    //soft delete
    public bool DeleteStatus { get; set; }

    public virtual ICollection<Permission> EmployeePermissions { get; set; }
    public virtual Departament EmployeeDep { get; set; }

    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    {
      // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
      var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
      // Add custom user claims here
      return userIdentity;
    }
  }
}
