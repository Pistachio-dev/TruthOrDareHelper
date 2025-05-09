using Dalamud.Utility;
using DalamudBasics.Logging;
using Model;
using System.Text.RegularExpressions;
using TruthOrDareHelper.Modules.Chat.Interface;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Modules.Chat.Commands
{
    internal abstract class ChatCommandBase
    {
        protected readonly ITruthOrDareSession session;
        protected readonly Configuration configuration;
        protected readonly IToDChatOutput chatOutput;
        protected readonly ILogService logService;

        protected ChatCommandBase(ITruthOrDareSession session, Configuration configuration, IToDChatOutput chatOutput, ILogService logService)
        {
            this.session = session;
            this.configuration = configuration;
            this.chatOutput = chatOutput;
            this.logService = logService;
        }

        public bool ApplyIfMatched(string sender, string message)
        {
            bool isMatch = IsMatch(message.ToLower());
            if (!isMatch) { return false; }
            bool isApplicable = IsApplicable(sender);
            if (!isApplicable) { return false; }
            Execute(sender, message);
            return true;
        }

        protected abstract bool IsMatch(string messageInLowercase);

        protected abstract bool IsApplicable(string sender);

        protected abstract void Execute(string sender, string message);

        protected bool IsMatchAsSeparateWordInPhrase(string word, string message)
        {
            var matches = new Regex($"[^\\w\\d\\s]*(\\w*)({word})(\\w*)[^\\w\\d\\s]*").Match(message);
            return matches.Success && matches.Groups[1].Value.IsNullOrWhitespace() && matches.Groups[3].Value.IsNullOrWhitespace();
        }

        protected bool IsMatchWithNoOtherWords(string word, string message)
        {
            return new Regex($"^[^\\w\\d]*{word}[^\\w\\d]*$").IsMatch(message);
        }

        protected bool IsForbiddenReChoice(PlayerPair pair)
        {
            return pair.Loser != null && pair.ChallengeType != ChallengeType.None && !configuration.AllowChangeDecision;
        }
    }
}
