// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlightPlan.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities
{
    using System;

    /// <summary>
    ///     The Flight Plan.
    /// </summary>
    public class FlightPlan
    {
        /// <summary>
        ///     Gets or sets the ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="FlightRules"/>.
        /// </summary>
        public FlightPlanRules FlightRules { get; set; }

        /// <summary>
        ///     Gets or sets the aircraft type ICAO code.
        /// </summary>
        public string AircraftType { get; set; }

        /// <summary>
        ///     Gets or sets the planned true air speed.
        /// </summary>
        public string TrueAirSpeed { get; set; }

        /// <summary>
        ///     Gets or sets the planned altitude in feet (ft).
        /// </summary>
        public int Altitude { get; set; }

        /// <summary>
        ///     Gets or sets the ICAO code of the departure airport.
        /// </summary>
        public string DepartureIcao { get; set; }

        /// <summary>
        ///     Gets or sets the ICAO code of the arrival airport.
        /// </summary>
        public string ArrivalIcao { get; set; }

        /// <summary>
        ///     Gets or sets the ICAO code of the alternative airport.
        /// </summary>
        public string AlternateIcao { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DateTime"/> representing scheduled UTC time of departure.
        /// </summary>
        public DateTime? ScheduledDepartureTime { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DateTime"/> representing the actual UTC time of departure.
        /// </summary>
        /// <remarks>
        ///     Not provided by online networks. Must be calculated manually by OneSim.
        /// </remarks>
        public DateTime? ActualTimeOfDeparture { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="TimeSpan"/> representing the estimated amount of time enroute.
        /// </summary>
        public TimeSpan? EstimatedEnrouteTime { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="TimeSpan"/> representing the amount of fuel on board.
        /// </summary>
        public TimeSpan? Endurance { get; set; }

        /// <summary>
        ///     Gets or sets the route.
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        ///     Gets or sets the remarks.
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        ///     Gets the <see cref="DateTime"/> at which the flight is scheduled to arrive.
        /// </summary>
        public DateTime? ScheduledArrivalTime
        {
            get
            {
                if (ScheduledDepartureTime.HasValue &&
                    EstimatedEnrouteTime.HasValue)
                {
                    return ScheduledDepartureTime.Value.Add(EstimatedEnrouteTime.Value);
                }

                return null;
            }
        }
    }
}
