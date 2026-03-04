using System;
using System.Linq;

namespace Hearts.Core
{
    /// <summary>
    /// Top-level game manager. Orchestrates multiple rounds until the game ends.
    /// Manages players, scoring, and the overall game lifecycle.
    /// </summary>
    public class GameManager
    {
        private readonly IPlayer[] _players = new IPlayer[4];
        private readonly GameSettings _settings;
        private readonly ScoreTracker _scoreTracker;

        private RoundManager _currentRound;
        private int _roundNumber;

        /// <summary>
        /// Current game phase.
        /// </summary>
        public GamePhase Phase { get; private set; }

        /// <summary>
        /// The current round manager (null if no round in progress).
        /// </summary>
        public RoundManager CurrentRound => _currentRound;

        /// <summary>
        /// The score tracker for this game.
        /// </summary>
        public ScoreTracker Scores => _scoreTracker;

        /// <summary>
        /// Current round number (0-indexed).
        /// </summary>
        public int RoundNumber => _roundNumber;

        /// <summary>
        /// Read-only view of all players.
        /// </summary>
        public IPlayer[] Players => _players;

        /// <summary>
        /// Game settings.
        /// </summary>
        public GameSettings Settings => _settings;

        /// <summary>
        /// Event raised when a new round begins. Args: (roundNumber, passDirection)
        /// </summary>
        public event Action<int, PassDirection> OnRoundStarted;

        /// <summary>
        /// Event raised when a round's scoring is complete. Args: (roundScores, cumulativeScores)
        /// </summary>
        public event Action<int[], int[]> OnRoundScored;

        /// <summary>
        /// Event raised when the game ends. Args: (winnerSeat, finalScores)
        /// </summary>
        public event Action<int, int[]> OnGameOver;

        /// <summary>
        /// Event raised when the game phase changes.
        /// </summary>
        public event Action<GamePhase> OnPhaseChanged;

        public GameManager(GameSettings settings = null)
        {
            _settings = settings ?? GameSettings.Default;
            _scoreTracker = new ScoreTracker(_settings);
        }

        /// <summary>
        /// Sets a player in the given seat (0–3).
        /// </summary>
        public void SetPlayer(int seat, IPlayer player)
        {
            if (seat < 0 || seat > 3)
                throw new ArgumentOutOfRangeException(nameof(seat), "Seat must be 0–3.");

            _players[seat] = player;
        }

        /// <summary>
        /// Whether all 4 seats have players assigned.
        /// </summary>
        public bool AllSeatsOccupied => _players.All(p => p != null);

        /// <summary>
        /// Starts a new game. All 4 player seats must be filled.
        /// </summary>
        public void StartGame()
        {
            if (!AllSeatsOccupied)
                throw new InvalidOperationException("All 4 player seats must be filled before starting.");

            _scoreTracker.Reset();
            _roundNumber = 0;

            StartNextRound();
        }

        /// <summary>
        /// Starts the next round of the game.
        /// </summary>
        private void StartNextRound()
        {
            _currentRound = new RoundManager(_players, _settings, _roundNumber);

            // Subscribe to round events
            _currentRound.OnPhaseChanged += (phase) => SetPhase(phase);
            _currentRound.OnRoundComplete += OnCurrentRoundComplete;

            OnRoundStarted?.Invoke(_roundNumber, _currentRound.PassDirection);
            _currentRound.StartRound();
        }

        private void OnCurrentRoundComplete(int[] penaltyPoints, bool[] hasJackOfDiamonds)
        {
            // Score the round
            int[] roundScores = _scoreTracker.ScoreRound(penaltyPoints, hasJackOfDiamonds);

            // Notify players
            for (int i = 0; i < 4; i++)
            {
                _players[i].OnRoundComplete(roundScores, (int[])_scoreTracker.CumulativeScores.Clone());
            }

            OnRoundScored?.Invoke(roundScores, (int[])_scoreTracker.CumulativeScores.Clone());

            // Check for game end
            if (_scoreTracker.IsGameOver && !_scoreTracker.HasTie)
            {
                EndGame();
            }
            else
            {
                // Continue to next round (even if someone hit the limit, if there's a tie we keep going)
                _roundNumber++;
                StartNextRound();
            }
        }

        private void EndGame()
        {
            SetPhase(GamePhase.GameOver);

            int winnerSeat = _scoreTracker.GetWinnerSeat();
            int[] finalScores = (int[])_scoreTracker.CumulativeScores.Clone();

            for (int i = 0; i < 4; i++)
            {
                _players[i].OnGameOver(winnerSeat, finalScores);
            }

            OnGameOver?.Invoke(winnerSeat, finalScores);
        }

        private void SetPhase(GamePhase newPhase)
        {
            Phase = newPhase;
            OnPhaseChanged?.Invoke(newPhase);
        }

        /// <summary>
        /// Replaces a player at the given seat (e.g., for disconnection → AI substitution).
        /// The new player inherits the existing hand.
        /// </summary>
        public void ReplacePlayer(int seat, IPlayer newPlayer)
        {
            if (seat < 0 || seat > 3)
                throw new ArgumentOutOfRangeException(nameof(seat), "Seat must be 0–3.");

            var oldHand = _players[seat].Hand;
            _players[seat] = newPlayer;

            // Transfer the hand to the new player
            newPlayer.Hand.Clear();
            newPlayer.Hand.AddCards(oldHand.Cards);
        }
    }
}
