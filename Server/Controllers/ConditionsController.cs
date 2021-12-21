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
using Microsoft.Extensions.Configuration;
using daedalus.Shared.Model;

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

        [Route("api/v1/condition/current")]
        [HttpGet]
        async public Task<IActionResult> CurrentCondition() 
        {
            var query = _db.Conditions.AsQueryable();

            var response = new Shared.Model.Condition();

            var anyResults = await query.AnyAsync();
            if (!anyResults)
                return Ok(response);

            var condition = await query.OrderByDescending(r => r.LoggedAt).FirstOrDefaultAsync();
            if(condition != null)
                response = condition.ToSharedCondition();

            return Ok(response);
        }

        [Route("api/v1/condition/search")]
        [HttpPost]
        async public Task<IActionResult> SearchCondition([FromBody] Search model) 
        {
            var query = _db.Conditions.Where(c => c.LoggedAt >= model.Start.ToUniversalTime() && c.LoggedAt <= model.End.ToUniversalTime());

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
                                        .Skip(model.Page * model.Size).Take(model.Size)
                                        .Select(r => r.ToSharedCondition()).ToListAsync();

            return Ok(response);
        }
    }
}
