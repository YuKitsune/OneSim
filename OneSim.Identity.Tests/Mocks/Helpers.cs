// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helpers.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using Moq;

    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Infrastructure;
    using OneSim.Identity.Persistence;

    /// <summary>
    ///     The mocking helpers.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        ///     Gets a new <see cref="IIdentityDbContext{TUser}"/>.
        /// </summary>
        /// <returns>
        ///     A mocked <see cref="IIdentityDbContext{TUser}"/>.
        /// </returns>
        public static IIdentityDbContext<User> GetDbContext() =>
            new IdentityDbContext(
                new DbContextOptionsBuilder<IdentityDbContext>()
                   .UseInMemoryDatabase(Guid.NewGuid().ToString())
                   .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                   .Options);

        /// <summary>
        ///     Gets a new <see cref="UserManager{TUser}"/> for the <see cref="User"/>.
        /// </summary>
        /// <param name="dbContext">
        ///     The <see cref="IIdentityDbContext{TUser}"/>.
        /// </param>
        /// <returns>
        ///     The mocked <see cref="UserManager{TUser}"/>.
        /// </returns>
        public static UserManager<User> GetUserManager(IIdentityDbContext<User> dbContext)
        {
            Mock<IUserStore<User>> mockStore = new Mock<IUserStore<User>>();
            Mock<UserManager<User>> mockManager = new Mock<UserManager<User>>(mockStore.Object, null, null, null, null, null, null, null, null);
            mockManager.Object.UserValidators.Add(new UserValidator<User>());
            mockManager.Object.PasswordValidators.Add(new PasswordValidator<User>());

            // Todo: This needs a major fucking cleanup...
            mockManager.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<User>(
                                (user) =>
                                {
                                    User targetUser = dbContext.Users.FirstOrDefault(u => u.UserName == user.UserName);
                                    dbContext.Users.Remove(targetUser);
                                    dbContext.SaveChanges();
                                });
            mockManager.Setup(x => x.CreateAsync(It.IsAny<User>()))
                       .ReturnsAsync(IdentityResult.Success)
                       .Callback<User>((x) =>
                                       {
                                           dbContext.Users.Add(x);
                                           dbContext.SaveChanges();
                                       });
            mockManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                       .ReturnsAsync(IdentityResult.Success)
                       .Callback<User, string>((x, y) =>
                                               {
                                                   dbContext.Users.Add(x);
                                                   dbContext.SaveChanges();
                                               });
            mockManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
                       .ReturnsAsync(IdentityResult.Success)
                       .Callback<User, string>((user, token) =>
                                               {
                                                   User targetUser = dbContext
                                                                    .Users.FirstOrDefault(u => u.Email == user.Email);
                                                   targetUser.EmailConfirmed = true;
                                                   dbContext.SaveChanges();
                                               });
            mockManager.Setup(x => x.IsEmailConfirmedAsync(It.IsAny<User>())).ReturnsAsync(
                                                                                           (User user) => user.EmailConfirmed);
            mockManager.Setup(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<User, string, string>((user, oldPassword, newPassword) =>
                                                {
                                                    User targetUser = dbContext
                                                                     .Users.FirstOrDefault(u => u.Email == user.Email);
                                                    targetUser.PasswordHash = newPassword;
                                                    dbContext.SaveChanges();
                                                });
            mockManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);
            mockManager
                .Setup(x => x.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success).Callback<User, string, string>(
                                                                                     (user, newPassword, token) =>
                                                                                     {
                                                                                         User targetUser = dbContext.Users.FirstOrDefault(u => u.UserName == user.UserName);
                                                                                         targetUser.PasswordHash = newPassword;
                                                                                         dbContext.SaveChanges();
                                                                                     });
            mockManager
                .Setup(x => x.AddPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success).Callback<User, string>(
                                                                             (user, newPassword) =>
                                                                             {
                                                                                 User targetUser = dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
                                                                                 targetUser.PasswordHash = newPassword;
                                                                                 dbContext.SaveChanges();
                                                                             });
            mockManager.Setup(x =>
                    x.VerifyTwoFactorTokenAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            mockManager.Setup(x =>
                    x.GenerateNewTwoFactorRecoveryCodesAsync(It.IsAny<User>(), It.IsAny<int>()))
                .ReturnsAsync(new List<string>() { "TEST" });
            mockManager.Setup(x => x.SetTwoFactorEnabledAsync(It.IsAny<User>(), It.IsAny<bool>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<User, bool>(
                                      (user, enabled) =>
                                      {
                                          User targetUser = dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
                                          targetUser.TwoFactorEnabled = enabled;
                                          dbContext.SaveChanges();
                                      });

            return mockManager.Object;
        }

        /// <summary>
        ///     Gets a new <see cref="SignInManager{TUser}"/> for the <see cref="User"/>.
        /// </summary>
        /// <param name="userManager">
        ///     The <see cref="UserManager{TUser}"/>.
        /// </param>
        /// <returns>
        ///     The mocked <see cref="SignInManager{TUser}"/>.
        /// </returns>
        public static SignInManager<User> GetSignInManager(UserManager<User> userManager)
        {
            return new SignInManager<User>(
                userManager,
                new HttpContextAccessor(),
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<User>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<User>>().Object);
        }
    }
}
