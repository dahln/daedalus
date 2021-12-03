using daedalus.Shared.Model;
using JWT.Algorithms;
using JWT.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace daedalus.Function.Utility
{
    static public class JWTUtility
    {
        static public List<Shared.Model.Condition> Decode(this CondtionsAsJwt jwt, string secret)
        {
            List<Shared.Model.Condition> results = new List<Condition>();
            try
            {
                //Decode JWT
                var conditionsFromJwt = JwtBuilder.Create()
                       .WithAlgorithm(new HMACSHA256Algorithm())
                       .WithSecret(secret)
                       .MustVerifySignature()
                       .Decode<Dictionary<string, Shared.Model.Condition>>(jwt.JWT);

                //JWT Claims come as a dictionary of objects. 
                foreach (var condition in conditionsFromJwt)
                {
                    Condition loggedCondition = new Condition()
                    {
                        LoggedAt = condition.Value.LoggedAt,
                        DegreesCelsius = condition.Value.DegreesCelsius,
                        HumidityPercentage = condition.Value.HumidityPercentage,
                        PressureMillibars = condition.Value.PressureMillibars
                    };
                    results.Add(loggedCondition);
                }

                return results;
            }
            catch
            {
                return results;
            }
        }
    }
}
