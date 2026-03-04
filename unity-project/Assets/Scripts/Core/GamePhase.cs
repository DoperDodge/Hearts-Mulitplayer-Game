namespace Hearts.Core
{
    /// <summary>
    /// Represents the current phase of the game state machine.
    /// </summary>
    public enum GamePhase
    {
        /// <summary>Waiting for players to join the lobby.</summary>
        WaitingForPlayers,

        /// <summary>Cards are being dealt to all players.</summary>
        Dealing,

        /// <summary>Players are selecting cards to pass.</summary>
        PassingCards,

        /// <summary>A trick is in progress — players are playing cards.</summary>
        PlayingTrick,

        /// <summary>A trick has been completed and is being scored/collected.</summary>
        TrickComplete,

        /// <summary>All 13 tricks are done — round is being scored.</summary>
        ScoringRound,

        /// <summary>The game has ended — final scores displayed.</summary>
        GameOver
    }
}
