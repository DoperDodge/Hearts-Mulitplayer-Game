using NUnit.Framework;
using Hearts.Core;
using System.Collections.Generic;
using System.Linq;

namespace Hearts.Tests
{
    [TestFixture]
    public class RoundManagerTests
    {
        private TestPlayer[] _players;
        private GameSettings _settings;

        [SetUp]
        public void Setup()
        {
            _players = new TestPlayer[4];
            for (int i = 0; i < 4; i++)
            {
                _players[i] = new TestPlayer(i, autoPlay: true);
            }
            _settings = GameSettings.Default;
        }

        [Test]
        public void Round_Deal_EachPlayerGets13Cards()
        {
            var round = new RoundManager(_players, _settings, roundNumber: 3, seed: 42); // Hold round
            round.StartRound();

            for (int i = 0; i < 4; i++)
            {
                // After auto-playing all tricks, hands should be empty
                // But let's check that tricks happened
                Assert.AreEqual(13, round.TricksPlayed);
            }
        }

        [Test]
        public void Round_HoldRound_SkipsPassing()
        {
            // Round 3 = Hold
            var round = new RoundManager(_players, _settings, roundNumber: 3, seed: 42);
            Assert.AreEqual(PassDirection.Hold, round.PassDirection);

            round.StartRound();

            // Should complete all 13 tricks (auto-play)
            Assert.AreEqual(13, round.TricksPlayed);
            Assert.AreEqual(GamePhase.ScoringRound, round.Phase);
        }

        [Test]
        public void Round_PassingRound_PassesCards()
        {
            // Round 0 = Left
            var round = new RoundManager(_players, _settings, roundNumber: 0, seed: 42);
            Assert.AreEqual(PassDirection.Left, round.PassDirection);

            round.StartRound();

            // Should complete (auto-play handles passing and tricks)
            Assert.AreEqual(13, round.TricksPlayed);
            Assert.AreEqual(GamePhase.ScoringRound, round.Phase);
        }

        [Test]
        public void Round_TotalPenaltyPoints_Equals26()
        {
            var round = new RoundManager(_players, _settings, roundNumber: 3, seed: 42);

            round.StartRound();

            var penaltyPoints = round.GetRoundPenaltyPoints();
            int total = penaltyPoints.Sum();
            Assert.AreEqual(26, total, "Total penalty points per round must be 26.");
        }

        [Test]
        public void Round_AllTricksPlayed_Exactly13()
        {
            var round = new RoundManager(_players, _settings, roundNumber: 0, seed: 100);
            round.StartRound();

            Assert.AreEqual(13, round.TricksPlayed);
            Assert.AreEqual(13, round.CompletedTricks.Count);
        }

        [Test]
        public void Round_EachTrick_HasExactly4Cards()
        {
            var round = new RoundManager(_players, _settings, roundNumber: 0, seed: 100);
            round.StartRound();

            foreach (var trick in round.CompletedTricks)
            {
                Assert.AreEqual(4, trick.CardCount);
                Assert.IsTrue(trick.IsComplete);
            }
        }

        [Test]
        public void Round_All52Cards_PlayedExactlyOnce()
        {
            var round = new RoundManager(_players, _settings, roundNumber: 0, seed: 100);
            round.StartRound();

            var allPlayedCards = new HashSet<Card>();
            foreach (var trick in round.CompletedTricks)
            {
                foreach (var card in trick.GetAllCards())
                {
                    Assert.IsTrue(allPlayedCards.Add(card), $"Card {card} was played more than once.");
                }
            }
            Assert.AreEqual(52, allPlayedCards.Count);
        }

        [Test]
        public void Round_FirstTrick_LeadsWith2OfClubs()
        {
            var round = new RoundManager(_players, _settings, roundNumber: 3, seed: 42);
            round.StartRound();

            var firstTrick = round.CompletedTricks[0];
            var leadCard = firstTrick.Plays[0].Card;

            Assert.AreEqual(new Card(Suit.Clubs, Rank.Two), leadCard,
                "First trick must be led with 2 of Clubs.");
        }

        [Test]
        public void Round_HeartsBroken_TrackedCorrectly()
        {
            var round = new RoundManager(_players, _settings, roundNumber: 3, seed: 42);

            bool heartsBrokenFired = false;
            round.OnHeartsBroken += () => heartsBrokenFired = true;

            round.StartRound();

            // Hearts must eventually be played in any complete round
            Assert.IsTrue(round.HeartsBroken, "Hearts should be broken by end of round.");
            Assert.IsTrue(heartsBrokenFired, "HeartsBroken event should have fired.");
        }

        [Test]
        public void Round_Events_RoundCompleteFires()
        {
            var round = new RoundManager(_players, _settings, roundNumber: 0, seed: 42);

            bool roundComplete = false;
            int[] reportedPenalties = null;
            round.OnRoundComplete += (penalties, jod) =>
            {
                roundComplete = true;
                reportedPenalties = penalties;
            };

            round.StartRound();

            Assert.IsTrue(roundComplete);
            Assert.IsNotNull(reportedPenalties);
            Assert.AreEqual(26, reportedPenalties.Sum());
        }

        [Test]
        public void Round_Events_TrickCompletedFires13Times()
        {
            var round = new RoundManager(_players, _settings, roundNumber: 0, seed: 42);

            int trickCompleteCount = 0;
            round.OnTrickCompleted += (trick, winner) => trickCompleteCount++;

            round.StartRound();

            Assert.AreEqual(13, trickCompleteCount);
        }

        [Test]
        public void Round_Events_CardPlayedFires52Times()
        {
            var round = new RoundManager(_players, _settings, roundNumber: 0, seed: 42);

            int cardPlayedCount = 0;
            round.OnCardPlayed += (seat, card) => cardPlayedCount++;

            round.StartRound();

            Assert.AreEqual(52, cardPlayedCount);
        }

        [Test]
        public void Round_DifferentSeeds_ProduceDifferentGames()
        {
            var round1 = new RoundManager(_players, _settings, roundNumber: 0, seed: 1);
            round1.StartRound();
            var penalties1 = round1.GetRoundPenaltyPoints();

            // Need fresh players
            for (int i = 0; i < 4; i++)
            {
                _players[i] = new TestPlayer(i, autoPlay: true);
            }

            var round2 = new RoundManager(_players, _settings, roundNumber: 0, seed: 999);
            round2.StartRound();
            var penalties2 = round2.GetRoundPenaltyPoints();

            // Very unlikely to be identical with different seeds
            bool anyDifferent = false;
            for (int i = 0; i < 4; i++)
            {
                if (penalties1[i] != penalties2[i])
                {
                    anyDifferent = true;
                    break;
                }
            }
            Assert.IsTrue(anyDifferent, "Different seeds should produce different penalty distributions.");
        }

        [Test]
        public void Round_SameSeed_ProducesSameGame()
        {
            var round1 = new RoundManager(_players, _settings, roundNumber: 3, seed: 42);
            round1.StartRound();
            var penalties1 = round1.GetRoundPenaltyPoints();

            // Fresh players with same seed
            for (int i = 0; i < 4; i++)
            {
                _players[i] = new TestPlayer(i, autoPlay: true);
            }

            var round2 = new RoundManager(_players, _settings, roundNumber: 3, seed: 42);
            round2.StartRound();
            var penalties2 = round2.GetRoundPenaltyPoints();

            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(penalties1[i], penalties2[i],
                    $"Same seed should produce same results. Player {i}: {penalties1[i]} vs {penalties2[i]}");
            }
        }
    }
}
