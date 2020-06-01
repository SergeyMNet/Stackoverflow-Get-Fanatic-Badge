using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StackoverflowGetFanaticBadge
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var isSuccess = false;

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    var chromeUrl = _configuration["RemoteWebDriverUrl"];
                    using (var browser = new BrowserEngine(chromeUrl))
                    {
                        var login = _configuration["Login"];
                        var pass = _configuration["Password"];
                        var url = "https://stackoverflow.com/";

                        isSuccess = browser.MakeLogin(login, pass, url);

                        if (isSuccess)
                        {
                            _logger.LogInformation($"Worker result - OK");
                        }
                        else
                        {
                            _logger.LogWarning($"Worker result - service couldn't log in, please check your credentials.");
                            // TODO: send an email if we have a problem
                        }
                    }
                }catch(Exception ex)
                {
                    _logger.LogError($"Worker throw an error: {ex.ToString()}");
                    isSuccess = false;
                }

                // if we get a successful result - repeat after 25 hours; 
                // if not, try to repeat after 1 hour
                await Task.Delay(isSuccess ?
                    (25 * 60 * 60 * 1000) : // 25h
                    (60 * 60 * 1000) // 1h
                    , stoppingToken);
            }
        }
    }
}
