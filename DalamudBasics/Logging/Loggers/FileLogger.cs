using System.IO;

namespace DalamudBasics.Logging.Loggers
{
    internal class FileLogger : IFileLogger
    {
        private readonly string fileRoute;

        public FileLogger(string filePath)
        {
            this.fileRoute = filePath;
        }

        public void Log(string message)
        {
            using (StreamWriter sw = File.AppendText(fileRoute))
            {
                sw.WriteLine(message);
            }
        }
    }
}
