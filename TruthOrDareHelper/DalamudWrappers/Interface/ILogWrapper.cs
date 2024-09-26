using System;

namespace TruthOrDareHelper.DalamudWrappers.Interface
{
    public interface ILogWrapper
    {
        void Debug(string message);

        void Error(Exception ex, string message);

        void Error(string message);

        void Info(string message);

        void Warning(string message);
    }
}
