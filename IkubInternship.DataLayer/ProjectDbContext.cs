using IkubInternship.DomainModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.DataLayer
{
  public class ProjectDbContext:IdentityDbContext<ApplicationUser>
  {
    public ProjectDbContext() : base("PermissionManagementCon")
    { }

    public static ProjectDbContext Create()
    {
      return new ProjectDbContext();
    }
    public DbSet<Departament> Departaments { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<PermissionsPerYear> NrPermissionsPerYear { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<ExceptionLog> ExceptionLogs { get; set; }
  }
}
