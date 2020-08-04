// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoordinateComponent.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    using System;

    /// <summary>
    ///     The coordinate component.
    /// </summary>
    public class CoordinateComponent
    {
        /// <summary>
        ///     The number of digits to round to for the seconds component.
        /// </summary>
        public const int SecondsRoundingDigits = 3;

        /// <summary>
        ///     The number of digits to round to for the decimal value.
        /// </summary>
        public const int DecimalRoundingDigits = 7;

        /// <summary>
        ///     Gets the <see cref="CardinalDirection"/>.
        /// </summary>
        public CardinalDirection CardinalDirection { get; }

        /// <summary>
        ///     Gets the <see cref="CardinalAxis"/> which the current <see cref="CoordinateComponent"/> represents.
        /// </summary>
        public CardinalAxis CardinalAxis => CardinalDirection == CardinalDirection.North || CardinalDirection == CardinalDirection.South ?
                                                CardinalAxis.Latitude :
                                                CardinalAxis.Longitude;

        /// <summary>
        ///     Gets the degrees.
        /// </summary>
        public uint Degrees { get; }

        /// <summary>
        ///     Gets the minutes.
        /// </summary>
        public uint Minutes { get; }

        /// <summary>
        ///     Gets the seconds.
        /// </summary>
        public double Seconds { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CoordinateComponent"/> class.
        /// </summary>
        /// <param name="cardinalDirection">
        ///     The <see cref="CardinalDirection"/>.
        /// </param>
        /// <param name="degrees">
        ///     The degrees.
        /// </param>
        /// <param name="minutes">
        ///     The minutes.
        /// </param>
        /// <param name="seconds">
        ///     The seconds.
        /// </param>
        public CoordinateComponent(
            CardinalDirection cardinalDirection,
            uint degrees,
            uint minutes,
            double seconds)
        {
            CardinalDirection = cardinalDirection;
            Degrees = degrees;
            Minutes = minutes;
            Seconds = Math.Round(seconds, SecondsRoundingDigits);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CoordinateComponent"/> class.
        /// </summary>
        /// <param name="cardinalAxis">
        ///     The <see cref="CardinalAxis"/>.
        /// </param>
        /// <param name="decimalValue">
        ///     The decimal value.
        /// </param>
        public CoordinateComponent(CardinalAxis cardinalAxis, double decimalValue)
        {
            // Figure out the direction
            CardinalDirection = cardinalAxis switch
            {
                CardinalAxis.Latitude => decimalValue >= 0 ? CardinalDirection.North : CardinalDirection.South,
                CardinalAxis.Longitude => decimalValue >= 0 ? CardinalDirection.East : CardinalDirection.West,
                _ => throw new NotSupportedException($"The axis {cardinalAxis} is not supported.")
            };

            // We've got the direction, don't need the negative anymore
            decimalValue = Math.Abs(decimalValue);

            // Degrees = whole part of the decimal
            Degrees = (uint)Math.Truncate(decimalValue);

            // Minutes = remainder of above * 60
            Minutes = (uint)Math.Truncate(decimalValue - Degrees) * 60;

            // Seconds = remainder of above * 3600
            Seconds = Math.Round((decimalValue - Degrees - ((double)Minutes / 60)) * 3600, SecondsRoundingDigits);
        }

        /// <summary>
        ///     Parses the given DMS (Degrees, Minutes, Seconds) <see cref="string"/> into a
        ///     <see cref="CoordinateComponent"/>.
        /// </summary>
        /// <param name="dmsString">
        ///     The original DMS <see cref="string"/> to parse.
        /// </param>
        /// <returns>
        ///     The <see cref="CoordinateComponent"/> parsed from the <paramref name="dmsString"/>.
        /// </returns>
        public static CoordinateComponent Parse(string dmsString)
        {
            if (string.IsNullOrEmpty(dmsString)) throw new ArgumentNullException(nameof(dmsString));

            if (TryParse(
                dmsString,
                out CardinalDirection direction,
                out uint degrees,
                out uint minutes,
                out double seconds))
                return new CoordinateComponent(direction, degrees, minutes, seconds);

            // If we fall through to this line, then there was a problem with the value.
            throw new Exception($"Invalid formatting in lat/lon value: {dmsString}");
        }

        /// <summary>
        ///     Tries to parse the given DMS (Degrees, Minutes, Seconds) <see cref="string"/> into individual components.
        /// </summary>
        /// <param name="dmsString">
        ///     The original DMS <see cref="string"/> to parse.
        /// </param>
        /// <param name="direction">
        ///     The <see cref="CardinalDirection"/>.
        /// </param>
        /// <param name="degrees">
        ///     The degrees component.
        /// </param>
        /// <param name="minutes">
        ///     The minutes component.
        /// </param>
        /// <param name="seconds">
        ///     The seconds component.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="dmsString"/> could be parsed into individual components, <c>false</c>
        ///     otherwise.
        /// </returns>
        public static bool TryParse(
            string dmsString,
            out CardinalDirection direction,
            out uint degrees,
            out uint minutes,
            out double seconds)
        {
            if (!string.IsNullOrEmpty(dmsString))
            {
                // Get the cardinal direction
                char firstChar = dmsString[0];
                CardinalDirection? internalDirection = null;
                switch (char.ToUpper(firstChar))
                {
                    case 'N':
                        internalDirection = CardinalDirection.North;
                        break;

                    case 'E':
                        internalDirection = CardinalDirection.East;
                        break;

                    case 'S':
                        internalDirection = CardinalDirection.South;
                        break;

                    case 'W':
                        internalDirection = CardinalDirection.West;
                        break;
                }

                if (internalDirection.HasValue)
                {
                    direction = internalDirection.Value;

                    // Remove the first character
                    dmsString = dmsString.Substring(1);

                    // Get the whole degrees portion
                    char[] sep = { '.', ',' };
                    int pt1 = dmsString.IndexOfAny(sep);
                    if (pt1 > -1)
                    {
                        string deg = dmsString.Substring(0, pt1);

                        // Get the minutes portion.
                        int pt2 = dmsString.IndexOfAny(sep, pt1 + 1);
                        if (pt2 > -1)
                        {
                            string min = dmsString.Substring(pt1 + 1, (pt2 - pt1) - 1);

                            // Get the whole seconds portion.
                            int pt3 = dmsString.IndexOfAny(sep, pt2 + 1);
                            if (pt3 > -1)
                            {
                                string secWhole = dmsString.Substring(pt2 + 1, (pt3 - pt2) - 1);

                                // Get the partial seconds portion.
                                if (pt3 < dmsString.Length - 1)
                                {
                                    string secDec = dmsString.Substring(pt3 + 1);

                                    // Reassemble the seconds value.
                                    string sec = secWhole + "." + secDec;

                                    // Parse into numeric values.
                                    if (uint.TryParse(deg, out uint degreesOut) &&
                                        uint.TryParse(min, out uint minutesOut) &&
                                        double.TryParse(sec, out double secondsOut))
                                    {
                                        degrees = degreesOut;
                                        minutes = minutesOut;
                                        seconds = secondsOut;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Mission failed, we'll get em' next time
            direction = CardinalDirection.North;
            degrees = 0;
            minutes = 0;
            seconds = 0.0;
            return false;
        }

        /// <summary>
        ///     Gets the decimal value of the current <see cref="CoordinateComponent"/>.
        /// </summary>
        /// <returns>
        ///     The decimal value of the current <see cref="CoordinateComponent"/> in the form of a <see cref="double"/>.
        /// </returns>
        public double GetDecimal()
        {
            bool negative = CardinalDirection == CardinalDirection.South || CardinalDirection == CardinalDirection.West;

            double result = Degrees;
            result += Minutes / 60.0;
            result += Math.Round(Seconds, SecondsRoundingDigits) / 3600.0;

            // Return the result, negated if necessary.
            double value = Math.Round(negative ? (result * -1.0) : result, DecimalRoundingDigits);
            return value;
        }

        /// <summary>
        ///     Determines whether or not the <paramref name="obj"/> is equal to the current
        ///     <see cref="CoordinateComponent"/> instance.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="object"/> to compare.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="obj"/> has the same value as the current
        ///     <see cref="CoordinateComponent"/> instance, <c>false</c> otherwise.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (obj != null &&
                obj is CoordinateComponent coordinateComponent)
            {
                return CardinalDirection == coordinateComponent.CardinalDirection &&
                       Degrees == coordinateComponent.Degrees &&
                       Minutes == coordinateComponent.Minutes &&
                       Math.Abs(Seconds - coordinateComponent.Seconds) < 0.0001;
            }

            // If we made it to here, then we don't have a match
            return false;
        }

        /// <summary>
        ///     Gets the <see cref="string"/> representation of the current <see cref="CoordinateComponent"/>.
        /// </summary>
        /// <returns>
        ///     The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            char direction = CardinalDirection switch
            {
                CardinalDirection.North => 'N',
                CardinalDirection.East => 'E',
                CardinalDirection.South => 'S',
                CardinalDirection.West => 'W',
                _ => throw new NotSupportedException($"The direction {CardinalDirection} is not supported.")
            };

            return $"{direction}{Degrees:000}.{Minutes:00}.{Seconds:#0.000}";
        }
    }
}
