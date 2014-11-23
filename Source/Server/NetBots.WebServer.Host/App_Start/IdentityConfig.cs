using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using NetBots.WebServer.Host.Models;
using NetBots.WebServer.Model;
using NetBots.WebServer.Data.MsSql;

namespace NetBots.WebServer.Host
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // TODO: Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // TODO: Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });

            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });

            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;

            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }

        public async Task<IList<PlayerBot>> GetPlayerBotsAsync(string userId)
        {
            var user = await this.FindByIdAsync(userId);
            if (user != null && user.Bots != null)
            {
                return user.Bots.ToList();
            }
            else
            {
                return new PlayerBot[0];
            }
        }

        public async Task AddBotAsync(string userId, PlayerBot bot)
        {
            var db = new ApplicationDbContext();
            if (db.PlayerBots.Any(x => x.Name == bot.Name))
            {
                throw new ArgumentException("A bot with that name already exists");
            }
            if (db.PlayerBots.Any(x => x.URL == bot.URL))
            {
                throw new ArgumentException("A bot with that URL already exists");
            }
            var user = await this.FindByIdAsync(userId);
            user.Bots.Add(bot);
            await this.UpdateAsync(user);
        }

        public async Task UpdateBotAsync(string userId, PlayerBotViewModel model)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (user != null)
                {
                    var botToUpdate = db.PlayerBots.FirstOrDefault(x => x.Id == model.Id && x.OwnerId == user.Id);
                    if (botToUpdate != null)
                    {
                        if (db.PlayerBots.Any(x => x.Name == model.Name && x.Id != model.Id))
                        {
                            throw new ArgumentException("A bot with that name already exists");
                        }
                        botToUpdate.Name = model.Name;
                        botToUpdate.URL = model.Url;
                        botToUpdate.Image = model.Image;
                        botToUpdate.Private = model.Private;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        throw new ArgumentException("Could not find bot");
                    }
                }
                else
                {
                    throw new ArgumentException("Could not find user");
                }
            }
        }

        public async Task DeleteBot(string userId, int botId)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (user != null)
                {
                    var botToRemove = db.PlayerBots.FirstOrDefault(x => x.Id == botId && x.OwnerId == user.Id);
                    if (botToRemove != null)
                    {
                        var games = db.GameSummaries.Where(x => x.Player1.Id == botToRemove.Id || x.Player2.Id == botToRemove.Id);
                        db.GameSummaries.RemoveRange(games);
                        db.PlayerBots.Remove(botToRemove);
                        await db.SaveChangesAsync();
                        return;
                    }
                    throw new ArgumentException("Could not find bot");
                }
                throw new ArgumentException("Could not find user");
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
