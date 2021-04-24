using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using daedalus.Shared;
using daedalus.Server.Database;
using Microsoft.EntityFrameworkCore;
using daedalus.Server.Utility;
using JWT.Builder;
using JWT.Algorithms;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace daedalus.Server.Controllers
{
    [ApiController]
    public class ConditionsController : ControllerBase
    {
        private readonly daedalusDBContext _db;
        private static IConfiguration _configuration;

        public ConditionsController(IConfiguration configuration, daedalusDBContext db)
        {
            _db = db;
            _configuration = configuration;
        }

        [Route("api/v1/condition")]
        [HttpPost]
        async public Task<IActionResult> LogCondition([FromBody] Shared.Model.CondtionsAsJwt model)
        {
            var conditions = model.Decode(_configuration["JWT_Key"]);
            if (conditions.Count == 0)
                return BadRequest("No Conditions Saved");

            var dbConditions = conditions.Select(c => c.ToCondition());

            _db.Conditions.AddRange(dbConditions);
            await _db.SaveChangesAsync();

            return Ok();
        }

        // [Route("api/v1/condition/clear")]
        // [HttpGet]
        // async public Task<IActionResult> Clear() 
        // {
        //     var allConditions = _db.Conditions.Where(c => c.Id != null);
        //     _db.Conditions.RemoveRange(allConditions);
        //     await _db.SaveChangesAsync();

        //     return Ok("Cleared");
        // }

        [Route("api/v1/condition/search/{start}/{end}/{offset}/{startDaylightSavingsOffset}/{endDaylightSavingsOffset}/{page}/{size}")]
        [HttpGet]
        async public Task<IActionResult> SearchCondition(long start, long end, long offset, long startDaylightSavingsOffset, long endDaylightSavingsOffset, int page, int size) 
        {
            offset *= -1;
            startDaylightSavingsOffset *= -1;
            endDaylightSavingsOffset *= -1;
            DateTime startFilter = new DateTime(start).AddTicks(offset).AddTicks(startDaylightSavingsOffset);
            DateTime endFilter = new DateTime(end).AddTicks(offset).AddTicks(endDaylightSavingsOffset);

            var query = _db.Conditions.Where(c => c.LoggedAt >= startFilter && c.LoggedAt < endFilter);

            var response = new Shared.Model.ConditionSearchResponse();

            var anyResults = await query.AnyAsync();
            if (!anyResults)
                return Ok(response);

            response.HighTemperature = await query.MaxAsync(s => s.DegreesCelsius);
            response.LowTemperature = await query.MinAsync(s => s.DegreesCelsius);
            response.AverageTemperature = await query.AverageAsync(s => s.DegreesCelsius);
            response.HighHumidity = await query.MaxAsync(s => s.HumidityPercentage);
            response.LowHumidity = await query.MinAsync(s => s.HumidityPercentage);
            response.AverageHumidity = await query.AverageAsync(s => s.HumidityPercentage);
            response.HighPressure = await query.MaxAsync(s => s.PressureMillibars);
            response.LowPressure = await query.MinAsync(s => s.PressureMillibars);
            response.AveragePressure = await query.AverageAsync(s => s.PressureMillibars);

            response.Total = await query.CountAsync();

            response.Data = await query.OrderByDescending(r => r.LoggedAt)
                                                            .Skip(page * size).Take(size)
                                                            .Select(r => r.ToSharedCondition()).ToListAsync();

            return Ok(response);
        }
    }
}
