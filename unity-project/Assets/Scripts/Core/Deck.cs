using System;
using System.Collections.Generic;

namespace Hearts.Core
{
    /// <summary>
    /// Represents a standard 52-card deck with shuffle and deal capabilities.
    /// </summary>
    public class Deck
    {
        private readonly List<Card> _cards = new List<Card>(52);
        private readonly Random _rng;

        /// <summary>
        /// Creates a new deck. Optionally provide a seed for deterministic shuffling (useful for testing and multiplayer sync).
        /// </summary>
        public Deck(int? seed = null)
        {
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
            Reset();
        }

        /// <summary>
        /// Number of cards remaining in the deck.
        /// </summary>
        public int CardsRemaining => _cards.Count;

        /// <summary>
        /// Resets the deck to a full 52-card sorted state.
        /// </summary>
        public void Reset()
        {
            _cards.Clear();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    _cards.Add(new Card(suit, rank));
                }
            }
        }

        /// <summary>
        /// Shuffles the deck using the Fisher-Yates algorithm.
        /// </summary>
        public void Shuffle()
        {
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                Card temp = _cards[i];
                _cards[i] = _cards[j];
                _cards[j] = temp;
            }
        }

        /// <summary>
        /// Deals the specified number of cards from the top of the deck.
        /// </summary>
        public List<Card> Deal(int count)
        {
            if (count > _cards.Count)
                throw new InvalidOperationException($"Cannot deal {count} cards, only {_cards.Count} remaining.");

            var dealt = _cards.GetRange(0, count);
            _cards.RemoveRange(0, count);
            return dealt;
        }

        /// <summary>
        /// Deals the entire deck evenly to the specified number of players (must divide evenly).
        /// Returns an array of hands, one per player.
        /// </summary>
        public List<Card>[] DealAll(int playerCount)
        {
            if (_cards.Count % playerCount != 0)
                throw new InvalidOperationException($"Cannot deal {_cards.Count} cards evenly to {playerCount} players.");

            int cardsPerPlayer = _cards.Count / playerCount;
            var hands = new List<Card>[playerCount];

            for (int i = 0; i < playerCount; i++)
            {
                hands[i] = Deal(cardsPerPlayer);
            }

            return hands;
        }
    }
}
