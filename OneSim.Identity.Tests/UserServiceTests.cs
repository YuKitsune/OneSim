// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserServiceTests.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using NUnit.Framework;

    using OneSim.Identity.Application.Exceptions;
    using OneSim.Identity.Infrastructure;
    using OneSim.Identity.Tests.Mocks;

    /// <summary>
    ///     The <see cref="UserService"/> Tests.
    /// </summary>
    [TestFixture]
    public class UserServiceTests
    {
        /// <summary>
        ///     Gets the default request scheme.
        /// </summary>
        public static string DefaultRequestScheme => "HTTPS";

        /// <summary>
        ///     Gets the password.
        /// </summary>
        public static string Password => "PassWord123!@#";

        /// <summary>
        ///     Gets a dummy user to test with.
        /// </summary>
        public static User TestUser => new User { Email = "test@test.com" };

        /// <summary>
        ///     Ensures a new user can register.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task UserCanRegister()
        {
            // Arrange
            MockSet mocks = new MockSet();

            // Act
            UserService service = new UserService(
                mocks.UserManager,
                mocks.DbContext,
                mocks.EmailSender,
                mocks.GetLogger<UserService>());
            await service.RegisterUserAsync(TestUser, Password);

            // Assert
            Assert.IsTrue(mocks.DbContext.Users.Any(u => u.Email == TestUser.Email));
        }

        /// <summary>
        ///     Ensures a password reset email can be sent.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task PasswordResetEmailCanBeSent()
        {
            // Arrange
            MockSet mocks = new MockSet();

            // Create the user first
            UserService service = new UserService(
                mocks.UserManager,
                mocks.DbContext,
                mocks.EmailSender,
                mocks.GetLogger<UserService>());
            await service.RegisterUserAsync(TestUser, Password);

            // Get the user
            User testUser = await mocks.DbContext.Users.FirstAsync();

            await service.VerifyEmailAsync(testUser, "Not am empty string");

            // Act
            await service.SendPasswordResetEmailAsync(testUser);

            // Assert
            Assert.AreEqual(1, mocks.EmailSender.SentMessages.Count);
        }

        /// <summary>
        ///     Ensures a password reset email is not sent when the email has not been confirmed.
        ///     Also ensures this can be overridden.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task PasswordResetEmailIsNotSentToUnConfirmedEmail()
        {
            // Arrange
            MockSet mocks = new MockSet();

            // Create the user first
            UserService service = new UserService(
                mocks.UserManager,
                mocks.DbContext,
                mocks.EmailSender,
                mocks.GetLogger<UserService>());
            await service.RegisterUserAsync(TestUser, Password);

            // Act / Assert
            Assert.ThrowsAsync<UnverifiedEmailException>(
                async () =>
                    await service.SendPasswordResetEmailAsync(TestUser));
            Assert.DoesNotThrowAsync(async () => await service.SendPasswordResetEmailAsync(TestUser, true));
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

            // Create the user first
            UserService service = new UserService(
                mocks.UserManager,
                mocks.DbContext,
                mocks.EmailSender,
                mocks.GetLogger<UserService>());
            await service.RegisterUserAsync(TestUser, Password);
            User testUser = await mocks.DbContext.Users.FirstAsync();

            await service.VerifyEmailAsync(testUser, "Not an empty string");
            await service.SendPasswordResetEmailAsync(testUser);

            // Act
            await service.ResetPasswordAsync(testUser, "MyNewSecurePassword321", "NotAnEmptyString");

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
            string oldPassword = "MySecurePassword123";
            string newPassword = "MyNewSecurePassword321";

            // Create the user
            UserService service = new UserService(
                mocks.UserManager,
                mocks.DbContext,
                mocks.EmailSender,
                mocks.GetLogger<UserService>());
            await service.RegisterUserAsync(TestUser, oldPassword);

            // Store them for later
            User testUser = await mocks.DbContext.Users.FirstAsync();

            // Act
            string oldPasswordHash = testUser.PasswordHash;
            await service.ChangePasswordAsync(testUser, oldPassword, newPassword);
            string newPasswordHash = testUser.PasswordHash;

            // Assert
            Assert.AreNotEqual(oldPasswordHash, newPasswordHash);
        }

        /// <summary>
        ///     Ensures a verification email is sent.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task VerificationEmailIsSent()
        {
            // Arrange
            MockSet mocks = new MockSet();

            // Create the user
            UserService service = new UserService(
                mocks.UserManager,
                mocks.DbContext,
                mocks.EmailSender,
                mocks.GetLogger<UserService>());
            await service.RegisterUserAsync(TestUser, Password);

            // Act
            await service.SendVerificationEmailAsync(TestUser);

            // Assert
            Assert.AreEqual(1, mocks.EmailSender.SentMessages.Count);
        }

        /// <summary>
        ///     Ensures an email address can be verified.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task EmailCanBeConfirmed()
        {
            // Arrange
            MockSet mocks = new MockSet();

            // Create the user
            UserService service = new UserService(
                mocks.UserManager,
                mocks.DbContext,
                mocks.EmailSender,
                mocks.GetLogger<UserService>());
            await service.RegisterUserAsync(TestUser, Password);

            // Act
            await service.VerifyEmailAsync(TestUser, "NotAnEmptyString");

            // Assert
            Assert.IsTrue(mocks.DbContext.Users.Any(u => u.EmailConfirmed));
        }
    }
}
