using NUnit.Framework;
using Hearts.Core;

namespace Hearts.Tests
{
    [TestFixture]
    public class DeckTests
    {
        [Test]
        public void Deck_New_Has52Cards()
        {
            var deck = new Deck();
            Assert.AreEqual(52, deck.CardsRemaining);
        }

        [Test]
        public void Deck_Deal_ReducesCount()
        {
            var deck = new Deck();
            var dealt = deck.Deal(5);
            Assert.AreEqual(5, dealt.Count);
            Assert.AreEqual(47, deck.CardsRemaining);
        }

        [Test]
        public void Deck_DealAll_FourPlayers_13Each()
        {
            var deck = new Deck();
            var hands = deck.DealAll(4);

            Assert.AreEqual(4, hands.Length);
            foreach (var hand in hands)
            {
                Assert.AreEqual(13, hand.Count);
            }
            Assert.AreEqual(0, deck.CardsRemaining);
        }

        [Test]
        public void Deck_DealAll_NoDuplicateCards()
        {
            var deck = new Deck();
            deck.Shuffle();
            var hands = deck.DealAll(4);

            var allCards = new System.Collections.Generic.HashSet<Card>();
            foreach (var hand in hands)
            {
                foreach (var card in hand)
                {
                    Assert.IsTrue(allCards.Add(card), $"Duplicate card found: {card}");
                }
            }
            Assert.AreEqual(52, allCards.Count);
        }

        [Test]
        public void Deck_Shuffle_SeededProducesSameResult()
        {
            var deck1 = new Deck(seed: 42);
            deck1.Shuffle();
            var hand1 = deck1.Deal(13);

            var deck2 = new Deck(seed: 42);
            deck2.Shuffle();
            var hand2 = deck2.Deal(13);

            for (int i = 0; i < 13; i++)
            {
                Assert.AreEqual(hand1[i], hand2[i], $"Card {i} differs between seeded decks.");
            }
        }

        [Test]
        public void Deck_Shuffle_DifferentSeedsProduceDifferentResults()
        {
            var deck1 = new Deck(seed: 42);
            deck1.Shuffle();
            var hand1 = deck1.Deal(13);

            var deck2 = new Deck(seed: 99);
            deck2.Shuffle();
            var hand2 = deck2.Deal(13);

            bool anyDifferent = false;
            for (int i = 0; i < 13; i++)
            {
                if (hand1[i] != hand2[i])
                {
                    anyDifferent = true;
                    break;
                }
            }
            Assert.IsTrue(anyDifferent, "Different seeds should produce different shuffles.");
        }

        [Test]
        public void Deck_Reset_RestoresFullDeck()
        {
            var deck = new Deck();
            deck.Deal(20);
            Assert.AreEqual(32, deck.CardsRemaining);

            deck.Reset();
            Assert.AreEqual(52, deck.CardsRemaining);
        }

        [Test]
        public void Deck_DealTooMany_ThrowsException()
        {
            var deck = new Deck();
            Assert.Throws<System.InvalidOperationException>(() => deck.Deal(53));
        }
    }
}
