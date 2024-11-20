using DalamudBasics.Chat.Output;
using DalamudBasics.Logging;
using DalamudBasics.Targeting;
using Model;
using System.Collections.Generic;
using System.Linq;
using TruthOrDareHelper.Modules.Chat.Interface;

namespace TruthOrDareHelper.Modules.Chat.Signs
{
    public class SignManager : ISignManager
    {
        private string ClearMarkCommand { get; } = "/mk off";

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

        public SignManager(ILogService logService, ITargetingService targetingService, IToDChatOutput chatOutput)
        {
            this.logService = logService;
            this.targetingService = targetingService;
            this.chatOutput = chatOutput;
        }

        public void ClearMarks(List<PlayerPair> markedPlayerPairs)
        {
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
            foreach (var pair in playerPairsToMark)
            {
                MarkPlayer(pair.Winner, true);
                if (pair.Loser != null)
                {
                    MarkPlayer(pair.Loser, false);
                }
            }
        }

        public void MarkPlayer(PlayerInfo player, bool isWinner)
        {
            if (targetingService.TargetPlayer(player.FullName))
            {
                chatOutput.WriteCommand(GetMarkCommand(isWinner));
            }
        }

        public void UnmarkPlayer(PlayerInfo player)
        {
            if (targetingService.TargetPlayer(player.FullName))
            {
                chatOutput.WriteCommand(ClearMarkCommand);
            }
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
