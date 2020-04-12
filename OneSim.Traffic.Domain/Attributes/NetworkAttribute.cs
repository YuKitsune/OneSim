// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkAttribute.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Attributes
{
    using System;

    using OneSim.Traffic.Domain.Entities;

    /// <summary>
    ///     The <see cref="Attribute"/> used to specify which OFSN(s) the target <c>class</c> is restricted to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class NetworkAttribute : Attribute
    {
        // Todo: Attribute holder? Doesn't sound right...

        /// <summary>
        ///     Gets the <see cref="NetworkType"/> which the current <see cref="Attribute"/> holder is valid for.
        /// </summary>
        public NetworkType Type { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkAttribute"/> class.
        /// </summary>
        /// <param name="type">
        ///        The <see cref="NetworkType"/>.
        /// </param>
        public NetworkAttribute(NetworkType type) => Type = type;
    }
}
