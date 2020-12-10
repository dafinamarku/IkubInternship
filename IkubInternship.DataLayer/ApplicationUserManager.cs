
using IkubInternship.DomainModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IkubInternship.DataLayer
{
  public class ApplicationUserManager:UserManager<ApplicationUser>
  {
    
    public ApplicationUserManager(ApplicationUserStore store) : base(store)
    {
     
    }

    public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
           IOwinContext context)
    {
      var manager = new ApplicationUserManager(new ApplicationUserStore(new ProjectDbContext()));
   
      // Configure user lockout defaults
      manager.UserLockoutEnabledByDefault = true;
      manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
      manager.MaxFailedAccessAttemptsBeforeLockout = 3;
   
      manager.EmailService = new EmailService();
      var dataProtectionProvider = options.DataProtectionProvider;
      if (dataProtectionProvider != null)
      {
        manager.UserTokenProvider =
            new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
            {
              TokenLifespan = TimeSpan.FromHours(3)
            };
      }
      return manager;
    }
  }

  public class EmailService : IIdentityMessageService
  {
    public async Task SendAsync(IdentityMessage message)
    {
      await configSendGridasync(message);
    }

    private async Task configSendGridasync(IdentityMessage message)
   {
      var apiKey = ConfigurationManager.AppSettings["HrEmailKey"];
      var client = new SendGridClient(apiKey);
      var from = new EmailAddress("permissionHr@outlook.com", "HR");
      var subject = message.Subject;
      var to = new EmailAddress(message.Destination);
      var plainTextContent = message.Body;
      var htmlContent = message.Body;
      var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
      var response = await client.SendEmailAsync(msg);

      // Send the email.
      if (client != null)
      {
        await client.SendEmailAsync(msg);
      }
      else
      {
        Trace.TraceError("Failed to create Web transport.");
        await Task.FromResult(0);
      }
    }
  }


  // Configure the application sign-in manager which is used in this application.
  public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
  {
    public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
        : base(userManager, authenticationManager)
    {
    }

    public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
    {
      return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
    }

    public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
    {
      return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
    }
  }
}
