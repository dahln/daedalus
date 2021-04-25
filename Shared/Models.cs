using System;
using System.Collections.Generic;
using System.Text;

namespace daedalus.Shared.Model
{
    /// <summary>
    /// "Conditions" meaning this JWT could contain multiple conditions.
    /// </summary>
    public class CondtionsAsJwt
    {
        public string JWT { get; set; }
    }

    public class Condition
    {
        public string Id { get; set; }
        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
        public double DegreesCelsius { get; set; }
        public double PressureMillibars { get; set; }
        public double HumidityPercentage { get; set; }
    }

    public class Period
    {
        public PeriodType PeriodType { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class Search
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Page { get; set; }
        public int Size { get; set; } = 10;
    }

    public class ConditionSearchResponse
    {
        public List<Condition> Data { get; set; } = new List<Condition>();
        public int Total { get; set; }

        public double LowTemperature { get; set; }
        public double HighTemperature { get; set; }
        public double AverageTemperature { get; set; }

        public double LowHumidity { get; set; }
        public double HighHumidity { get; set; }
        public double AverageHumidity { get; set; }

        public double LowPressure { get; set; }
        public double HighPressure { get; set; }
        public double AveragePressure { get; set; }
    }
}
