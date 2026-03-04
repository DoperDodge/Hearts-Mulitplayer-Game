using NUnit.Framework;
using Hearts.Core;

namespace Hearts.Tests
{
    [TestFixture]
    public class GameManagerTests
    {
        private GameManager _game;
        private TestPlayer[] _players;

        [SetUp]
        public void Setup()
        {
            _game = new GameManager(GameSettings.Default);
            _players = new TestPlayer[4];
            for (int i = 0; i < 4; i++)
            {
                _players[i] = new TestPlayer(i, autoPlay: true);
                _game.SetPlayer(i, _players[i]);
            }
        }

        [Test]
        public void GameManager_AllSeatsOccupied_WhenFull()
        {
            Assert.IsTrue(_game.AllSeatsOccupied);
        }

        [Test]
        public void GameManager_AllSeatsOccupied_FalseWhenMissing()
        {
            var game = new GameManager();
            game.SetPlayer(0, _players[0]);
            Assert.IsFalse(game.AllSeatsOccupied);
        }

        [Test]
        public void GameManager_StartGame_ThrowsIfNotFull()
        {
            var game = new GameManager();
            game.SetPlayer(0, _players[0]);
            Assert.Throws<System.InvalidOperationException>(() => game.StartGame());
        }

        [Test]
        public void GameManager_StartGame_PlaysToCompletion()
        {
            bool gameOverFired = false;
            int winnerSeat = -1;
            _game.OnGameOver += (winner, scores) =>
            {
                gameOverFired = true;
                winnerSeat = winner;
            };

            _game.StartGame();

            Assert.IsTrue(gameOverFired, "Game should eventually end.");
            Assert.IsTrue(winnerSeat >= 0 && winnerSeat <= 3, "Winner should be a valid seat.");
        }

        [Test]
        public void GameManager_GameOver_WinnerHasLowestScore()
        {
            int[] finalScores = null;
            int winnerSeat = -1;
            _game.OnGameOver += (winner, scores) =>
            {
                winnerSeat = winner;
                finalScores = scores;
            };

            _game.StartGame();

            Assert.IsNotNull(finalScores);
            int winnerScore = finalScores[winnerSeat];
            for (int i = 0; i < 4; i++)
            {
                Assert.LessOrEqual(winnerScore, finalScores[i],
                    $"Winner (P{winnerSeat}, score {winnerScore}) should have lowest or tied score, but P{i} has {finalScores[i]}.");
            }
        }

        [Test]
        public void GameManager_GameOver_SomeoneReached100()
        {
            int[] finalScores = null;
            _game.OnGameOver += (winner, scores) => finalScores = scores;

            _game.StartGame();

            Assert.IsNotNull(finalScores);
            bool someoneOver100 = false;
            for (int i = 0; i < 4; i++)
            {
                if (finalScores[i] >= 100)
                    someoneOver100 = true;
            }
            Assert.IsTrue(someoneOver100, "Game should end when someone reaches 100.");
        }

        [Test]
        public void GameManager_PlayersNotified_GameOver()
        {
            _game.StartGame();

            for (int i = 0; i < 4; i++)
            {
                Assert.IsTrue(_players[i].GameIsOver,
                    $"Player {i} should have been notified of game over.");
            }
        }

        [Test]
        public void GameManager_RoundEvents_Fire()
        {
            int roundsStarted = 0;
            int roundsScored = 0;
            _game.OnRoundStarted += (num, dir) => roundsStarted++;
            _game.OnRoundScored += (scores, cumulative) => roundsScored++;

            _game.StartGame();

            Assert.Greater(roundsStarted, 0);
            Assert.AreEqual(roundsStarted, roundsScored);
        }

        [Test]
        public void GameManager_CustomScoreLimit_50()
        {
            var settings = new GameSettings { ScoreLimit = 50 };
            var game = new GameManager(settings);
            for (int i = 0; i < 4; i++)
            {
                _players[i] = new TestPlayer(i, autoPlay: true);
                game.SetPlayer(i, _players[i]);
            }

            int[] finalScores = null;
            game.OnGameOver += (winner, scores) => finalScores = scores;
            game.StartGame();

            bool someoneOver50 = false;
            for (int i = 0; i < 4; i++)
            {
                if (finalScores[i] >= 50)
                    someoneOver50 = true;
            }
            Assert.IsTrue(someoneOver50, "Game with limit 50 should end when someone reaches 50.");
        }

        [Test]
        public void GameManager_ReplacePlayer_TransfersHand()
        {
            // We can't easily test mid-game replacement with auto-play,
            // but we can test the hand transfer mechanism
            var game = new GameManager();
            for (int i = 0; i < 4; i++)
            {
                game.SetPlayer(i, new TestPlayer(i));
            }

            // Manually add cards to player 0
            game.Players[0].Hand.AddCards(new System.Collections.Generic.List<Card>
            {
                new Card(Suit.Hearts, Rank.Ace),
                new Card(Suit.Clubs, Rank.King)
            });

            var newPlayer = new TestPlayer(0, name: "Bot 0");
            game.ReplacePlayer(0, newPlayer);

            Assert.AreEqual(2, newPlayer.Hand.Count);
            Assert.IsTrue(newPlayer.Hand.Contains(new Card(Suit.Hearts, Rank.Ace)));
            Assert.IsTrue(newPlayer.Hand.Contains(new Card(Suit.Clubs, Rank.King)));
        }
    }
}
