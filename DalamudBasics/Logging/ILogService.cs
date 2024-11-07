using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalamudBasics.Logging
{
    public interface ILogService
    {
        void Debug(string message);
        void Error(Exception ex, string message);
        void Error(string message);
        void Fatal(Exception ex, string message);
        void Info(string message);
        void Tick(IFramework framework);
        void Warning(string message);
    }
}
