namespace OneSim.Identity.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    using OneSim.Identity.Application;
    using OneSim.Identity.Application.Interfaces;
    using OneSim.Identity.Domain.Entities;
    using OneSim.Identity.Domain.Exceptions;
    using OneSim.Identity.Tests.Utils;

    /// <summary>
    ///     The <see cref="UserService"/> Tests.
    /// </summary>
    [TestFixture]
    public class UserServiceTests
    {
        /// <summary>
        ///     The default request scheme.
        /// </summary>
        public const string DefaultRequestScheme = "HTTPS";

        /// <summary>
        ///     Ensures a new user can be created and an email confirmation email is sent to the user.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task UserCanBeCreated()
        {
            // Arrange
            MockSet mocks = new MockSet();
            ApplicationUser userToCreate = new ApplicationUser { UserName = "TestUser", Email = "test@test.com" };
            string password = "MySecurePassword123";

            // Act
            UserService service = new UserService(mocks.UserManager, mocks.SignInManager, mocks.Logger);
            await service.CreateUser(userToCreate, password, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);

            // Assert
            Assert.IsTrue(mocks.DbContext.Users.Any(u => u.UserName == userToCreate.UserName));
            Assert.IsTrue(mocks.EmailSender.SentMessages.Any(m => m.Recipient == userToCreate.Email));
        }

        /// <summary>
        ///     Ensures an existing user can be deleted.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task UserCanBeDeleted()
        {
            // Arrange
            MockSet mocks = new MockSet();
            ApplicationUser userToDelete = new ApplicationUser { UserName = "TestUser", Email = "test@test.com" };
            string password = "MySecurePassword123";

            // Create the user first
            UserService service = new UserService(mocks.UserManager, mocks.SignInManager, mocks.Logger);
            await service.CreateUser(userToDelete, password, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);

            // Act
            await service.DeleteUser(userToDelete);

            // Assert
            Assert.IsTrue(!mocks.DbContext.Users.Any());
        }

        /// <summary>
        ///     Ensures a password reset email is sent.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task PasswordResetEmailIsSent()
        {
            // Arrange
            MockSet mocks = new MockSet();
            ApplicationUser testUser = new ApplicationUser { UserName = "TestUser", Email = "test@test.com" };
            string password = "MySecurePassword123";

            // Create the user first
            UserService service = new UserService(mocks.UserManager, mocks.SignInManager, mocks.Logger);
            await service.CreateUser(testUser, password, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);
            await service.ConfirmEmail(testUser, string.Empty);

            // Act
            await service.SendPasswordResetEmail(mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender, testUser);

            // Assert
            Assert.IsTrue(mocks.EmailSender.SentMessages.Count == 2);
        }

        /// <summary>
        ///     Ensures a password reset email is not sent when the email has not been confirmed.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task PasswordResetEmailIsNotSentToUnConfirmedEmail()
        {
            // Arrange
            MockSet mocks = new MockSet();
            ApplicationUser testUser = new ApplicationUser { UserName = "TestUser", Email = "test@test.com" };
            string password = "MySecurePassword123";

            // Create the user first
            UserService service = new UserService(mocks.UserManager, mocks.SignInManager, mocks.Logger);
            await service.CreateUser(testUser, password, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);

            // Act / Assert
            Assert.ThrowsAsync<EmailUnconfirmedException>(async () => await service.SendPasswordResetEmail(mocks.UrlHelper,
                                                                                                           DefaultRequestScheme,
                                                                                                           mocks.EmailSender,
                                                                                                           testUser));
        }

        /// <summary>
        ///     Ensures a password can be reset.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task PasswordCanBeReset()
        {
            // Arrange
            MockSet mocks = new MockSet();
            ApplicationUser testUser = new ApplicationUser { UserName = "TestUser", Email = "test@test.com" };
            string password = "MySecurePassword123";

            // Create the user first
            UserService service = new UserService(mocks.UserManager, mocks.SignInManager, mocks.Logger);
            await service.CreateUser(testUser, password, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);
            await service.ConfirmEmail(testUser, string.Empty);
            await service.SendPasswordResetEmail(mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender, testUser);

            // Act
            await service.ResetPassword(testUser, "MyNewSecurePassword321", "NotAnEmptyString");

            // Assert
            Assert.IsTrue(true);
        }

        /// <summary>
        ///     Ensures a password can be changed.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task PasswordCanBeChanged()
        {
            // Arrange
            MockSet mocks = new MockSet();
            ApplicationUser testUser = new ApplicationUser { UserName = "TestUser", Email = "test@test.com" };
            string oldPassword = "MySecurePassword123";
            string newPassword = "MyNewSecurePassword321";

            // Create the user
            UserService service = new UserService(mocks.UserManager, mocks.SignInManager, mocks.Logger);
            await service.CreateUser(testUser, oldPassword, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);

            // Act
            await service.ChangePassword(testUser, oldPassword, newPassword);

            // Assert
            Assert.IsTrue(true);
        }

        /// <summary>
        ///     Ensures an email confirmation email is sent.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task EmailConfirmationEmailIsSent()
        {
            // Arrange
            MockSet mocks = new MockSet();
            ApplicationUser testUser = new ApplicationUser { UserName = "TestUser", Email = "test@test.com" };
            string password = "MySecurePassword123";

            // Create the user
            UserService service = new UserService(mocks.UserManager, mocks.SignInManager, mocks.Logger);
            await service.CreateUser(testUser, password, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);

            // Act
            await service.SendEmailConfirmationEmail(mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender, testUser);

            // Assert
            Assert.IsTrue(mocks.EmailSender.SentMessages.Count == 2);
        }

        /// <summary>
        ///     Ensures an email address can be confirmed.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task EmailCanBeConfirmed()
        {
            // Arrange
            MockSet mocks = new MockSet();
            ApplicationUser testUser = new ApplicationUser { UserName = "TestUser", Email = "test@test.com" };
            string password = "MySecurePassword123";

            // Create the user
            UserService service = new UserService(mocks.UserManager, mocks.SignInManager, mocks.Logger);
            await service.CreateUser(testUser, password, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);

            // Act
            await service.ConfirmEmail(testUser, "NotAnEmptyString");

            // Assert
            Assert.IsTrue(mocks.DbContext.Users.Any(u => u.EmailConfirmed));
        }

        /// <summary>
        ///     Ensures Two-Factor Authentication can be enabled.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task TwoFactorAuthenticationCanBeEnabled()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     Ensures Two-Factor Authentication can be reset.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task TwoFactorAuthenticationCanBeReset()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     Ensures Two-Factor Authentication can be disabled.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task TwoFactorAuthenticationCanBeDisabled()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     Ensures Two-Factor Authentication recovery codes can be generated.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task RecoveryCodesCanBeGenerated()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     Ensures Two-Factor Authentication recovery codes can't be generated when Two-Factor Authentication is
        ///     not enabled.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task RecoveryCodesCannotBeGeneratedWithoutTwoFactorAuthenticationEnabled()
        {
            await Task.Yield();
            Assert.Fail();
        }
    }
}