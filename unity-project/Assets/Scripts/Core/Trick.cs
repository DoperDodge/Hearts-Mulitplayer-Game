using System;
using System.Collections.Generic;
using System.Linq;

namespace Hearts.Core
{
    /// <summary>
    /// Represents a single trick: 4 cards played in order, one per player.
    /// </summary>
    public class Trick
    {
        /// <summary>
        /// Cards played in this trick, in play order. Each entry is (playerSeat, card).
        /// </summary>
        private readonly List<(int PlayerSeat, Card Card)> _plays = new List<(int, Card)>(4);

        /// <summary>
        /// The seat index of the player who leads this trick.
        /// </summary>
        public int LeadPlayerSeat { get; }

        /// <summary>
        /// The suit of the lead card. Null if no card has been played yet.
        /// </summary>
        public Suit? LedSuit => _plays.Count > 0 ? _plays[0].Card.Suit : (Suit?)null;

        /// <summary>
        /// Read-only view of all plays in this trick.
        /// </summary>
        public IReadOnlyList<(int PlayerSeat, Card Card)> Plays => _plays.AsReadOnly();

        /// <summary>
        /// Number of cards played in this trick so far.
        /// </summary>
        public int CardCount => _plays.Count;

        /// <summary>
        /// Whether all 4 cards have been played.
        /// </summary>
        public bool IsComplete => _plays.Count == 4;

        /// <summary>
        /// Total penalty points in this trick.
        /// </summary>
        public int PenaltyPoints => _plays.Sum(p => p.Card.PenaltyPoints);

        /// <summary>
        /// Whether this trick contains any Hearts.
        /// </summary>
        public bool ContainsHearts => _plays.Any(p => p.Card.Suit == Suit.Hearts);

        /// <summary>
        /// Whether this trick contains the Queen of Spades.
        /// </summary>
        public bool ContainsQueenOfSpades => _plays.Any(p => p.Card.IsQueenOfSpades);

        public Trick(int leadPlayerSeat)
        {
            LeadPlayerSeat = leadPlayerSeat;
        }

        /// <summary>
        /// Plays a card into this trick.
        /// </summary>
        public void PlayCard(int playerSeat, Card card)
        {
            if (IsComplete)
                throw new InvalidOperationException("Trick is already complete (4 cards played).");

            if (_plays.Any(p => p.PlayerSeat == playerSeat))
                throw new InvalidOperationException($"Player {playerSeat} has already played in this trick.");

            _plays.Add((playerSeat, card));
        }

        /// <summary>
        /// Determines the winner of this trick. Must be called after all 4 cards are played.
        /// Winner is the player who played the highest card of the led suit.
        /// </summary>
        public int GetWinnerSeat()
        {
            if (!IsComplete)
                throw new InvalidOperationException("Cannot determine winner — trick is not complete.");

            Suit led = _plays[0].Card.Suit;

            int winnerSeat = _plays[0].PlayerSeat;
            Rank highestRank = _plays[0].Card.Rank;

            for (int i = 1; i < _plays.Count; i++)
            {
                var play = _plays[i];
                // Only cards of the led suit can win
                if (play.Card.Suit == led && play.Card.Rank > highestRank)
                {
                    highestRank = play.Card.Rank;
                    winnerSeat = play.PlayerSeat;
                }
            }

            return winnerSeat;
        }

        /// <summary>
        /// Returns the seat index of the next player to play in this trick.
        /// </summary>
        public int GetNextPlayerSeat()
        {
            if (IsComplete)
                throw new InvalidOperationException("Trick is complete — no next player.");

            if (_plays.Count == 0)
                return LeadPlayerSeat;

            int lastSeat = _plays[_plays.Count - 1].PlayerSeat;
            return (lastSeat + 1) % 4;
        }

        /// <summary>
        /// Returns all cards played in this trick.
        /// </summary>
        public List<Card> GetAllCards() => _plays.Select(p => p.Card).ToList();

        public override string ToString()
        {
            return string.Join(", ", _plays.Select(p => $"P{p.PlayerSeat}:{p.Card}"));
        }
    }
}
