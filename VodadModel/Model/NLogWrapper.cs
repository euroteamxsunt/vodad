using System.IO;
using NLog;
using NLog.Targets.Wrappers;
using NLog.Targets;

namespace VodadModel
{
    public class NLogWrapper
    {
        private static NLog.Logger logger;

        public static NLog.Logger Logger
        {
            get
            {
                if (logger == null)
                {
                    var traceAll = false;
#if(DEBUG)
                    traceAll = true;
#endif
                    NLogWrapper.LoadLoggers(NLog.LogLevel.Info, System.IO.Path.Combine("${basedir}", "logs"), "VodadLogs", traceAll);
                    logger = NLog.LogManager.GetCurrentClassLogger();
                }
                return logger;
            }
        }

        public static void LoadLoggers(LogLevel logLevel, string filePath, string fileName, bool includeTraceOnAll)
        {
            //Create targets
            var vsTarget = new DebuggerTarget
            {
                Layout =
                    "${longdate} ${windows-identity}:${identity} ${processtime} ${threadname}(${threadid}) ${level:uppercase=true}${newline}${stacktrace}${newline}${message}${newline}${exception:format=ToString:maxInnerExceptionLevel=5}",
                Name = "VsTarget"
            };


            var file = new FileTarget
            {
                Layout =
                    "${longdate} ${windows-identity}:${identity} ${processtime} ${threadname}(${threadid}) ${level:uppercase=true}${newline}${stacktrace}${newline}${message}${newline}${exception:format=ToString:maxInnerExceptionLevel=5}",
                FileName = Path.Combine(filePath, fileName + ".log"),
                ArchiveFileName = Path.Combine(filePath, fileName + ".{#}.log"),
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.Rolling,
                KeepFileOpen = false,
                CreateDirs = true,
                LineEnding = LineEndingMode.Default,
                Name = "File"
            };

            var consoleFileAndUdp = new SplitGroupTarget(vsTarget, file) { Name = "ConsoleAndFile" };

            //Add the Loggers to the config
            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(consoleFileAndUdp, logLevel);


            //Make sure any loggers sitting out there are updated to the new settings.
            LogManager.ReconfigExistingLoggers();
        }

        public static void LoadLoggersWithTraceLevel(string filePath, string fileName, bool includeTraceOnAll)
        {
            LoadLoggers(LogLevel.Trace, filePath, fileName, includeTraceOnAll);
        }

    }
}