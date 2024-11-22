using Dalamud.Game.Text;
using DalamudBasics.Chat.Listener;
using System;
using TruthOrDareHelper.Modules.Chat.Interface;

namespace TruthOrDareHelper.Modules.Chat
{
    public class ToDChatListener : IToDChatListener
    {
        private readonly IChatListener chatListener;
        private readonly ICommandRunner commandRunner;

        public ToDChatListener(IChatListener chatListener, ICommandRunner commandRunner)
        {
            this.chatListener = chatListener;
            this.commandRunner = commandRunner;
        }

        public void AttachCommandDetector()
        {
            chatListener.AddPreprocessedMessageListener(DetectAndHandleCommands);
        }

        private void DetectAndHandleCommands(XivChatType type, string senderFullName, string message, DateTime receivedAt)
        {
            commandRunner.RunRelevantCommand(senderFullName, message);
        }
    }
}
