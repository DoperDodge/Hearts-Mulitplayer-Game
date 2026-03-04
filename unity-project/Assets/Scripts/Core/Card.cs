using System;

namespace Hearts.Core
{
    /// <summary>
    /// Represents a single playing card. Immutable value type.
    /// </summary>
    public readonly struct Card : IEquatable<Card>, IComparable<Card>
    {
        public Suit Suit { get; }
        public Rank Rank { get; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        /// <summary>
        /// Numeric sort value: suit * 100 + rank. Sorts by suit first, then rank.
        /// </summary>
        public int SortValue => (int)Suit * 100 + (int)Rank;

        /// <summary>
        /// Penalty points this card is worth (Hearts = 1, Queen of Spades = 13, all others = 0).
        /// </summary>
        public int PenaltyPoints
        {
            get
            {
                if (Suit == Suit.Hearts) return 1;
                if (Suit == Suit.Spades && Rank == Rank.Queen) return 13;
                return 0;
            }
        }

        /// <summary>
        /// Whether this card is a penalty card (any Heart or Queen of Spades).
        /// </summary>
        public bool IsPenaltyCard => PenaltyPoints > 0;

        /// <summary>
        /// Whether this is the 2 of Clubs (must lead first trick).
        /// </summary>
        public bool IsTwoOfClubs => Suit == Suit.Clubs && Rank == Rank.Two;

        /// <summary>
        /// Whether this is the Queen of Spades.
        /// </summary>
        public bool IsQueenOfSpades => Suit == Suit.Spades && Rank == Rank.Queen;

        /// <summary>
        /// Whether this is the Jack of Diamonds (optional variant bonus card).
        /// </summary>
        public bool IsJackOfDiamonds => Suit == Suit.Diamonds && Rank == Rank.Jack;

        public int CompareTo(Card other) => SortValue.CompareTo(other.SortValue);

        public bool Equals(Card other) => Suit == other.Suit && Rank == other.Rank;

        public override bool Equals(object obj) => obj is Card other && Equals(other);

        public override int GetHashCode() => SortValue;

        public static bool operator ==(Card left, Card right) => left.Equals(right);

        public static bool operator !=(Card left, Card right) => !left.Equals(right);

        public override string ToString()
        {
            string rankStr = Rank switch
            {
                Rank.Two => "2",
                Rank.Three => "3",
                Rank.Four => "4",
                Rank.Five => "5",
                Rank.Six => "6",
                Rank.Seven => "7",
                Rank.Eight => "8",
                Rank.Nine => "9",
                Rank.Ten => "10",
                Rank.Jack => "J",
                Rank.Queen => "Q",
                Rank.King => "K",
                Rank.Ace => "A",
                _ => "?"
            };

            string suitStr = Suit switch
            {
                Suit.Clubs => "\u2663",
                Suit.Diamonds => "\u2666",
                Suit.Spades => "\u2660",
                Suit.Hearts => "\u2665",
                _ => "?"
            };

            return $"{rankStr}{suitStr}";
        }
    }
}
