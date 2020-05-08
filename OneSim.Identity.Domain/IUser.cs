// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUser.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Domain
{
    /// <summary>
    ///     The interface representing an individual user.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        ///     Gets or sets the username of the current <see cref="IUser"/>.
        /// </summary>
        public string UserName { get; set; }
    }
}
