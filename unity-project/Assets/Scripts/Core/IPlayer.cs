using System.Collections.Generic;

namespace Hearts.Core
{
    /// <summary>
    /// Interface for all player types (Human, AI, Network).
    /// Methods are called by the game manager to request player actions.
    /// Implementations may be async (e.g., waiting for human input or network response).
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// The player's seat index (0–3, clockwise: South, West, North, East).
        /// </summary>
        int SeatIndex { get; }

        /// <summary>
        /// Display name of the player.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Whether this player is controlled by AI.
        /// </summary>
        bool IsAI { get; }

        /// <summary>
        /// Whether this player is connected (always true for local/AI, may be false for network players).
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// The player's current hand.
        /// </summary>
        Hand Hand { get; }

        /// <summary>
        /// Called when cards are dealt to this player.
        /// </summary>
        void OnCardsDealt(List<Card> cards);

        /// <summary>
        /// Called to request the player to select 3 cards to pass.
        /// The callback should be invoked with the selected cards.
        /// </summary>
        void RequestPassCards(PassDirection direction, System.Action<List<Card>> onCardsSelected);

        /// <summary>
        /// Called when the player receives passed cards.
        /// </summary>
        void OnCardsReceived(List<Card> cards);

        /// <summary>
        /// Called to request the player to play a card.
        /// legalPlays contains all cards the player is allowed to play.
        /// The callback should be invoked with the selected card.
        /// </summary>
        void RequestPlayCard(List<Card> legalPlays, Trick currentTrick, System.Action<Card> onCardPlayed);

        /// <summary>
        /// Called when a trick is completed, informing the player of the result.
        /// </summary>
        void OnTrickComplete(Trick completedTrick, int winnerSeat);

        /// <summary>
        /// Called when a round is completed with the round scores.
        /// </summary>
        void OnRoundComplete(int[] roundScores, int[] cumulativeScores);

        /// <summary>
        /// Called when the game ends.
        /// </summary>
        void OnGameOver(int winnerSeat, int[] finalScores);
    }
}
