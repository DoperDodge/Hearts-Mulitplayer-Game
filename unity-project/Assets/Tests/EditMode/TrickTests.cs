using NUnit.Framework;
using Hearts.Core;

namespace Hearts.Tests
{
    [TestFixture]
    public class TrickTests
    {
        [Test]
        public void Trick_New_IsEmpty()
        {
            var trick = new Trick(0);
            Assert.AreEqual(0, trick.CardCount);
            Assert.IsFalse(trick.IsComplete);
            Assert.IsNull(trick.LedSuit);
        }

        [Test]
        public void Trick_PlayCard_AddsToTrick()
        {
            var trick = new Trick(0);
            trick.PlayCard(0, new Card(Suit.Clubs, Rank.Two));

            Assert.AreEqual(1, trick.CardCount);
            Assert.AreEqual(Suit.Clubs, trick.LedSuit);
        }

        [Test]
        public void Trick_FourCards_IsComplete()
        {
            var trick = new Trick(0);
            trick.PlayCard(0, new Card(Suit.Clubs, Rank.Two));
            trick.PlayCard(1, new Card(Suit.Clubs, Rank.Five));
            trick.PlayCard(2, new Card(Suit.Clubs, Rank.King));
            trick.PlayCard(3, new Card(Suit.Clubs, Rank.Ace));

            Assert.IsTrue(trick.IsComplete);
            Assert.AreEqual(4, trick.CardCount);
        }

        [Test]
        public void Trick_Winner_HighestOfLedSuit()
        {
            var trick = new Trick(0);
            trick.PlayCard(0, new Card(Suit.Clubs, Rank.Two));
            trick.PlayCard(1, new Card(Suit.Clubs, Rank.King));
            trick.PlayCard(2, new Card(Suit.Clubs, Rank.Five));
            trick.PlayCard(3, new Card(Suit.Clubs, Rank.Ace));

            Assert.AreEqual(3, trick.GetWinnerSeat()); // Player 3 has Ace
        }

        [Test]
        public void Trick_Winner_OffSuitCardsCantWin()
        {
            var trick = new Trick(0);
            trick.PlayCard(0, new Card(Suit.Clubs, Rank.Two));
            trick.PlayCard(1, new Card(Suit.Hearts, Rank.Ace));  // Off-suit, can't win
            trick.PlayCard(2, new Card(Suit.Clubs, Rank.Five));
            trick.PlayCard(3, new Card(Suit.Spades, Rank.Ace));  // Off-suit, can't win

            Assert.AreEqual(2, trick.GetWinnerSeat()); // Player 2 has highest Club
        }

        [Test]
        public void Trick_PenaltyPoints_NoHearts()
        {
            var trick = new Trick(0);
            trick.PlayCard(0, new Card(Suit.Clubs, Rank.Two));
            trick.PlayCard(1, new Card(Suit.Clubs, Rank.Five));
            trick.PlayCard(2, new Card(Suit.Clubs, Rank.King));
            trick.PlayCard(3, new Card(Suit.Clubs, Rank.Ace));

            Assert.AreEqual(0, trick.PenaltyPoints);
            Assert.IsFalse(trick.ContainsHearts);
            Assert.IsFalse(trick.ContainsQueenOfSpades);
        }

        [Test]
        public void Trick_PenaltyPoints_WithHearts()
        {
            var trick = new Trick(0);
            trick.PlayCard(0, new Card(Suit.Clubs, Rank.Two));
            trick.PlayCard(1, new Card(Suit.Hearts, Rank.Five));
            trick.PlayCard(2, new Card(Suit.Hearts, Rank.King));
            trick.PlayCard(3, new Card(Suit.Clubs, Rank.Ace));

            Assert.AreEqual(2, trick.PenaltyPoints); // 2 hearts = 2 points
            Assert.IsTrue(trick.ContainsHearts);
        }

        [Test]
        public void Trick_PenaltyPoints_QueenOfSpades()
        {
            var trick = new Trick(0);
            trick.PlayCard(0, new Card(Suit.Clubs, Rank.Two));
            trick.PlayCard(1, new Card(Suit.Spades, Rank.Queen));
            trick.PlayCard(2, new Card(Suit.Clubs, Rank.King));
            trick.PlayCard(3, new Card(Suit.Clubs, Rank.Ace));

            Assert.AreEqual(13, trick.PenaltyPoints);
            Assert.IsTrue(trick.ContainsQueenOfSpades);
        }

        [Test]
        public void Trick_PenaltyPoints_QueenAndHearts()
        {
            var trick = new Trick(0);
            trick.PlayCard(0, new Card(Suit.Clubs, Rank.Two));
            trick.PlayCard(1, new Card(Suit.Spades, Rank.Queen));
            trick.PlayCard(2, new Card(Suit.Hearts, Rank.King));
            trick.PlayCard(3, new Card(Suit.Hearts, Rank.Ace));

            Assert.AreEqual(15, trick.PenaltyPoints); // 13 + 1 + 1
        }

        [Test]
        public void Trick_GetNextPlayerSeat_Clockwise()
        {
            var trick = new Trick(2); // Player 2 leads
            Assert.AreEqual(2, trick.GetNextPlayerSeat());

            trick.PlayCard(2, new Card(Suit.Clubs, Rank.Two));
            Assert.AreEqual(3, trick.GetNextPlayerSeat());

            trick.PlayCard(3, new Card(Suit.Clubs, Rank.Five));
            Assert.AreEqual(0, trick.GetNextPlayerSeat()); // Wraps around

            trick.PlayCard(0, new Card(Suit.Clubs, Rank.King));
            Assert.AreEqual(1, trick.GetNextPlayerSeat());
        }

        [Test]
        public void Trick_PlayCard_SamePlayerTwice_Throws()
        {
            var trick = new Trick(0);
            trick.PlayCard(0, new Card(Suit.Clubs, Rank.Two));
            Assert.Throws<System.InvalidOperationException>(
                () => trick.PlayCard(0, new Card(Suit.Clubs, Rank.Three)));
        }

        [Test]
        public void Trick_PlayCard_FifthCard_Throws()
        {
            var trick = new Trick(0);
            trick.PlayCard(0, new Card(Suit.Clubs, Rank.Two));
            trick.PlayCard(1, new Card(Suit.Clubs, Rank.Three));
            trick.PlayCard(2, new Card(Suit.Clubs, Rank.Four));
            trick.PlayCard(3, new Card(Suit.Clubs, Rank.Five));

            Assert.Throws<System.InvalidOperationException>(
                () => trick.PlayCard(0, new Card(Suit.Clubs, Rank.Six)));
        }

        [Test]
        public void Trick_GetWinner_BeforeComplete_Throws()
        {
            var trick = new Trick(0);
            trick.PlayCard(0, new Card(Suit.Clubs, Rank.Two));
            Assert.Throws<System.InvalidOperationException>(() => trick.GetWinnerSeat());
        }
    }
}
