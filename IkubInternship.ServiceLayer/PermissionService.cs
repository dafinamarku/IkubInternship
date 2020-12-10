using IkubInternship.DomainModels;
using IkubInternship.RepositoryContracts;
using IkubInternship.RepositoryLayer;
using IkubInternship.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.ServiceLayer
{
  public class PermissionService:IPermissionService
  {
    IPermissionsRepository repository;
    ExceptionDbLogger exDbLogger;

    private static readonly log4net.ILog log
   = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public PermissionService(IPermissionsRepository rep)
    {
      this.repository = rep;
      exDbLogger = new ExceptionDbLogger();
    }

    public MultiResult<Permission> GetPermissionsForHr()
    {
      try
      {
        List<Permission> permissionsForHr = repository.GetPermissionsForHr()
          .OrderByDescending(x=>x.Status=="Asked")
          .ThenByDescending(x=>x.Status=="Canceled")
          .ThenBy(x => x.PermissionDate).ToList();
        return new MultiResult<Permission>(permissionsForHr, false, String.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new MultiResult<Permission>(null, true, ex);
      }
    }

    public MultiResult<Permission> GetPermissionsForSupervisor(string supervisorId)
    {
      try
      {
        var result = repository.GetPermissionsForSupervisor(supervisorId)
          .OrderByDescending(x => x.Status == "Asked")
          .ThenByDescending(x => x.Status == "Canceled")
          .ThenBy(x => x.PermissionDate).ToList(); 
        return new MultiResult<Permission>(result, false, string.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new MultiResult<Permission>(null, true, ex);
      }
    }

    public MultiResult<Permission> GetPermissionsForEmployee(string employeeId)
    {
      try
      {
        var result = repository.GetPermissionsForEmployee(employeeId).OrderBy(x=>x.PermissionDate).ToList();
        return new MultiResult<Permission>(result, false, string.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new MultiResult<Permission>(null, true, ex);
      }
    }

    public Result<Permission> GetPermissionById(int id)
    {
      try
      {
        var res = repository.GetPermissionById(id);
        if (res == null)
        {
          log.Error("Tried to access a non existing Permission");
          exDbLogger.InsertDbException("Tried to access a non existing Permission", DateTime.Now);
          return new Result<Permission>(null, true, "There is no permission corresponding to id: " + id.ToString());
        }
        else
          return new Result<Permission>(res, false, string.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<Permission>(null, true, ex);
      }
    }

    //nje leje nuk mund te shtohet nqs punonjesi nuk ka me leje te mbetura
    //ose nqs data e lejes eshte dite pushimi
    public Result<bool> InsertPermission(Permission p)
    {
      try
      {
        var askedBy = repository.GetUser(p.EmployeeId);
        if (askedBy.RemainingPermissions == 0)
          return new Result<bool>(false, true, "You have no remaining permissions.");
        if (repository.IsVacation(p.PermissionDate))
          return new Result<bool>(false, true, p.PermissionDate.ToString() + " is already a day off.");
        bool res = repository.InsertPermission(p);
        if (res == true)
          return new Result<bool>(res, false, string.Empty);
        else
        {
          log.Error("Tried to insert a Permission for dates that were asked before.");
          exDbLogger.InsertDbException("Tried to insert a Permission for dates that were asked before.", DateTime.Now);
          return new Result<bool>(res, true, "You tried to ask permission a date that you have already asked.");
        }
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    public Result<bool> UpdatePermission(Permission p)
    {
      try
      {
        bool result = repository.UpdatePermission(p);
        if (result == true)
          return new Result<bool>(result, false, string.Empty);
        else
        {
          string err = "Tried to update a non existing Permission OR tried to ask permission for some dates that you have already asked permission.";
          log.Error(err);
          exDbLogger.InsertDbException(err, DateTime.Now);
          return new Result<bool>(result, true, err);
        }
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    public Result<bool> DeletePermission(int id)
    {
      try
      {
        bool res = repository.DeletePermission(id);
        if (res == true)
          return new Result<bool>(res, false, string.Empty);
        else
        {
          string err = "Tried to delete a non existing Permission.";
          log.Error(err);
          exDbLogger.InsertDbException(err, DateTime.Now);
          return new Result<bool>(res, true, err);
        }
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    public Result<bool> CanBeRefusedOrApprovedBy(string uid, string userRole, int permissionId)
    {
      try
      {
        var permission = repository.GetPermissionById(permissionId);
        if (permission == null)
        {
          string err = "This Permission can not be refused OR approved beacuse it doesn't exist.";
          log.Error(err);
          exDbLogger.InsertDbException(err, DateTime.Now);
          return new Result<bool>(false, true, err);
        }

        //nje leje mund te refuzohet/pranohet ne rast se ajo ka statusing Asked
        //ajo mund te refuzohet  1.nga HR nqs ajo eshte kerkuar nga nje pergjegjes departamenti
        //2.nga nje pergjegjes dep nqs ajo eshte kerkuar nga nje punonjes i te njejtit dep (jo nga vete pergjegjesi)
        if (permission.Status != "Asked" || permission.Employee.DeleteStatus==true)//nuk mund tu ndryshohet statusi lejeve qe jane bere nga punonjes qe tashme jane fshire
        {
          string err = "Can not refuse OR approve a permission that has Status != Asked";
          log.Error(err);
          exDbLogger.InsertDbException(err, DateTime.Now);
          return new Result<bool>(false, true, err);
        }

        if (userRole == "HR")
        {
          if (permission.Employee.isSupervisor == true)
            return new Result<bool>(true, false, string.Empty);
          else
          {
            string err = "A HR can not refuse OR approve a permission that was not asked by a supervisor.";
            log.Error(err);
            exDbLogger.InsertDbException(err, DateTime.Now);
            return new Result<bool>(false, true, err);
          }
        }
        else //ne kete rast userRole=="Supervisor", qe mund te refuzoje nje leje vtm nqs eshte kerkuar nga nje punonjes i te njejtit dep ose i nje dep bije qe nuk ka pergjegjes
        {
          ApplicationUser currentUser = repository.GetUser(uid);
          if ((currentUser.DepId == permission.Employee.DepId && currentUser.Id != permission.Employee.Id) || (permission.Employee.EmployeeDep.ParentDepId == currentUser.DepId && repository.HasSupervisor((int)permission.Employee.DepId)==false))
            return new Result<bool>(true, false, string.Empty);
          else
          {
            string error = "A Supervisor can not refuase OR approve a permission asked by himself OR someone from a different departament.";
            log.Error(error);
            exDbLogger.InsertDbException(error, DateTime.Now);
            return new Result<bool>(false, true, error);
          }
        }
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    //kontrollon nqs nje user mund te shohe nje kerkese per leje dhe nqs mundet kthen lejen si vlere kthimi
    public Result<Permission> CanUserViewDetailsOf(string uid, string userRole, int permissionId)
    {
      try
      {
        var permission = repository.GetPermissionById(permissionId);
        if (permission == null)
        {
          string err = "Can not view this permission because it doesn't exist.";
          log.Error(err);
          exDbLogger.InsertDbException(err, DateTime.Now);
          return new Result<Permission>(null, true, err);
        }
        //nje punonjes mund te shikoje vetem lejet qe ai vete ka kerkuar
        if (userRole == "Employee")
        {
          if (permission.Employee.Id == uid && permission.Status!="Canceled")
            return new Result<Permission>(permission, false, string.Empty);
          else
          {
            string err = "An Employee tried to view details of a permission that was not asked by him/her.";
            log.Error(err);
            exDbLogger.InsertDbException(err, DateTime.Now);
            return new Result<Permission>(null, true, "You can't view details of a permission that was not asked by you.");
          }
        }
        else
        {
          //HR mund te shikoje vetem lejet e kerkuara nga pergjegjesit e departamentit
          if (userRole == "HR")
          {
            if (permission.Employee.isSupervisor == true) 
              return new Result<Permission>(permission, false, string.Empty);
            else
            {
              string err = "HR tried to view details of a permission that was not asked by a supervisor.";
              log.Error(err);
              exDbLogger.InsertDbException(err, DateTime.Now);
              return new Result<Permission>(null, true, "You can't view details of a permission that was not asked by a supervisor.");
            }
          }
          else //userRole=="Supervisor"
          {
            //nje pergjegjes mund te shohe vetem lejet e kerkuara nga vete (por qe nuk jane te anuluara) ai ose nga punonjes qe jane ne te njejtin dep me te
            var theSupervisor = repository.GetUser(uid);
            if ((permission.EmployeeId==uid && permission.Status != "Canceled") || 
                (permission.Employee.DepId == theSupervisor.DepId && permission.EmployeeId!=uid && permission.Employee.DeleteStatus == false) ||
                (permission.Employee.EmployeeDep.ParentDepId == theSupervisor.DepId && repository.HasSupervisor((int)permission.Employee.DepId) == false && permission.Employee.DeleteStatus == false))
              return new Result<Permission>(permission, false, string.Empty);
            else
            {
              string err = "A Supervisor trued to view a permission that was not asked by himself or someone from the same departament.";
              log.Error(err);
              exDbLogger.InsertDbException(err, DateTime.Now);
              return new Result<Permission>(null, true, "You can't view details of a permission that was not asked by you OR someone from the same departament.");
            }
          }

        }
     
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<Permission>(null, true, ex);
      }
    }

    //nr max i lejeve te lejuara (qe mund te percaktoje HR) per 1 vit mund te jete deri ne 100 dite pune
    public Result<bool> SetNrOfPermissionsPerYear(int nr)
    {
      try
      {
        if (nr > 100)
          return new Result<bool>(false, true, "Permissions per year can not be more than 100.");
        repository.SetNrOfPermissionsPerYear(nr);
        return new Result<bool>(true, false, string.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    public Result<PermissionsPerYear> GetPermissionsPerYear(int year)
    {
      try
      {
        var res=repository.GetPermissionsPerYear(year);
        return new Result<PermissionsPerYear>(res, false, string.Empty);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<PermissionsPerYear>(null, true, ex);
      }
    }

    //nje user mund te anuloje nje kerkese per leje nqs 
    //1.kerkesa ekziston, eshte bere nga vete ai, 2.nese kerkesa eshte e pamiratuar, 3.nese kerkesa eshte aprovuar 
    //do te mund te anulohet jo me pak se 48 ore nga dita e lejes
    public Result<bool> CanUserCancelPermission(string uid, int permissionId)
    {
      try
      {
        var permissionToCancel = repository.GetPermissionById(permissionId);
        if (permissionToCancel != null)
        {
          string error = string.Empty;
          if (permissionToCancel.Employee.Id != uid)
          {
            error = "You can not cancel a permission that was not asked by you";
            log.Error(error);
            exDbLogger.InsertDbException(error, DateTime.Now);
            return new Result<bool>(false, true, error);
          }

          TimeSpan diff = permissionToCancel.PermissionDate - DateTime.Now;
          if (permissionToCancel.Status == "Asked")
            return new Result<bool>(true, false, string.Empty);
          else
          {
            if (permissionToCancel.Status == "Approved" && diff.TotalHours >= 48)
              return new Result<bool>(true, false, string.Empty);
            else
            {
              error = "If the permission is approved it CAN be canceled not less than 48 hours before the Permission Date.";
              log.Error(error);
              exDbLogger.InsertDbException(error, DateTime.Now);
              return new Result<bool>(false, true, error);
            }

          }
        }
        string err = "Tried to cancel a non existing permission.";
        log.Error(err);
        exDbLogger.InsertDbException(err, DateTime.Now);
        return new Result<bool>(false, true, err);
      }
      catch (Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }

    public Result<bool> CanUserDeletePermission(string uid, string userRole, int permissionId)
    {
      try
      {
        string error = string.Empty;
        if (userRole == "HR" || userRole == "Supervisor")
        {
          var permission = repository.GetPermissionById(permissionId);
          if (permission != null)
          {
            if (permission.Employee.DeleteStatus == true)
              error = "You can not delete this permission.";
            else
            {
              if (userRole == "HR" && permission.Employee.isSupervisor == true && permission.Status == "Canceled")
                return new Result<bool>(true, false, string.Empty);
              var user = repository.GetUser(uid);
              //nje pergjegjes mund ta fshije nje leje nqs leja nuk eshte kerkuar nga vete ai dhe eshte kerkuar ose nga nje punonjes i te njetit dep ose
              //nje punonjes i nje dep bije qe nuk ka pergjegjes
              if (userRole == "Supervisor" && permission.EmployeeId != user.Id && permission.Status == "Canceled" && (permission.Employee.DepId == user.DepId || (permission.Employee.EmployeeDep.ParentDepId == user.DepId && repository.HasSupervisor((int)permission.Employee.DepId) == false)))
                return new Result<bool>(true, false, string.Empty);
              error = "You can not delete this permission";
            }
          }
          else
            error = "Can not delete a non existing permission.";
        }
        else
        {
          error = "An Employee can not delete a permission.";
        }
        log.Error(error);
        exDbLogger.InsertDbException(error, DateTime.Now);
        return new Result<bool>(false, true, error);
      }
      catch(Exception ex)
      {
        log.Error(ex.Message);
        exDbLogger.InsertDbException(ex.Message, DateTime.Now);
        return new Result<bool>(false, true, ex);
      }
    }
  }
}
