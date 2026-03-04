using System;
using System.Collections.Generic;
using System.Linq;

namespace Hearts.Core
{
    /// <summary>
    /// Represents a player's hand of cards. Provides query and manipulation methods.
    /// </summary>
    public class Hand
    {
        private readonly List<Card> _cards = new List<Card>();

        /// <summary>
        /// Read-only view of all cards in the hand.
        /// </summary>
        public IReadOnlyList<Card> Cards => _cards.AsReadOnly();

        /// <summary>
        /// Number of cards currently in hand.
        /// </summary>
        public int Count => _cards.Count;

        /// <summary>
        /// Whether the hand is empty.
        /// </summary>
        public bool IsEmpty => _cards.Count == 0;

        /// <summary>
        /// Adds multiple cards to the hand and sorts.
        /// </summary>
        public void AddCards(IEnumerable<Card> cards)
        {
            _cards.AddRange(cards);
            Sort();
        }

        /// <summary>
        /// Adds a single card to the hand and sorts.
        /// </summary>
        public void AddCard(Card card)
        {
            _cards.Add(card);
            Sort();
        }

        /// <summary>
        /// Removes a card from the hand. Throws if card is not present.
        /// </summary>
        public void RemoveCard(Card card)
        {
            if (!_cards.Remove(card))
                throw new InvalidOperationException($"Card {card} is not in this hand.");
        }

        /// <summary>
        /// Removes multiple cards from the hand.
        /// </summary>
        public void RemoveCards(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                RemoveCard(card);
            }
        }

        /// <summary>
        /// Whether the hand contains the specified card.
        /// </summary>
        public bool Contains(Card card) => _cards.Contains(card);

        /// <summary>
        /// Whether the hand contains the 2 of Clubs.
        /// </summary>
        public bool HasTwoOfClubs => _cards.Any(c => c.IsTwoOfClubs);

        /// <summary>
        /// Returns all cards of the specified suit.
        /// </summary>
        public List<Card> GetCardsOfSuit(Suit suit) => _cards.Where(c => c.Suit == suit).ToList();

        /// <summary>
        /// Whether the hand has any cards of the specified suit.
        /// </summary>
        public bool HasSuit(Suit suit) => _cards.Any(c => c.Suit == suit);

        /// <summary>
        /// Whether the hand contains only penalty cards (Hearts and Queen of Spades).
        /// </summary>
        public bool HasOnlyPenaltyCards => _cards.All(c => c.IsPenaltyCard);

        /// <summary>
        /// Whether the hand contains only Hearts.
        /// </summary>
        public bool HasOnlyHearts => _cards.All(c => c.Suit == Suit.Hearts);

        /// <summary>
        /// Returns all penalty cards in the hand.
        /// </summary>
        public List<Card> GetPenaltyCards() => _cards.Where(c => c.IsPenaltyCard).ToList();

        /// <summary>
        /// Returns all non-penalty cards in the hand.
        /// </summary>
        public List<Card> GetNonPenaltyCards() => _cards.Where(c => !c.IsPenaltyCard).ToList();

        /// <summary>
        /// Returns all cards that are legal to play given the current game context.
        /// </summary>
        public List<Card> GetLegalPlays(Suit? ledSuit, bool isFirstTrick, bool heartsBroken, bool isLeading)
        {
            // If leading a trick
            if (isLeading)
            {
                return GetLegalLeads(heartsBroken);
            }

            // Must follow suit if possible
            if (ledSuit.HasValue)
            {
                var suitCards = GetCardsOfSuit(ledSuit.Value);
                if (suitCards.Count > 0)
                    return suitCards;
            }

            // Cannot follow suit — can play anything, with first-trick restrictions
            if (isFirstTrick)
            {
                // No penalty cards on first trick, unless hand is all penalty cards
                var nonPenalty = GetNonPenaltyCards();
                if (nonPenalty.Count > 0)
                    return nonPenalty;
            }

            // Can play any card
            return new List<Card>(_cards);
        }

        /// <summary>
        /// Returns all cards that are legal to lead with.
        /// </summary>
        private List<Card> GetLegalLeads(bool heartsBroken)
        {
            if (heartsBroken)
                return new List<Card>(_cards);

            // Hearts not broken — cannot lead Hearts unless hand is all Hearts
            var nonHearts = _cards.Where(c => c.Suit != Suit.Hearts).ToList();
            if (nonHearts.Count > 0)
                return nonHearts;

            // Hand is all Hearts — must lead a Heart (this also breaks Hearts)
            return new List<Card>(_cards);
        }

        /// <summary>
        /// Sorts the hand by suit (Clubs, Diamonds, Spades, Hearts), then by rank.
        /// </summary>
        public void Sort()
        {
            _cards.Sort((a, b) => a.SortValue.CompareTo(b.SortValue));
        }

        /// <summary>
        /// Clears all cards from the hand.
        /// </summary>
        public void Clear()
        {
            _cards.Clear();
        }

        public override string ToString()
        {
            return string.Join(" ", _cards.Select(c => c.ToString()));
        }
    }
}
