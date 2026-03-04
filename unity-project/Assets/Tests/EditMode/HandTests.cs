using NUnit.Framework;
using Hearts.Core;
using System.Collections.Generic;

namespace Hearts.Tests
{
    [TestFixture]
    public class HandTests
    {
        private Hand _hand;

        [SetUp]
        public void Setup()
        {
            _hand = new Hand();
        }

        [Test]
        public void Hand_Empty_HasZeroCards()
        {
            Assert.AreEqual(0, _hand.Count);
            Assert.IsTrue(_hand.IsEmpty);
        }

        [Test]
        public void Hand_AddCards_IncreasesCount()
        {
            _hand.AddCards(new List<Card>
            {
                new Card(Suit.Hearts, Rank.Ace),
                new Card(Suit.Clubs, Rank.Two)
            });

            Assert.AreEqual(2, _hand.Count);
            Assert.IsFalse(_hand.IsEmpty);
        }

        [Test]
        public void Hand_AddCards_SortsAutomatically()
        {
            _hand.AddCards(new List<Card>
            {
                new Card(Suit.Hearts, Rank.Ace),
                new Card(Suit.Clubs, Rank.Two),
                new Card(Suit.Clubs, Rank.King)
            });

            // Clubs before Hearts, and within clubs: Two before King
            Assert.AreEqual(new Card(Suit.Clubs, Rank.Two), _hand.Cards[0]);
            Assert.AreEqual(new Card(Suit.Clubs, Rank.King), _hand.Cards[1]);
            Assert.AreEqual(new Card(Suit.Hearts, Rank.Ace), _hand.Cards[2]);
        }

        [Test]
        public void Hand_RemoveCard_DecreasesCount()
        {
            var card = new Card(Suit.Spades, Rank.Queen);
            _hand.AddCard(card);
            _hand.RemoveCard(card);

            Assert.AreEqual(0, _hand.Count);
        }

        [Test]
        public void Hand_RemoveCard_NotInHand_Throws()
        {
            var card = new Card(Suit.Spades, Rank.Queen);
            Assert.Throws<System.InvalidOperationException>(() => _hand.RemoveCard(card));
        }

        [Test]
        public void Hand_Contains_Works()
        {
            var card = new Card(Suit.Diamonds, Rank.Jack);
            _hand.AddCard(card);

            Assert.IsTrue(_hand.Contains(card));
            Assert.IsFalse(_hand.Contains(new Card(Suit.Diamonds, Rank.Queen)));
        }

        [Test]
        public void Hand_HasTwoOfClubs_WhenPresent()
        {
            _hand.AddCard(new Card(Suit.Clubs, Rank.Two));
            Assert.IsTrue(_hand.HasTwoOfClubs);
        }

        [Test]
        public void Hand_HasTwoOfClubs_WhenAbsent()
        {
            _hand.AddCard(new Card(Suit.Clubs, Rank.Three));
            Assert.IsFalse(_hand.HasTwoOfClubs);
        }

        [Test]
        public void Hand_GetCardsOfSuit_ReturnsCorrectCards()
        {
            _hand.AddCards(new List<Card>
            {
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Hearts, Rank.King),
                new Card(Suit.Clubs, Rank.Ace),
                new Card(Suit.Spades, Rank.Five)
            });

            var hearts = _hand.GetCardsOfSuit(Suit.Hearts);
            Assert.AreEqual(2, hearts.Count);
            Assert.IsTrue(hearts[0].Suit == Suit.Hearts);
            Assert.IsTrue(hearts[1].Suit == Suit.Hearts);
        }

        [Test]
        public void Hand_HasSuit_ReturnsTrueWhenPresent()
        {
            _hand.AddCard(new Card(Suit.Diamonds, Rank.Three));
            Assert.IsTrue(_hand.HasSuit(Suit.Diamonds));
            Assert.IsFalse(_hand.HasSuit(Suit.Hearts));
        }

        [Test]
        public void Hand_HasOnlyPenaltyCards_AllHearts()
        {
            _hand.AddCards(new List<Card>
            {
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Hearts, Rank.King)
            });
            Assert.IsTrue(_hand.HasOnlyPenaltyCards);
            Assert.IsTrue(_hand.HasOnlyHearts);
        }

        [Test]
        public void Hand_HasOnlyPenaltyCards_HeartsAndQueenOfSpades()
        {
            _hand.AddCards(new List<Card>
            {
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Spades, Rank.Queen)
            });
            Assert.IsTrue(_hand.HasOnlyPenaltyCards);
            Assert.IsFalse(_hand.HasOnlyHearts);
        }

        [Test]
        public void Hand_HasOnlyPenaltyCards_MixedHand()
        {
            _hand.AddCards(new List<Card>
            {
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Clubs, Rank.Ace)
            });
            Assert.IsFalse(_hand.HasOnlyPenaltyCards);
        }

        // --- Legal Play Tests ---

        [Test]
        public void Hand_LegalPlays_MustFollowSuit()
        {
            _hand.AddCards(new List<Card>
            {
                new Card(Suit.Clubs, Rank.Three),
                new Card(Suit.Clubs, Rank.King),
                new Card(Suit.Hearts, Rank.Ace)
            });

            var legal = _hand.GetLegalPlays(
                ledSuit: Suit.Clubs, isFirstTrick: false, heartsBroken: false, isLeading: false);

            Assert.AreEqual(2, legal.Count);
            Assert.IsTrue(legal.TrueForAll(c => c.Suit == Suit.Clubs));
        }

        [Test]
        public void Hand_LegalPlays_CanPlayAnythingWhenVoid()
        {
            _hand.AddCards(new List<Card>
            {
                new Card(Suit.Hearts, Rank.Ace),
                new Card(Suit.Spades, Rank.Queen),
                new Card(Suit.Diamonds, Rank.Five)
            });

            var legal = _hand.GetLegalPlays(
                ledSuit: Suit.Clubs, isFirstTrick: false, heartsBroken: false, isLeading: false);

            Assert.AreEqual(3, legal.Count); // Can play anything
        }

        [Test]
        public void Hand_LegalPlays_FirstTrick_NoPenaltyCardsWhenVoid()
        {
            _hand.AddCards(new List<Card>
            {
                new Card(Suit.Hearts, Rank.Ace),
                new Card(Suit.Spades, Rank.Queen),
                new Card(Suit.Diamonds, Rank.Five)
            });

            var legal = _hand.GetLegalPlays(
                ledSuit: Suit.Clubs, isFirstTrick: true, heartsBroken: false, isLeading: false);

            Assert.AreEqual(1, legal.Count);
            Assert.AreEqual(new Card(Suit.Diamonds, Rank.Five), legal[0]);
        }

        [Test]
        public void Hand_LegalPlays_FirstTrick_AllPenaltyCards_CanPlayAny()
        {
            _hand.AddCards(new List<Card>
            {
                new Card(Suit.Hearts, Rank.Ace),
                new Card(Suit.Spades, Rank.Queen),
                new Card(Suit.Hearts, Rank.Two)
            });

            var legal = _hand.GetLegalPlays(
                ledSuit: Suit.Clubs, isFirstTrick: true, heartsBroken: false, isLeading: false);

            // All penalty cards — exception allows playing any
            Assert.AreEqual(3, legal.Count);
        }

        [Test]
        public void Hand_LegalPlays_CannotLeadHeartsBeforeBroken()
        {
            _hand.AddCards(new List<Card>
            {
                new Card(Suit.Clubs, Rank.Three),
                new Card(Suit.Hearts, Rank.Ace),
                new Card(Suit.Hearts, Rank.King)
            });

            var legal = _hand.GetLegalPlays(
                ledSuit: null, isFirstTrick: false, heartsBroken: false, isLeading: true);

            Assert.AreEqual(1, legal.Count);
            Assert.AreEqual(Suit.Clubs, legal[0].Suit);
        }

        [Test]
        public void Hand_LegalPlays_CanLeadHeartsAfterBroken()
        {
            _hand.AddCards(new List<Card>
            {
                new Card(Suit.Clubs, Rank.Three),
                new Card(Suit.Hearts, Rank.Ace)
            });

            var legal = _hand.GetLegalPlays(
                ledSuit: null, isFirstTrick: false, heartsBroken: true, isLeading: true);

            Assert.AreEqual(2, legal.Count);
        }

        [Test]
        public void Hand_LegalPlays_OnlyHearts_CanLeadBeforeBroken()
        {
            _hand.AddCards(new List<Card>
            {
                new Card(Suit.Hearts, Rank.Ace),
                new Card(Suit.Hearts, Rank.King)
            });

            var legal = _hand.GetLegalPlays(
                ledSuit: null, isFirstTrick: false, heartsBroken: false, isLeading: true);

            Assert.AreEqual(2, legal.Count); // Exception: only hearts in hand
        }

        [Test]
        public void Hand_Clear_RemovesAllCards()
        {
            _hand.AddCard(new Card(Suit.Hearts, Rank.Ace));
            _hand.Clear();
            Assert.AreEqual(0, _hand.Count);
            Assert.IsTrue(_hand.IsEmpty);
        }
    }
}
