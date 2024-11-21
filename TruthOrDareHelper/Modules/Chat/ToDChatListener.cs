using Dalamud.Game.Text;
using DalamudBasics.Chat.Listener;
using DalamudBasics.Logging;
using Model;
using System;
using System.Linq;
using TruthOrDareHelper.Modules.Chat.Interface;

namespace TruthOrDareHelper.Modules.Chat
{
    public class ToDChatListener : IToDChatListener
    {
        private readonly IChatListener chatListener;
        private readonly ILogService logService;
        private readonly ITruthOrDareSession session;
        private readonly ICommandRunner commandRunner;

        private XivChatType[] channelsToReadCommandsIn = [XivChatType.Say, XivChatType.Shout, XivChatType.Yell, XivChatType.Party, XivChatType.Alliance];

        public ToDChatListener(IChatListener chatListener, ILogService logService, ITruthOrDareSession session, ICommandRunner commandRunner)
        {
            this.chatListener = chatListener;
            this.logService = logService;
            this.session = session;
            this.commandRunner = commandRunner;
        }

        public void AttachCommandDetector()
        {
            chatListener.AddPreprocessedMessageListener(DetectAndHandleCommands);
        }

        private void DetectAndHandleCommands(XivChatType type, string senderFullName, string message, DateTime receivedAt)
        {
            if (!channelsToReadCommandsIn.Contains(type))
            {
                return;
            }

            commandRunner.RunRelevantCommand(senderFullName, message);
        }
    }
}
