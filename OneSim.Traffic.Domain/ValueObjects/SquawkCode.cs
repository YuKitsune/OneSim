// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SquawkCode.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.ValueObjects
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json.Serialization;

    using OneSim.Traffic.Domain.Exceptions;
    using OneSim.Traffic.Domain.ValueObjects.Converters;

    /// <summary>
    ///     The Squawk Code Value Object.
    /// </summary>
    [JsonConverter(typeof(SquawkCodeConverter))]
    public struct SquawkCode
    {
        /// <summary>
        ///     The digits of the squawk code.
        /// </summary>
        private readonly IReadOnlyList<int> _digits;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SquawkCode"/> struct.
        /// </summary>
        /// <param name="code">
        ///     The squawk code in the form of a <see cref="string"/>.
        /// </param>
        public SquawkCode(string code)
        {
            // Check for empty string
            if (string.IsNullOrEmpty(code)) throw new InvalidSquawkCodeException(code, "Squawk codes cannot be null or empty.");

            // Check the length is correct
            if (code.Length > 4) throw new InvalidSquawkCodeException(code, "Squawk codes cannot be more than 4 characters long.");
            if (code.Length < 4) throw new InvalidSquawkCodeException(code, "Squawk codes cannot be less than 4 characters long.");

            // Get each character
            char[] codeChars = code.ToCharArray();

            // Convert each character to an int and store them in an array
            int[] digits = new int[codeChars.Length];
            for (int i = 0; i < codeChars.Length; i++)
            {
                int currentInt = (int)char.GetNumericValue(codeChars[i]);

                // Check it's between 0 and 7
                if (currentInt < 0 ||
                    currentInt > 7)
                    throw new InvalidSquawkCodeException(code, "All digits in squawk codes must be between 0 and 7.");

                digits[i] = currentInt;
            }

            // Set the digits property as readonly
            _digits = new List<int>(digits).AsReadOnly();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SquawkCode"/> struct.
        /// </summary>
        /// <param name="code">
        ///     The squawk code in the form of an <see cref="int"/>.
        /// </param>
        public SquawkCode(int code)
            : this(code.ToString("0000"))
        {
        }

        /// <summary>
        ///     Gets the <see cref="SquawkCode"/> indicating an emergency.
        /// </summary>
        public static SquawkCode Emergency => new SquawkCode("7700");

        /// <summary>
        ///     Gets the <see cref="SquawkCode"/> indicating a radio failure.
        /// </summary>
        public static SquawkCode RadioFailure => new SquawkCode("7600");

        /// <summary>
        ///     Gets the <see cref="SquawkCode"/> indicating a hijack.
        /// </summary>
        public static SquawkCode Hijack => new SquawkCode("7500");

        /// <summary>
        ///     Gets the <see cref="string"/> representation of the current <see cref="SquawkCode"/>.
        /// </summary>
        /// <returns>
        ///        The Squawk code in the form of a <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            char[] squawkChars = new char[_digits.Count];
            for (int i = 0; i < _digits.Count; i++) squawkChars[i] = _digits[i].ToString().ToCharArray()[0];

            return new string(squawkChars);
        }

        /// <summary>
        ///     Implicitly converts a <see cref="SquawkCode"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="code">
        ///        The <see cref="SquawkCode"/>.
        /// </param>
        /// <returns>
        ///        The <see cref="string"/>.
        /// </returns>
        public static implicit operator string(SquawkCode code) => code.ToString();

        /// <summary>
        ///     Implicitly converts a <see cref="string"/> to a <see cref="SquawkCode"/>.
        /// </summary>
        /// <param name="code">
        ///     The <see cref="string"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="SquawkCode"/>.
        /// </returns>
        public static implicit operator SquawkCode(string code) => new SquawkCode(code);

        /// <summary>
        ///     Implicitly converts a <see cref="SquawkCode"/> to an <see cref="int"/>.
        /// </summary>
        /// <param name="code">
        ///     The <see cref="SquawkCode"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="int"/>.
        /// </returns>
        public static implicit operator int(SquawkCode code) => int.Parse(code.ToString());

        /// <summary>
        ///     Implicitly converts an <see cref="int"/> to a <see cref="SquawkCode"/>.
        /// </summary>
        /// <param name="code">
        ///     The <see cref="int"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="SquawkCode"/>.
        /// </returns>
        public static implicit operator SquawkCode(int code) => new SquawkCode(code);

        /// <summary>
        ///     Determines if the object on the left hand side is the same as the one on the right hand side.
        /// </summary>
        /// <param name="obj1">
        ///        The object on the left hand side.
        /// </param>
        /// <param name="obj2">
        ///        The object on the right hand side.
        /// </param>
        /// <returns>
        ///        Whether or not the object on the left hand side is the same as the one on the right hand side.
        /// </returns>
        public static bool operator ==(SquawkCode obj1, SquawkCode obj2) => obj1.Equals(obj2);

        /// <summary>
        ///     Determines if the object on the left hand side is different to the one on the right hand side.
        /// </summary>
        /// <param name="obj1">
        ///        The object on the left hand side.
        /// </param>
        /// <param name="obj2">
        ///        The object on the right hand side.
        /// </param>
        /// <returns>
        ///        Whether or not the object on the left hand side is different to the one on the right hand side.
        /// </returns>
        public static bool operator !=(SquawkCode obj1, SquawkCode obj2) => !(obj1 == obj2);

        /// <summary>
        ///     Determines whether or not the given <paramref name="obj"/> is equal to the current <see cref="SquawkCode"/>.
        /// </summary>
        /// <param name="obj">
        ///        The <see cref="object"/> to compare.
        /// </param>
        /// <returns>
        ///        Whether or not the given <paramref name="obj"/> is equal to the current <see cref="SquawkCode"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj switch
            {
                null => false,
                string s => ToString() == s,
                int i => (int)this == i,
                SquawkCode code => _digits?.SequenceEqual(code._digits) ?? false,
                _ => false
            };
        }

        /// <summary>
        ///     Gets the hash code of the current <see cref="SquawkCode"/>.
        /// </summary>
        /// <returns>
        ///        The hash code of the current <see cref="SquawkCode"/>.
        /// </returns>
        public override int GetHashCode() => this * _digits.Sum();
    }
}
