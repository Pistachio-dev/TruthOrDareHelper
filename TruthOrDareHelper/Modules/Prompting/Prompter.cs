using Dalamud.Plugin;
using DalamudBasics.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TruthOrDareHelper.Modules.Prompting.DefaultPrompts;
using TruthOrDareHelper.Modules.Prompting.Interface;

namespace TruthOrDareHelper.Modules.Prompting
{
    public class Prompter : IPrompter
    {
        private const string PromptFilesFolderName = "Prompts";
        private const string SfwTruthFileName = "SFWTRUTHS.txt";
        private const string NsfwTruthFileName = "NSFWTRUTHS.txt";
        private const string SfwDareFileName = "SFWDARES.txt";
        private const string NsfwDareFileName = "NSFWDARES.txt";
        private readonly IPromptCollection[] promptCollections;
        private readonly ILogService logService;
        private readonly string folderRoute;
        private DateTime lastLoaded = DateTime.MinValue;

        public Prompter(IDalamudPluginInterface pluginInterface, ILogService logService)
        {
            this.folderRoute = pluginInterface.GetPluginConfigDirectory() + Path.DirectorySeparatorChar + PromptFilesFolderName + Path.DirectorySeparatorChar;
            this.logService = logService;
            promptCollections = [
                new SfwTruths(SfwTruthFileName),
                new NsfwTruths(NsfwTruthFileName),
                new SfwDares(SfwDareFileName),
                new NsfwDares(NsfwDareFileName),
            ];
        }

        private IPromptCollection SfwTruths => promptCollections[0];
        private IPromptCollection NsfwTruths => promptCollections[1];
        private IPromptCollection SfwDares => promptCollections[2];
        private IPromptCollection NsfwDares => promptCollections[3];

        public string GetStatsString()
        {
            var stats = new PromptsLoadedStats(lastLoaded,
                promptCollections[0].LoadedPromts.Length,
                promptCollections[1].LoadedPromts.Length,
                promptCollections[2].LoadedPromts.Length,
                promptCollections[3].LoadedPromts.Length);

            int truthAmount = stats.SfwTruthAmount + stats.NsfwTruthAmount;
            int dareAmount = stats.SfwDareAmount + stats.NsfwDareAmount;

            return $"{truthAmount} prompts for truth ({stats.SfwTruthAmount} SFW, {stats.NsfwTruthAmount} NSFW) " +
                $"and {dareAmount} prompts for dare ({stats.SfwDareAmount} SFW, {stats.NsfwDareAmount} NSFW) loaded. Last reload at {stats.LastLoad.ToShortTimeString()}";
        }

        public void OpenFolder()
        {
            Process.Start(new ProcessStartInfo(folderRoute) { UseShellExecute = true });
        }

        public void SeedIfNeeded()
        {
            if (!Directory.Exists(folderRoute))
            {
                Directory.CreateDirectory(folderRoute);
            }

            foreach (var promptCollection in promptCollections)
            {
                SeedFileIfMissing(promptCollection);
            }            
        }

        public void LoadPromptsToMemory()
        {
            foreach (var promptCollection in promptCollections)
            {
                List<string> prompts = new() ;
                using var sr = new StreamReader(folderRoute + promptCollection.FileName);
                while (!sr.EndOfStream)
                {
                    prompts.Add(sr.ReadLine());
                }

                promptCollection.LoadedPromts = prompts.ToArray();
            }
        }

        public string GetPrompt(bool useSfwTruths, bool useNsfwTruths, bool useSfwDares, bool useNsfwDares)
        {
            return GetPrompt([useSfwTruths, useNsfwTruths, useSfwDares, useNsfwDares]);
        }

        private string GetPrompt(bool[] flags)
        {
            if (flags.Length != 4) { throw new Exception($"Error on prompt retrieval: incorrect flag amount."); }
            int count = 0;
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i])
                {
                    count += promptCollections[i].LoadedPromts.Length;
                }
            }

            int index = new Random().Next(0, count);
            int cursor = 0;
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i])
                {
                    if (index < cursor + promptCollections[i].LoadedPromts.Length)
                    {                        
                        return promptCollections[i].Tag + promptCollections[i].LoadedPromts[index - cursor];
                    }

                    cursor += promptCollections[i].LoadedPromts.Length;
                }
            }

            return "No prompt available. Oh no.";
        }

        private void SeedFileIfMissing(IPromptCollection promptCollection)
        {
            string fullRoute = folderRoute + promptCollection.FileName;
            if (!File.Exists(fullRoute))
            {
                using var sw = new StreamWriter(fullRoute);
                for (int i = 0; i < promptCollection.DefaultPrompts.Length; i++)
                {
                    sw.WriteLine(promptCollection.DefaultPrompts[i]);
                }
            }
        }
    }
}
