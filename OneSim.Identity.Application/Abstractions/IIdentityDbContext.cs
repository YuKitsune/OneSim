// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdentityDbContext.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Application.Abstractions
{
    using Microsoft.EntityFrameworkCore;

    using OneSim.Identity.Domain;

    using Strato.Persistence.Abstractions;

    /// <summary>
    ///     The interface representing an <see cref="IDbContext"/> for storing users.
    /// </summary>
    /// <typeparam name="TUser">
    ///     The type of <see cref="IUser"/>.
    /// </typeparam>
    public interface IIdentityDbContext<TUser> : IDbContext
        where TUser : class, IUser
    {
        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <typeparamref name="TUser"/>s.
        /// </summary>
        DbSet<TUser> Users { get; set; }
    }
}
