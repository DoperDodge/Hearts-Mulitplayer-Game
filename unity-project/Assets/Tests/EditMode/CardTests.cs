using NUnit.Framework;
using Hearts.Core;

namespace Hearts.Tests
{
    [TestFixture]
    public class CardTests
    {
        [Test]
        public void Card_TwoOfClubs_IsTwoOfClubs()
        {
            var card = new Card(Suit.Clubs, Rank.Two);
            Assert.IsTrue(card.IsTwoOfClubs);
        }

        [Test]
        public void Card_AceOfSpades_IsNotTwoOfClubs()
        {
            var card = new Card(Suit.Spades, Rank.Ace);
            Assert.IsFalse(card.IsTwoOfClubs);
        }

        [Test]
        public void Card_QueenOfSpades_IsQueenOfSpades()
        {
            var card = new Card(Suit.Spades, Rank.Queen);
            Assert.IsTrue(card.IsQueenOfSpades);
            Assert.IsTrue(card.IsPenaltyCard);
            Assert.AreEqual(13, card.PenaltyPoints);
        }

        [Test]
        public void Card_Heart_IsPenaltyCard()
        {
            var card = new Card(Suit.Hearts, Rank.Seven);
            Assert.IsTrue(card.IsPenaltyCard);
            Assert.AreEqual(1, card.PenaltyPoints);
        }

        [Test]
        public void Card_NonPenalty_HasZeroPoints()
        {
            var card = new Card(Suit.Diamonds, Rank.King);
            Assert.IsFalse(card.IsPenaltyCard);
            Assert.AreEqual(0, card.PenaltyPoints);
        }

        [Test]
        public void Card_KingOfSpades_IsNotPenaltyCard()
        {
            var card = new Card(Suit.Spades, Rank.King);
            Assert.IsFalse(card.IsPenaltyCard);
            Assert.AreEqual(0, card.PenaltyPoints);
        }

        [Test]
        public void Card_JackOfDiamonds_IsJackOfDiamonds()
        {
            var card = new Card(Suit.Diamonds, Rank.Jack);
            Assert.IsTrue(card.IsJackOfDiamonds);
        }

        [Test]
        public void Card_Equality_SameSuitAndRank()
        {
            var a = new Card(Suit.Hearts, Rank.Ace);
            var b = new Card(Suit.Hearts, Rank.Ace);
            Assert.AreEqual(a, b);
            Assert.IsTrue(a == b);
        }

        [Test]
        public void Card_Equality_DifferentCards()
        {
            var a = new Card(Suit.Hearts, Rank.Ace);
            var b = new Card(Suit.Hearts, Rank.King);
            Assert.AreNotEqual(a, b);
            Assert.IsTrue(a != b);
        }

        [Test]
        public void Card_SortValue_SuitThenRank()
        {
            var twoClubs = new Card(Suit.Clubs, Rank.Two);
            var aceClubs = new Card(Suit.Clubs, Rank.Ace);
            var twoHearts = new Card(Suit.Hearts, Rank.Two);

            Assert.Less(twoClubs.SortValue, aceClubs.SortValue);
            Assert.Less(aceClubs.SortValue, twoHearts.SortValue);
        }

        [Test]
        public void Card_CompareTo_OrdersCorrectly()
        {
            var low = new Card(Suit.Clubs, Rank.Three);
            var high = new Card(Suit.Clubs, Rank.King);

            Assert.Less(low.CompareTo(high), 0);
            Assert.Greater(high.CompareTo(low), 0);
            Assert.AreEqual(0, low.CompareTo(low));
        }

        [Test]
        public void Card_ToString_FormatsCorrectly()
        {
            Assert.AreEqual("2\u2663", new Card(Suit.Clubs, Rank.Two).ToString());
            Assert.AreEqual("Q\u2660", new Card(Suit.Spades, Rank.Queen).ToString());
            Assert.AreEqual("A\u2665", new Card(Suit.Hearts, Rank.Ace).ToString());
            Assert.AreEqual("10\u2666", new Card(Suit.Diamonds, Rank.Ten).ToString());
        }

        [Test]
        public void Card_AllHearts_TotalPenaltyIs13()
        {
            int total = 0;
            foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
            {
                total += new Card(Suit.Hearts, rank).PenaltyPoints;
            }
            Assert.AreEqual(13, total);
        }

        [Test]
        public void Card_AllPenalties_TotalIs26()
        {
            int total = 0;
            foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
            {
                total += new Card(Suit.Hearts, rank).PenaltyPoints;
            }
            total += new Card(Suit.Spades, Rank.Queen).PenaltyPoints;
            Assert.AreEqual(26, total);
        }
    }
}
