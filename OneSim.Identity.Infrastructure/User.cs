// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Infrastructure
{
    using Microsoft.AspNetCore.Identity;

    using OneSim.Identity.Domain;

    /// <summary>
    ///     The implementation of <see cref="IUser"/> based on the <see cref="IdentityUser"/>.
    /// </summary>
    public class User : IdentityUser, IUser
    {
    }
}
