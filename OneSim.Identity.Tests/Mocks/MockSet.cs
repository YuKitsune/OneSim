// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockSet.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Tests.Mocks
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    using Moq;

    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Infrastructure;

    /// <summary>
    ///     A collection of pre-constructed mock objects.
    /// </summary>
    public class MockSet
    {
        /// <summary>
        ///     Gets the <see cref="IIdentityDbContext{TUser}"/>.
        /// </summary>
        public IIdentityDbContext<User> DbContext { get; }

        /// <summary>
        ///     Gets the <see cref="UserManager{TUser}"/> for the <see cref="User"/>.
        /// </summary>
        public UserManager<User> UserManager { get; }

        /// <summary>
        ///     Gets the <see cref="SignInManager{TUser}"/> for the <see cref="User"/>.
        /// </summary>
        public SignInManager<User> SignInManager { get; }

        /// <summary>
        ///     Gets the <see cref="MockEmailSender"/>.
        /// </summary>
        public MockEmailSender EmailSender { get; }

        /// <summary>
        ///     Gets the <see cref="ILogger{TCategoryName}"/> for the <typeparamref name="TLogCategory"/>.
        /// </summary>
        /// <typeparam name="TLogCategory">
        ///     The log category.
        /// </typeparam>
        /// <returns>
        ///     The mocked <see cref="ILogger{TCategoryName}"/> instance.
        /// </returns>
        public ILogger<TLogCategory> GetLogger<TLogCategory>() => new Mock<ILogger<TLogCategory>>().Object;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MockSet"/> class.
        /// </summary>
        public MockSet()
        {
            DbContext = Helpers.GetDbContext();
            UserManager = Helpers.GetUserManager(DbContext);
            SignInManager = Helpers.GetSignInManager(UserManager);
            EmailSender = new MockEmailSender();
        }
    }
}
