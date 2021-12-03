using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using daedalus.Function.Database;
using Microsoft.Extensions.DependencyInjection;
using daedalus.Function.Utility;
using System.Linq;
using daedalus.Shared.Model;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace daedalus.Function
{
    public static class Function
    {
        [FunctionName("Condition_Log")]
        public static async Task<IActionResult> ConditionLog(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/condition")] HttpRequest req,
            ILogger log)
        {
            MongoContext context = new MongoContext();

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            CondtionsAsJwt encryptedContent = JsonConvert.DeserializeObject<CondtionsAsJwt>(body);

            var decodedCondtions = encryptedContent.Decode(Environment.GetEnvironmentVariable("JWT_Key"));
            if (decodedCondtions.Count == 0)
                return new BadRequestResult();

            var condition = decodedCondtions.Select(c => c);
            await context.Conditions.InsertManyAsync(condition);

            return new OkObjectResult("Saved");
        }


        [FunctionName("Condition_Search")]
        public static async Task<IActionResult> ConditionSearch(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/condition/search")] HttpRequest req,
            ILogger log)
        {
            MongoContext context = new MongoContext();

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            Search model = JsonConvert.DeserializeObject<Search>(body);

            var query = await context.Conditions.Find(c => c.LoggedAt >= model.Start.ToUniversalTime() && c.LoggedAt <= model.End.ToUniversalTime()).ToListAsync();

            var response = new Shared.Model.ConditionSearchResponse();

            var anyResults = query.Any();
            if (!anyResults)
                return new OkObjectResult(response);

            response.HighTemperature = query.Max(s => s.DegreesCelsius);
            response.LowTemperature = query.Min(s => s.DegreesCelsius);
            response.AverageTemperature = query.Average(s => s.DegreesCelsius);
            response.HighHumidity = query.Max(s => s.HumidityPercentage);
            response.LowHumidity = query.Min(s => s.HumidityPercentage);
            response.AverageHumidity = query.Average(s => s.HumidityPercentage);
            response.HighPressure = query.Max(s => s.PressureMillibars);
            response.LowPressure = query.Min(s => s.PressureMillibars);
            response.AveragePressure = query.Average(s => s.PressureMillibars);

            response.Total = await context.Conditions.CountDocumentsAsync(c => c.LoggedAt >= model.Start.ToUniversalTime() && c.LoggedAt <= model.End.ToUniversalTime());

            response.Data = query.OrderByDescending(r => r.LoggedAt)
                                .Skip(model.Page * model.Size).Take(model.Size).ToList();

            return new OkObjectResult(response);
        }
    }
}
