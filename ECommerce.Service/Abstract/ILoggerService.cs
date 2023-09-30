namespace ECommerce.Service.Abstract
{
    public interface ILoggerService
    {
        void LogError(string message);
        void LogWarning(string message);
        void LogInformation(string message);
        void LogDebug(string message);
        void LogVerbose(string message);
    }
}
