using System;

namespace Magnanibot.Exceptions
{
    /// <summary>
    ///   This exception type is never logged.
    /// </summary>
    public class BotException : Exception
    {
        public BotException()
        { }

        public BotException(string message)
            : base(message)
        { }

        public BotException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
