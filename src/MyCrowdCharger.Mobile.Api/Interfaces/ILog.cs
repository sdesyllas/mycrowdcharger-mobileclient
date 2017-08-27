using System;

namespace MyCrowdCharger.Mobile.Api.Interfaces
{
    public interface ILog
    {
        void Info(string message);

        void Debug(string message);

        void Warning(string message);

        void Error(string message, Exception exception);
    }
}
