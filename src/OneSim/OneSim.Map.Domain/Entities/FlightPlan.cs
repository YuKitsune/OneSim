namespace OneSim.Map.Domain.Entities
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
        ///     Gets or sets the <see cref="DateTime"/> representing estimated UTC time of departure.
        /// </summary>
        public DateTime? EstimatedTimeOfDeparture { get; set; }

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
        public TimeSpan? TimeEnroute { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="TimeSpan"/> representing the amount of fuel on board.
        /// </summary>
        public TimeSpan? FuelOnBoard { get; set; }

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
        public DateTime? ScheduledTimeOfArrival
        {
            get
            {
                if (EstimatedTimeOfDeparture.HasValue &&
                    TimeEnroute.HasValue)
                {
                    return EstimatedTimeOfDeparture.Value.Add(TimeEnroute.Value);
                }

                return null;
            }
        }
    }
}