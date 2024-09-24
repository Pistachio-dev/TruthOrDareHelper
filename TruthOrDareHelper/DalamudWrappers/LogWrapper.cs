using System;

namespace TruthOrDareHelper.DalamudWrappers
{
    public class LogWrapper
    {
        public void Info(string message)
        {
            Plugin.Log.Info(message);
        }

        public void Debug(string message)
        {
            Plugin.Log.Debug(message);
        }

        public void Warning(string message)
        {
            Plugin.Log.Warning(message);
        }

        public void Error(string message)
        {
            Plugin.Log.Error(message);
        }

        public void Error(Exception ex, string message)
        {
            Plugin.Log.Error(ex, message);
        }
    }
}
