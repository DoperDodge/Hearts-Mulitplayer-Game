namespace Hearts.Core
{
    /// <summary>
    /// Configurable game settings/variants. Applied per game.
    /// </summary>
    public class GameSettings
    {
        /// <summary>
        /// Cumulative score that triggers game end. Default: 100.
        /// </summary>
        public int ScoreLimit { get; set; } = 100;

        /// <summary>
        /// What happens when a player Shoots the Moon.
        /// True = add 26 to all other players (default).
        /// False = subtract 26 from the shooter (min 0).
        /// </summary>
        public bool ShootTheMoonAddsToOthers { get; set; } = true;

        /// <summary>
        /// Whether the Jack of Diamonds variant is enabled (-10 points for taking it).
        /// </summary>
        public bool JackOfDiamondsEnabled { get; set; } = false;

        /// <summary>
        /// Whether card passing occurs. If false, every round is a Hold round.
        /// </summary>
        public bool PassingEnabled { get; set; } = true;

        /// <summary>
        /// Whether penalty cards are restricted on the first trick.
        /// </summary>
        public bool FirstTrickPenaltyRestriction { get; set; } = true;

        /// <summary>
        /// Returns default tournament-standard settings.
        /// </summary>
        public static GameSettings Default => new GameSettings();
    }
}
