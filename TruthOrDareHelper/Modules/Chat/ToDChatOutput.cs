using Dalamud.Plugin.Services;
using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.Chat.Output;
using DalamudBasics.Configuration;
using DalamudBasics.Extensions;
using DalamudBasics.Logging;
using DalamudBasics.Targeting;
using Model;
using System.Collections.Generic;
using System.Text;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Modules.Chat
{
    internal class ToDChatOutput : ChatOutput, IToDChatOutput
    {
        private readonly Configuration configuration;

        public ToDChatOutput(IConfigurationService<Configuration> configService, ILogService logService, IClientChatGui chatGui, IClientState clientState, ITargetingService targetingService)
            : base(configService.GetConfiguration(), logService, chatGui, clientState, targetingService)
        {
            this.configuration = configService.GetConfiguration();
        }

        public void ChatSoundWakeUp(PlayerInfo player)
        {
            WriteChat($"{player.FullName.WithoutWorldName()}, it's your turn! <se.9>");
        }

        public void TellWakeUp(PlayerInfo player)
        {
            WriteChat($"{player.FullName} It's your turn!", GetOutputTypeForTell());
        }

        public void WritePairs(List<PlayerPair> pairs)
        {
            foreach (PlayerPair p in pairs)
            {
                StringBuilder s = new StringBuilder(p.Winner.FullName.WithoutWorldName());
                s.Append(" [");
                s.Append(p.Winner.LastRollResult);
                s.Append(']');
                if (p.Loser != null)
                {
                    s.Append(" ");
                    s.Append(p.Loser.FullName.WithoutWorldName());
                    s.Append(" [");
                    s.Append(p.Loser.LastRollResult);
                    s.Append("]");
                    if (configuration.WriteAcceptedChallenges)
                    {
                        s.Append($"Accepts: Truth: {GetAcceptanceMessage(p.Loser.AcceptsSfwTruth, p.Loser.AcceptsNsfwTruth)}" +
                        $" Dare: {GetAcceptanceMessage(p.Loser.AcceptsSfwDare, p.Loser.AcceptsNsfwDare)}");
                    }
                }
                else
                {
                    s.Append(", choose your \"victim\".");
                }
                if (p.ChallengeType == ChallengeType.None)
                {
                    s.Append("<se.5>");
                }

                if (p.ChallengeType != ChallengeType.None)
                {
                    s.Append($" Chose: {(p.ChallengeType == ChallengeType.Truth ? "Truth!" : "Dare!")}.");
                }
                if (p.Done)
                {
                    s.Append(" They're done!");
                }

                WriteChat(s.ToString(), null, 1000);
            }
        }

        private string GetAcceptanceMessage(bool sfwFlag, bool nsfwFlag)
        {
            if (sfwFlag)
            {
                if (nsfwFlag)
                {
                    return "Any";
                }

                return "SFW only";
            }
            if (nsfwFlag)
            {
                return "NSFW only";
            }

            return "Hasn't decided yet";
        }
    }
}
