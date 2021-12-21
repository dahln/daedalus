using System;
using System.Linq;

namespace climatepi.Server.Utility
{
    static public class ToSharedModels
    {
        static public Shared.Model.Condition ToSharedCondition(this Database.Condition model)
        {
            var condition = new Shared.Model.Condition()
            {
                Id = model.Id,
                LoggedAt = model.LoggedAt,
                DegreesCelsius = model.DegreesCelsius,
                HumidityPercentage = model.HumidityPercentage,
                PressureMillibars = model.PressureMillibars
            };

            return condition;
        }
    }
}
