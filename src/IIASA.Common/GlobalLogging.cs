using Microsoft.Extensions.Logging;

namespace IIASA.Common
{
    public class GlobalLogging
    {
        private static ILoggerFactory _factory;

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_factory != null)
                    return _factory;

                _factory = new LoggerFactory();

                return _factory;
            }
            set => _factory = value;
        }

        public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
    }
}