namespace TruthOrDareHelper.Settings
{
    public enum RollingType
    {
        PluginRng, // Plain RNG done in the plugin
        DiceRoll, // Read the /dice or /random done by the player
        PluginWeightedRng // Rolls are weighted so players that have not participated get a higher chance
    }
}
