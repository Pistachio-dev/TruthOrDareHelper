using Dalamud.Plugin.Services;
using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.Chat.Output;
using DalamudBasics.Configuration;
using DalamudBasics.Logging;
using Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruthOrDareHelper.Modules.Chat.Interface;

namespace TruthOrDareHelper.Modules.Chat
{
    internal class ToDChatOutput : ChatOutput, IToDChatOutput
    {
        public ToDChatOutput(IConfiguration configuration, ILogService logService, IClientChatGui chatGui, IClientState clientState)
            : base(configuration,logService, chatGui, clientState)
        {
        }

        public void WritePairs(List<PlayerPair> pairs)
        {
            foreach (PlayerPair p in pairs)
            {
                StringBuilder s = new StringBuilder(RemoveWorldFromName(p.Winner.FullName));
                if (p.Loser != null)
                {
                    s.Append("->");
                    s.Append(RemoveWorldFromName(p.Loser.FullName));
                }
                else
                {
                    s.Append(", choose your victim.");
                }

                if (p.ChallengeType != ChallengeType.None)
                {
                    s.Append($" Chose: {(p.ChallengeType == ChallengeType.Truth ? "Truth!" : "Dare!")}.");
                }
                if (p.Done)
                {
                    s.Append(" They're done!");
                }

                WriteChat(s.ToString());
            }
        }

        public static string RemoveWorldFromName(string name)
        {
            return name.Split("@").First();
        }
    }
}
