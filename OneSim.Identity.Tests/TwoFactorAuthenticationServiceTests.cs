// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TwoFactorAuthenticationServiceTests.cs" company="Strato Systems Pty. Ltd.">
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

    using OneSim.Identity.Infrastructure;
    using OneSim.Identity.Tests.Mocks;

    /// <summary>
    ///     The <see cref="TwoFactorAuthenticationService"/> Tests.
    /// </summary>
    [TestFixture]
    public class TwoFactorAuthenticationServiceTests
    {
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

            // Create the service
            TwoFactorAuthenticationService service = new TwoFactorAuthenticationService(
                mocks.SignInManager,
                mocks.UserManager,
                mocks.GetLogger<TwoFactorAuthenticationService>());

            // Create the user
            UserService userService = new UserService(
                mocks.UserManager,
                mocks.DbContext,
                mocks.EmailSender,
                mocks.GetLogger<UserService>());

            await userService.RegisterUserAsync(UserServiceTests.TestUser, UserServiceTests.Password);

            // Get the user
            User testUser = await mocks.DbContext.Users.FirstAsync();

            await userService.VerifyEmailAsync(testUser, "NotAnEmptyString");

            // Act
            await service.EnableTwoFactorAuthenticationAsync(testUser, "NotAnEmptyString");

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

            // Create the service
            TwoFactorAuthenticationService service = new TwoFactorAuthenticationService(
                mocks.SignInManager,
                mocks.UserManager,
                mocks.GetLogger<TwoFactorAuthenticationService>());

            // Create the user
            UserService userService = new UserService(
                mocks.UserManager,
                mocks.DbContext,
                mocks.EmailSender,
                mocks.GetLogger<UserService>());
            await userService.RegisterUserAsync(UserServiceTests.TestUser, UserServiceTests.Password);

            // Get the user
            User testUser = await mocks.DbContext.Users.FirstAsync();
            await userService.VerifyEmailAsync(testUser, "NotAnEmptyString");
            await service.EnableTwoFactorAuthenticationAsync(testUser, "NotAnEmptyString");

            // Act
            await service.ResetTwoFactorAuthenticationAsync(testUser);

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

            // Create the service
            TwoFactorAuthenticationService service = new TwoFactorAuthenticationService(
                mocks.SignInManager,
                mocks.UserManager,
                mocks.GetLogger<TwoFactorAuthenticationService>());

            // Create the user
            UserService userService = new UserService(
                mocks.UserManager,
                mocks.DbContext,
                mocks.EmailSender,
                mocks.GetLogger<UserService>());

            await userService.RegisterUserAsync(UserServiceTests.TestUser, UserServiceTests.Password);

            // Get the user
            User testUser = await mocks.DbContext.Users.FirstAsync();

            await userService.VerifyEmailAsync(testUser, "NotAnEmptyString");
            await service.EnableTwoFactorAuthenticationAsync(testUser, "NotAnEmptyString");

            // Act
            IEnumerable<string> codes = await service.GetNewRecoveryCodesAsync(testUser);

            // Assert
            Assert.IsTrue(!codes.Any(string.IsNullOrEmpty));
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

            // Create the service
            TwoFactorAuthenticationService service = new TwoFactorAuthenticationService(
                mocks.SignInManager,
                mocks.UserManager,
                mocks.GetLogger<TwoFactorAuthenticationService>());

            // Create the user
            UserService userService = new UserService(
                mocks.UserManager,
                mocks.DbContext,
                mocks.EmailSender,
                mocks.GetLogger<UserService>());

            await userService.RegisterUserAsync(UserServiceTests.TestUser, UserServiceTests.Password);

            // Get the user
            User testUser = await mocks.DbContext.Users.FirstAsync();

            await userService.VerifyEmailAsync(testUser, "NotAnEmptyString");
            await service.EnableTwoFactorAuthenticationAsync(testUser, "NotAnEmptyString");

            // Act
            await service.DisableTwoFactorAuthenticationAsync(testUser);

            // Assert
            Assert.IsTrue(!mocks.DbContext.Users.Any(u => u.TwoFactorEnabled));
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

            // Create the service
            TwoFactorAuthenticationService service = new TwoFactorAuthenticationService(
                mocks.SignInManager,
                mocks.UserManager,
                mocks.GetLogger<TwoFactorAuthenticationService>());

            // Create the user
            UserService userService = new UserService(
                mocks.UserManager,
                mocks.DbContext,
                mocks.EmailSender,
                mocks.GetLogger<UserService>());
            await userService.RegisterUserAsync(UserServiceTests.TestUser, UserServiceTests.Password);
            await userService.VerifyEmailAsync(UserServiceTests.TestUser, "NotAnEmptyString");

            // Act / Assert
            Assert.ThrowsAsync<Exception>(async () => await service.GetNewRecoveryCodesAsync(UserServiceTests.TestUser));
        }
    }
}
