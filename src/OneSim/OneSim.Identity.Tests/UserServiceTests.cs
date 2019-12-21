namespace OneSim.Identity.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;

    using OneSim.Identity.Application;
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
            UserService service = new UserService(mocks.UserManager, mocks.Logger);
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
            UserService service = new UserService(mocks.UserManager, mocks.Logger);
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
            UserService service = new UserService(mocks.UserManager, mocks.Logger);
            await service.CreateUser(testUser, password, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);
            await service.ConfirmEmail(testUser, "Not am empty string");

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
            UserService service = new UserService(mocks.UserManager, mocks.Logger);
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
            UserService service = new UserService(mocks.UserManager, mocks.Logger);
            await service.CreateUser(testUser, password, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);
            await service.ConfirmEmail(testUser, "Not an empty string");
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
            UserService service = new UserService(mocks.UserManager, mocks.Logger);
            await service.CreateUser(testUser, oldPassword, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);

            // Act
            string oldPasswordHash = testUser.PasswordHash;
            await service.ChangePassword(testUser, oldPassword, newPassword);
            string newPasswordHash = testUser.PasswordHash;

            // Assert
            Assert.AreNotEqual(oldPasswordHash, newPasswordHash);
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
            UserService service = new UserService(mocks.UserManager, mocks.Logger);
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
            UserService service = new UserService(mocks.UserManager, mocks.Logger);
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
            // Arrange
            MockSet mocks = new MockSet();
            ApplicationUser testUser = new ApplicationUser { UserName = "TestUser", Email = "test@test.com" };
            string password = "MySecurePassword123";

            // Create the user
            UserService service = new UserService(mocks.UserManager, mocks.Logger);
            await service.CreateUser(testUser, password, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);
            await service.ConfirmEmail(testUser, "NotAnEmptyString");

            // Act
            await service.EnableTwoFactorAuthentication(testUser, "NotAnEmptyString");
            
            // Assert
            Assert.IsTrue(mocks.DbContext.Users.Any(u => u.TwoFactorEnabled));
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
            // Arrange
            MockSet mocks = new MockSet();
            ApplicationUser testUser = new ApplicationUser { UserName = "TestUser", Email = "test@test.com" };
            string password = "MySecurePassword123";

            // Create the user
            UserService service = new UserService(mocks.UserManager, mocks.Logger);
            await service.CreateUser(testUser, password, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);
            await service.ConfirmEmail(testUser, "NotAnEmptyString");
            await service.EnableTwoFactorAuthentication(testUser, "NotAnEmptyString");

            // Act
            await service.ResetTwoFactorAuthentication(testUser);
            
            // Assert
            Assert.IsTrue(!mocks.DbContext.Users.Any(u => u.TwoFactorEnabled));
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
            // Arrange
            MockSet mocks = new MockSet();
            ApplicationUser testUser = new ApplicationUser { UserName = "TestUser", Email = "test@test.com" };
            string password = "MySecurePassword123";

            // Create the user
            UserService service = new UserService(mocks.UserManager, mocks.Logger);
            await service.CreateUser(testUser, password, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);
            await service.ConfirmEmail(testUser, "NotAnEmptyString");
            await service.EnableTwoFactorAuthentication(testUser, "NotAnEmptyString");

            // Act
            await service.DisableTwoFactorAuthentication(testUser);
            
            // Assert
            Assert.IsTrue(!mocks.DbContext.Users.Any(u => u.TwoFactorEnabled));
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
            // Arrange
            MockSet mocks = new MockSet();
            ApplicationUser testUser = new ApplicationUser { UserName = "TestUser", Email = "test@test.com" };
            string password = "MySecurePassword123";

            // Create the user
            UserService service = new UserService(mocks.UserManager, mocks.Logger);
            await service.CreateUser(testUser, password, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);
            await service.ConfirmEmail(testUser, "NotAnEmptyString");
            await service.EnableTwoFactorAuthentication(testUser, "NotAnEmptyString");

            // Act
            IEnumerable<string> codes = await service.GetRecoveryCodes(testUser);
            
            // Assert
            Assert.IsTrue(!codes.Any(string.IsNullOrEmpty));
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
            // Arrange
            MockSet mocks = new MockSet();
            ApplicationUser testUser = new ApplicationUser { UserName = "TestUser", Email = "test@test.com" };
            string password = "MySecurePassword123";

            // Create the user
            UserService service = new UserService(mocks.UserManager, mocks.Logger);
            await service.CreateUser(testUser, password, mocks.UrlHelper, DefaultRequestScheme, mocks.EmailSender);
            await service.ConfirmEmail(testUser, "NotAnEmptyString");

            // Act / Assert
            Assert.ThrowsAsync<Exception>(async () =>
            {
                await service.GetRecoveryCodes(testUser);
            });
        }
    }
}