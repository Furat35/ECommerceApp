using ECommerce.Service.Abstract;
using Serilog;

namespace ECommerce.Service.Concrete
{
    public class LoggerService : ILoggerService
    {
        private readonly ILogger _logger;

        public LoggerService(ILogger logger)
        {
            _logger = logger;
        }

        public void LogError(string message)
            => _logger.Error(message);

        public void LogWarning(string message)
            => _logger.Warning(message);

        public void LogInformation(string message)
            => _logger.Information(message);

        public void LogDebug(string message)
            => _logger.Debug(message);

        public void LogVerbose(string message)
            => _logger.Verbose(message);
    }
}
