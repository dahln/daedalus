using daedalus.Server.Database;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace daedalus.Server
{

    public class SensorRunner : BackgroundService
    {
        private readonly ILogger<SensorRunner> _logger;
        public IConfiguration Configuration { get; }
        public IServiceProvider Services { get; }

        public SensorRunner(ILogger<SensorRunner> logger, IServiceProvider services, IConfiguration configuration)
        {
            Configuration = configuration;

            _logger = logger;
            Services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                //IHostedService is a singleton. It cannot consume scopped services.
                //Using the IServiceProvider, and a 'using' create a scope and a GetRequiredServices to create the scoped service
                using (var scope = Services.CreateScope())
                {
                    var _db = scope.ServiceProvider.GetRequiredService<daedalusDBContext>();

                    var i2cSettings = new I2cConnectionSettings(1, Bme280.SecondaryI2cAddress);
                    using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
                    using var bme280 = new Bme280(i2cDevice);

                    int measurementTime = bme280.GetMeasurementDuration();

                    while (true)
                    {
                        bme280.SetPowerMode(Bmx280PowerMode.Forced);
                        Thread.Sleep(measurementTime);

                        bme280.TryReadTemperature(out var tempValue);
                        bme280.TryReadPressure(out var preValue);
                        bme280.TryReadHumidity(out var humValue);
                        bme280.TryReadAltitude(out var altValue);

                        var condition = new Server.Database.Condition()
                        {
                            LoggedAt = DateTime.UtcNow,
                            DegreesCelsius = tempValue.DegreesCelsius,
                            PressureMillibars = preValue.Millibars,
                            HumidityPercentage = humValue.Percent
                        };

                        _db.Conditions.Add(condition);
                        await _db.SaveChangesAsync();

                        //Thread.Sleep(1000); //This works, but it is more often than I need
                        Thread.Sleep(60000); //New reading every 1 minute
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
