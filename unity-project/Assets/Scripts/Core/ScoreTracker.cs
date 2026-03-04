using System;
using System.Collections.Generic;
using System.Linq;

namespace Hearts.Core
{
    /// <summary>
    /// Tracks scores across rounds for all 4 players.
    /// </summary>
    public class ScoreTracker
    {
        private readonly GameSettings _settings;

        /// <summary>
        /// Round-by-round scores. Each entry is an array of 4 ints (one per player seat).
        /// </summary>
        private readonly List<int[]> _roundScores = new List<int[]>();

        /// <summary>
        /// Cumulative scores for each player (indices 0–3).
        /// </summary>
        public int[] CumulativeScores { get; } = new int[4];

        /// <summary>
        /// Number of rounds scored so far.
        /// </summary>
        public int RoundsPlayed => _roundScores.Count;

        /// <summary>
        /// Read-only view of round-by-round scores.
        /// </summary>
        public IReadOnlyList<int[]> RoundScores => _roundScores.AsReadOnly();

        public ScoreTracker(GameSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Scores a completed round. Takes each player's collected penalty points (before Shoot the Moon adjustment).
        /// Returns the adjusted round scores after applying Shoot the Moon and Jack of Diamonds variants.
        /// </summary>
        public int[] ScoreRound(int[] penaltyPointsPerPlayer, bool[] hasJackOfDiamonds)
        {
            if (penaltyPointsPerPlayer.Length != 4)
                throw new ArgumentException("Must provide penalty points for exactly 4 players.");

            // Validate: total penalty points should be 26
            int totalPenalty = penaltyPointsPerPlayer.Sum();
            if (totalPenalty != 26)
                throw new ArgumentException($"Total penalty points must be 26, but got {totalPenalty}.");

            int[] roundScores = new int[4];

            // Check for Shooting the Moon
            int moonShooter = -1;
            for (int i = 0; i < 4; i++)
            {
                if (penaltyPointsPerPlayer[i] == 26)
                {
                    moonShooter = i;
                    break;
                }
            }

            if (moonShooter >= 0)
            {
                // Shooting the Moon!
                if (_settings.ShootTheMoonAddsToOthers)
                {
                    // Add 26 to all other players
                    for (int i = 0; i < 4; i++)
                    {
                        roundScores[i] = (i == moonShooter) ? 0 : 26;
                    }
                }
                else
                {
                    // Subtract 26 from the shooter
                    for (int i = 0; i < 4; i++)
                    {
                        roundScores[i] = 0;
                    }
                    // Subtraction applied to cumulative (handled below)
                    roundScores[moonShooter] = -26;
                }
            }
            else
            {
                // Normal scoring
                Array.Copy(penaltyPointsPerPlayer, roundScores, 4);
            }

            // Apply Jack of Diamonds bonus if enabled
            if (_settings.JackOfDiamondsEnabled && hasJackOfDiamonds != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (hasJackOfDiamonds[i])
                    {
                        roundScores[i] -= 10;
                    }
                }
            }

            // Apply to cumulative scores
            for (int i = 0; i < 4; i++)
            {
                CumulativeScores[i] += roundScores[i];
                // Enforce minimum score of 0
                if (CumulativeScores[i] < 0)
                    CumulativeScores[i] = 0;
            }

            _roundScores.Add(roundScores);
            return roundScores;
        }

        /// <summary>
        /// Whether the game should end (any player has reached the score limit).
        /// </summary>
        public bool IsGameOver => CumulativeScores.Any(s => s >= _settings.ScoreLimit);

        /// <summary>
        /// Returns the seat index of the winner (lowest score). Returns -1 if game is not over.
        /// If there's a tie, returns -1 (tie must be resolved by playing another round).
        /// </summary>
        public int GetWinnerSeat()
        {
            if (!IsGameOver)
                return -1;

            int minScore = CumulativeScores.Min();
            var tied = Enumerable.Range(0, 4).Where(i => CumulativeScores[i] == minScore).ToList();

            // If there's a tie, no winner yet — play another round
            if (tied.Count > 1)
                return -1;

            return tied[0];
        }

        /// <summary>
        /// Whether there is an unresolved tie for first place.
        /// </summary>
        public bool HasTie
        {
            get
            {
                if (!IsGameOver) return false;
                int minScore = CumulativeScores.Min();
                return CumulativeScores.Count(s => s == minScore) > 1;
            }
        }

        /// <summary>
        /// Resets all scores for a new game.
        /// </summary>
        public void Reset()
        {
            Array.Clear(CumulativeScores, 0, 4);
            _roundScores.Clear();
        }
    }
}
