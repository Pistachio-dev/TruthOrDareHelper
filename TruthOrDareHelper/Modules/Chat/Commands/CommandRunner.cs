using DalamudBasics.Configuration;
using DalamudBasics.Logging;
using Model;
using TruthOrDareHelper.GameActions;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Modules.Chat.Commands
{
    public class CommandRunner : ICommandRunner
    {
        private readonly ITruthOrDareSession session;
        private readonly Configuration configuration;
        private readonly IToDChatOutput chatOutput;
        private readonly ILogService logService;

        private ChatCommandBase[] commands;

        public CommandRunner(IRunnerActions runnerActions, ITruthOrDareSession session, IConfigurationService<Configuration> configurationService, IToDChatOutput chatOutput, ILogService logService)
        {
            this.session = session;
            this.configuration = configurationService.GetConfiguration();
            this.chatOutput = chatOutput;
            this.logService = logService;

            commands =
            [
                new TruthCommand(session, configuration, chatOutput, logService),
                new DareCommand(session, configuration, chatOutput, logService),
                new DealersChoiceCommand(session, configuration, chatOutput, logService),
                new ConflipCommand(session, configuration, chatOutput, logService),
                new PasswordCommand(session, configuration, chatOutput, logService, runnerActions),
                new PromptCommand(runnerActions, session, configuration, chatOutput, logService)
            ];
        }

        public bool RunRelevantCommand(string sender, string message)
        {
            foreach (var command in commands)
            {
                if (command.ApplyIfMatched(sender, message))
                {
                    logService.Info($"Command of type {command.GetType()} matched and executed for sender \"{sender}\" and message \"{message}\"");
                    return true;
                }
            }

            return false;
        }
    }
}
