using System;
using MyCrowdCharger.Mobile.Api.Interfaces;

namespace MyCrowdCharger.Mobile.Api.Services
{
    public class XamarinLogService : ILog
    {
        public void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Warning(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Error(string message, Exception exception)
        {
            System.Diagnostics.Debug.Fail(message, exception.Message);
        }
    }
}
