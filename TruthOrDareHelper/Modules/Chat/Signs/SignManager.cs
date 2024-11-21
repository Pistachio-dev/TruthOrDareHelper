using DalamudBasics.Chat.Output;
using DalamudBasics.Configuration;
using DalamudBasics.Logging;
using DalamudBasics.Targeting;
using Model;
using System.Collections.Generic;
using System.Linq;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Modules.Chat.Signs
{
    public class SignManager : ISignManager
    {
        private string ClearMarkCommand { get; } = "/mk off";
        private const int TargetCommandDelay = 200;

        private static FfxivSign[] WinnerSigns = ((int[])[1, 2, 3, 4, 5, 6, 7, 6]).Select(number => new FfxivSign() { Text = $"attack{number}" }).ToArray();
        private static FfxivSign[] LoserSigns = [
            new FfxivSign() { Text = "bind1" },
            new FfxivSign() { Text = "bind2" },
            new FfxivSign() { Text = "bind3" },
            new FfxivSign() { Text = "ignore1" },
            new FfxivSign() { Text = "ignore2" },
            new FfxivSign() { Text = "square" },
            new FfxivSign() { Text = "triangle" },
            new FfxivSign() { Text = "cross" },
            new FfxivSign() { Text = "circle" },
        ];

        private readonly ILogService logService;
        private readonly ITargetingService targetingService;
        private readonly IToDChatOutput chatOutput;
        private readonly Configuration configuration;

        public SignManager(ILogService logService, ITargetingService targetingService, IToDChatOutput chatOutput, IConfigurationService<Configuration> configService)
        {
            this.logService = logService;
            this.targetingService = targetingService;
            this.chatOutput = chatOutput;
            this.configuration = configService.GetConfiguration();
        }

        public void ClearMarks(List<PlayerPair> markedPlayerPairs)
        {
            if (!configuration.MarkPlayers)
            {
                return;
            }
            foreach (var pair in markedPlayerPairs)
            {
                UnmarkPlayer(pair.Winner);
                if (pair.Loser != null)
                {
                    UnmarkPlayer(pair.Loser);
                }
            }

            ClearInUseFlags();
        }

        public void ApplyMarks(List<PlayerPair> playerPairsToMark)
        {
            if (!configuration.MarkPlayers)
            {
                return;
            }

            foreach (var pair in playerPairsToMark)
            {
                MarkPlayer(pair.Winner, true);
                if (pair.Loser != null)
                {
                    MarkPlayer(pair.Loser, false);
                }
            }
        }

        public void UnmarkPlayer(PlayerInfo player)
        {
            if (!configuration.MarkPlayers)
            {
                return;
            }
            chatOutput.WriteCommand(ClearMarkCommand, TargetCommandDelay, player.FullName);
        }

        public void MarkPlayer(PlayerInfo player, bool isWinner)
        {
            if (!configuration.MarkPlayers)
            {
                return;
            }

            chatOutput.WriteCommand(GetMarkCommand(isWinner), TargetCommandDelay, player.FullName);
        }

        private string GetMarkCommand(bool isForWinner)
        {
            FfxivSign[] pool = isForWinner ? WinnerSigns : LoserSigns;
            var sign = pool.FirstOrDefault(x => !x.InUse);
            if (sign == null)
            {
                logService.Warning($"All of the signs for {(isForWinner ? "winners" : "losers")} are spent!");
                return string.Empty;
            }

            sign.InUse = true;
            return $"/mk {sign.Text}";
        }

        private void ClearInUseFlags()
        {
            foreach (var sign in WinnerSigns.Concat(LoserSigns))
            {
                sign.InUse = false;
            }
        }

    }
}
