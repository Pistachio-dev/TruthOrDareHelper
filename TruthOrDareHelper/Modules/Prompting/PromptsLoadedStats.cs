using System;

namespace TruthOrDareHelper.Modules.Prompting
{
    internal class PromptsLoadedStats
    {
        public PromptsLoadedStats(DateTime lastLoad, int sfwTruthAmount, int nsfwTruthAmount, int sfwDareAmount, int nsfwDareAmount)
        {
            LastLoad = lastLoad;
            SfwTruthAmount = sfwTruthAmount;
            NsfwTruthAmount = nsfwTruthAmount;
            SfwDareAmount = sfwDareAmount;
            NsfwDareAmount = nsfwDareAmount;
        }

        public DateTime LastLoad { get; set; }
        public int SfwTruthAmount { get; set; }
        public int NsfwTruthAmount { get; set; }
        public int SfwDareAmount { get; set; }
        public int NsfwDareAmount { get; set; }
    }
}
