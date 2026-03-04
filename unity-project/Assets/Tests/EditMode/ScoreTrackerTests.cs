using NUnit.Framework;
using Hearts.Core;

namespace Hearts.Tests
{
    [TestFixture]
    public class ScoreTrackerTests
    {
        private GameSettings _settings;
        private ScoreTracker _tracker;

        [SetUp]
        public void Setup()
        {
            _settings = GameSettings.Default;
            _tracker = new ScoreTracker(_settings);
        }

        [Test]
        public void ScoreTracker_Initial_AllZeros()
        {
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(0, _tracker.CumulativeScores[i]);
            }
            Assert.AreEqual(0, _tracker.RoundsPlayed);
            Assert.IsFalse(_tracker.IsGameOver);
        }

        [Test]
        public void ScoreTracker_NormalRound_AddsPoints()
        {
            var penalties = new int[] { 5, 8, 0, 13 };
            var jackOfDiamonds = new bool[] { false, false, false, false };

            _tracker.ScoreRound(penalties, jackOfDiamonds);

            Assert.AreEqual(5, _tracker.CumulativeScores[0]);
            Assert.AreEqual(8, _tracker.CumulativeScores[1]);
            Assert.AreEqual(0, _tracker.CumulativeScores[2]);
            Assert.AreEqual(13, _tracker.CumulativeScores[3]);
            Assert.AreEqual(1, _tracker.RoundsPlayed);
        }

        [Test]
        public void ScoreTracker_InvalidTotal_Throws()
        {
            var penalties = new int[] { 5, 8, 0, 10 }; // Total = 23, not 26
            var jackOfDiamonds = new bool[] { false, false, false, false };

            Assert.Throws<System.ArgumentException>(
                () => _tracker.ScoreRound(penalties, jackOfDiamonds));
        }

        [Test]
        public void ScoreTracker_ShootTheMoon_AddsToOthers()
        {
            _settings.ShootTheMoonAddsToOthers = true;

            var penalties = new int[] { 26, 0, 0, 0 }; // Player 0 shoots the moon
            var jackOfDiamonds = new bool[] { false, false, false, false };

            var roundScores = _tracker.ScoreRound(penalties, jackOfDiamonds);

            Assert.AreEqual(0, roundScores[0]);   // Shooter gets 0
            Assert.AreEqual(26, roundScores[1]);  // Others get 26
            Assert.AreEqual(26, roundScores[2]);
            Assert.AreEqual(26, roundScores[3]);

            Assert.AreEqual(0, _tracker.CumulativeScores[0]);
            Assert.AreEqual(26, _tracker.CumulativeScores[1]);
        }

        [Test]
        public void ScoreTracker_ShootTheMoon_SubtractFromSelf()
        {
            _settings.ShootTheMoonAddsToOthers = false;

            // Give player 0 some existing score first
            _tracker.ScoreRound(new int[] { 0, 0, 0, 26 }, new bool[] { false, false, false, false });
            // Now player 3 has 26 from being shot at. Let's give player 0 some points.
            // Reset and start fresh for clarity:
            _tracker.Reset();
            // Give player 0 a score of 40
            _settings.ShootTheMoonAddsToOthers = true;
            _tracker.ScoreRound(new int[] { 14, 12, 0, 0 }, new bool[] { false, false, false, false });
            Assert.AreEqual(14, _tracker.CumulativeScores[0]);

            // Now player 0 shoots the moon with subtract variant
            _settings.ShootTheMoonAddsToOthers = false;
            var roundScores = _tracker.ScoreRound(new int[] { 26, 0, 0, 0 }, new bool[] { false, false, false, false });

            Assert.AreEqual(-26, roundScores[0]); // -26 round score
            Assert.AreEqual(0, _tracker.CumulativeScores[0]); // 14 - 26 = -12, clamped to 0
        }

        [Test]
        public void ScoreTracker_ShootTheMoon_SubtractClampsToZero()
        {
            _settings.ShootTheMoonAddsToOthers = false;

            // Player 0 starts with 10 points
            _tracker.ScoreRound(new int[] { 10, 10, 3, 3 }, new bool[] { false, false, false, false });

            // Player 0 shoots the moon, should go from 10 to 0 (not -16)
            _tracker.ScoreRound(new int[] { 26, 0, 0, 0 }, new bool[] { false, false, false, false });

            Assert.AreEqual(0, _tracker.CumulativeScores[0]);
        }

        [Test]
        public void ScoreTracker_JackOfDiamonds_SubtractsPoints()
        {
            _settings.JackOfDiamondsEnabled = true;

            var penalties = new int[] { 5, 8, 0, 13 };
            var jackOfDiamonds = new bool[] { false, false, true, false }; // Player 2 got J♦

            var roundScores = _tracker.ScoreRound(penalties, jackOfDiamonds);

            Assert.AreEqual(5, roundScores[0]);
            Assert.AreEqual(8, roundScores[1]);
            Assert.AreEqual(-10, roundScores[2]); // 0 - 10
            Assert.AreEqual(13, roundScores[3]);

            // Cumulative clamped to 0
            Assert.AreEqual(0, _tracker.CumulativeScores[2]);
        }

        [Test]
        public void ScoreTracker_GameOver_At100()
        {
            // Play multiple rounds to reach 100
            _tracker.ScoreRound(new int[] { 26, 0, 0, 0 }, new bool[] { false, false, false, false });
            Assert.IsFalse(_tracker.IsGameOver);

            _tracker.ScoreRound(new int[] { 26, 0, 0, 0 }, new bool[] { false, false, false, false });
            Assert.IsFalse(_tracker.IsGameOver);

            _tracker.ScoreRound(new int[] { 26, 0, 0, 0 }, new bool[] { false, false, false, false });
            Assert.IsFalse(_tracker.IsGameOver); // 78

            _tracker.ScoreRound(new int[] { 22, 4, 0, 0 }, new bool[] { false, false, false, false });
            Assert.IsTrue(_tracker.IsGameOver); // Player 0 at 100
        }

        [Test]
        public void ScoreTracker_Winner_LowestScore()
        {
            // Get player 0 to 100
            _tracker.ScoreRound(new int[] { 26, 0, 0, 0 }, new bool[] { false, false, false, false });
            _tracker.ScoreRound(new int[] { 26, 0, 0, 0 }, new bool[] { false, false, false, false });
            _tracker.ScoreRound(new int[] { 26, 0, 0, 0 }, new bool[] { false, false, false, false });
            _tracker.ScoreRound(new int[] { 22, 4, 0, 0 }, new bool[] { false, false, false, false });

            int winner = _tracker.GetWinnerSeat();
            // Players 2 and 3 both have 0 — tie
            Assert.AreEqual(-1, winner); // Tie, no single winner
            Assert.IsTrue(_tracker.HasTie);
        }

        [Test]
        public void ScoreTracker_Winner_SingleWinner()
        {
            _tracker.ScoreRound(new int[] { 26, 0, 0, 0 }, new bool[] { false, false, false, false });
            _tracker.ScoreRound(new int[] { 26, 0, 0, 0 }, new bool[] { false, false, false, false });
            _tracker.ScoreRound(new int[] { 26, 0, 0, 0 }, new bool[] { false, false, false, false });
            _tracker.ScoreRound(new int[] { 22, 3, 1, 0 }, new bool[] { false, false, false, false });

            Assert.IsTrue(_tracker.IsGameOver);
            Assert.AreEqual(3, _tracker.GetWinnerSeat()); // Player 3 has 0
            Assert.IsFalse(_tracker.HasTie);
        }

        [Test]
        public void ScoreTracker_CustomScoreLimit()
        {
            _settings.ScoreLimit = 50;
            _tracker = new ScoreTracker(_settings);

            _tracker.ScoreRound(new int[] { 26, 0, 0, 0 }, new bool[] { false, false, false, false });
            Assert.IsFalse(_tracker.IsGameOver); // 26 < 50

            _tracker.ScoreRound(new int[] { 24, 2, 0, 0 }, new bool[] { false, false, false, false });
            Assert.IsTrue(_tracker.IsGameOver); // 50 >= 50
        }

        [Test]
        public void ScoreTracker_Reset_ClearsEverything()
        {
            _tracker.ScoreRound(new int[] { 10, 10, 3, 3 }, new bool[] { false, false, false, false });
            _tracker.Reset();

            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(0, _tracker.CumulativeScores[i]);
            }
            Assert.AreEqual(0, _tracker.RoundsPlayed);
        }

        [Test]
        public void ScoreTracker_MultipleRounds_AccumulateCorrectly()
        {
            _tracker.ScoreRound(new int[] { 5, 8, 0, 13 }, new bool[] { false, false, false, false });
            _tracker.ScoreRound(new int[] { 3, 10, 13, 0 }, new bool[] { false, false, false, false });

            Assert.AreEqual(8, _tracker.CumulativeScores[0]);   // 5 + 3
            Assert.AreEqual(18, _tracker.CumulativeScores[1]);  // 8 + 10
            Assert.AreEqual(13, _tracker.CumulativeScores[2]);  // 0 + 13
            Assert.AreEqual(13, _tracker.CumulativeScores[3]);  // 13 + 0
            Assert.AreEqual(2, _tracker.RoundsPlayed);
        }
    }
}
