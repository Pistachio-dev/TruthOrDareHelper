using DalamudBasics.Chat.Output;
using DalamudBasics.Logging;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TruthOrDareHelper.Settings;

namespace TruthOrDareHelper.Modules.Chat.Commands
{
    internal abstract class ChatCommandBase
    {
        protected readonly ITruthOrDareSession session;
        protected readonly Configuration configuration;
        protected readonly IChatOutput chatOutput;
        protected readonly ILogService logService;

        protected ChatCommandBase(ITruthOrDareSession session, Configuration configuration, IChatOutput chatOutput, ILogService logService)
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
            if (!isApplicable) { return false;  }
            Execute(sender);
            return true;
        }

        protected abstract bool IsMatch(string messageInLowercase);

        protected abstract bool IsApplicable(string sender);

        protected abstract void Execute(string sender);

        protected bool IsMatchAsSeparateWordInPhrase(string word, string message)
        {
            return new Regex($"[^\\w\\d\\s]*(\\w*)({word})(\\w*)[^\\w\\d\\s]*").Match(message).Success;
        }
    }
}
