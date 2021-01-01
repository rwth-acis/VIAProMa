using System;

namespace i5.VIAProMa.DataModel
{
    /// <summary>
    /// Represents one entry on the GitHub punch card entry statistic
    /// </summary>
    [Serializable]
    public struct PunchCardEntry
    {
        /// <summary>
        /// The number of the weekday (for 0 to 6)
        /// </summary>
        public int day;
        /// <summary>
        /// The hour
        /// </summary>
        public int hour;
        /// <summary>
        /// The aggregated number of commits at the weekday and hour
        /// </summary>
        public int numberOfCommits;

        /// <summary>
        /// The weekday of this entry
        /// Converts the day number to a DayOfWeek object
        /// </summary>
        public DayOfWeek DayOfWeek
        {
            get { return (DayOfWeek)day; }
        }
    }
}