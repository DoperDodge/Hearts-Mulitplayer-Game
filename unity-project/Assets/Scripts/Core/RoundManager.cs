using System;
using System.Collections.Generic;
using System.Linq;

namespace Hearts.Core
{
    /// <summary>
    /// Manages a single round of Hearts: dealing, passing, 13 tricks, and scoring.
    /// This is the core game loop for one round.
    /// </summary>
    public class RoundManager
    {
        private readonly IPlayer[] _players;
        private readonly GameSettings _settings;
        private readonly int _roundNumber;

        private Deck _deck;
        private Trick _currentTrick;
        private int _tricksPlayed;
        private int _currentLeadSeat;
        private bool _heartsBroken;

        // Tracks penalty points collected by each player this round
        private readonly int[] _roundPenaltyPoints = new int[4];

        // Tracks whether each player collected the Jack of Diamonds
        private readonly bool[] _hasJackOfDiamonds = new bool[4];

        // Tracks all tricks played this round (for history/replay)
        private readonly List<Trick> _completedTricks = new List<Trick>();

        /// <summary>
        /// Current game phase within this round.
        /// </summary>
        public GamePhase Phase { get; private set; }

        /// <summary>
        /// Whether Hearts have been broken this round.
        /// </summary>
        public bool HeartsBroken => _heartsBroken;

        /// <summary>
        /// Number of tricks completed so far.
        /// </summary>
        public int TricksPlayed => _tricksPlayed;

        /// <summary>
        /// The current trick in progress (null if not in PlayingTrick phase).
        /// </summary>
        public Trick CurrentTrick => _currentTrick;

        /// <summary>
        /// The pass direction for this round.
        /// </summary>
        public PassDirection PassDirection { get; }

        /// <summary>
        /// Read-only view of completed tricks.
        /// </summary>
        public IReadOnlyList<Trick> CompletedTricks => _completedTricks.AsReadOnly();

        /// <summary>
        /// Event raised when the round phase changes.
        /// </summary>
        public event Action<GamePhase> OnPhaseChanged;

        /// <summary>
        /// Event raised when a card is played in a trick.
        /// Args: (playerSeat, card played)
        /// </summary>
        public event Action<int, Card> OnCardPlayed;

        /// <summary>
        /// Event raised when a trick is completed.
        /// Args: (trick, winnerSeat)
        /// </summary>
        public event Action<Trick, int> OnTrickCompleted;

        /// <summary>
        /// Event raised when Hearts are broken.
        /// </summary>
        public event Action OnHeartsBroken;

        /// <summary>
        /// Event raised when the round is complete.
        /// Args: (penaltyPointsPerPlayer, hasJackOfDiamonds)
        /// </summary>
        public event Action<int[], bool[]> OnRoundComplete;

        public RoundManager(IPlayer[] players, GameSettings settings, int roundNumber, int? seed = null)
        {
            if (players.Length != 4)
                throw new ArgumentException("Hearts requires exactly 4 players.");

            _players = players;
            _settings = settings;
            _roundNumber = roundNumber;
            PassDirection = settings.PassingEnabled
                ? PassDirectionExtensions.ForRound(roundNumber)
                : PassDirection.Hold;

            _deck = new Deck(seed);
            Phase = GamePhase.Dealing;
        }

        /// <summary>
        /// Starts the round: shuffles, deals, then initiates passing or trick play.
        /// </summary>
        public void StartRound()
        {
            SetPhase(GamePhase.Dealing);
            DealCards();

            if (PassDirection == PassDirection.Hold)
            {
                // Skip passing, go straight to trick play
                StartTrickPlay();
            }
            else
            {
                SetPhase(GamePhase.PassingCards);
                BeginPassingPhase();
            }
        }

        private void DealCards()
        {
            _deck.Reset();
            _deck.Shuffle();

            var hands = _deck.DealAll(4);

            for (int i = 0; i < 4; i++)
            {
                _players[i].OnCardsDealt(hands[i]);
            }
        }

        #region Passing Phase

        private int _passesReceived;
        private readonly Card[][] _selectedPasses = new Card[4][];

        private void BeginPassingPhase()
        {
            _passesReceived = 0;

            for (int i = 0; i < 4; i++)
            {
                int seat = i; // Capture for closure
                _players[i].RequestPassCards(PassDirection, (selectedCards) =>
                {
                    OnPlayerSelectedPassCards(seat, selectedCards);
                });
            }
        }

        private void OnPlayerSelectedPassCards(int seat, List<Card> selectedCards)
        {
            if (selectedCards == null || selectedCards.Count != 3)
                throw new ArgumentException($"Player {seat} must select exactly 3 cards to pass.");

            // Verify all cards are actually in the player's hand
            foreach (var card in selectedCards)
            {
                if (!_players[seat].Hand.Contains(card))
                    throw new ArgumentException($"Player {seat} tried to pass {card} which is not in their hand.");
            }

            _selectedPasses[seat] = selectedCards.ToArray();
            _passesReceived++;

            if (_passesReceived == 4)
            {
                ExecutePasses();
            }
        }

        private void ExecutePasses()
        {
            // Remove cards from each player's hand
            for (int i = 0; i < 4; i++)
            {
                _players[i].Hand.RemoveCards(_selectedPasses[i]);
            }

            // Add cards to target players
            for (int i = 0; i < 4; i++)
            {
                int targetSeat = PassDirection.GetTargetSeat(i);
                var passedCards = new List<Card>(_selectedPasses[i]);
                _players[targetSeat].OnCardsReceived(passedCards);
            }

            StartTrickPlay();
        }

        #endregion

        #region Trick Play

        private void StartTrickPlay()
        {
            _tricksPlayed = 0;
            _heartsBroken = false;

            // Find who has the 2 of Clubs — they lead first
            _currentLeadSeat = -1;
            for (int i = 0; i < 4; i++)
            {
                if (_players[i].Hand.HasTwoOfClubs)
                {
                    _currentLeadSeat = i;
                    break;
                }
            }

            if (_currentLeadSeat == -1)
                throw new InvalidOperationException("No player has the 2 of Clubs.");

            StartNextTrick();
        }

        private void StartNextTrick()
        {
            SetPhase(GamePhase.PlayingTrick);
            _currentTrick = new Trick(_currentLeadSeat);
            RequestNextPlay();
        }

        private void RequestNextPlay()
        {
            int nextSeat = _currentTrick.GetNextPlayerSeat();
            bool isFirstTrick = _tricksPlayed == 0 && _settings.FirstTrickPenaltyRestriction;

            // First card of first trick must be 2 of Clubs
            if (_tricksPlayed == 0 && _currentTrick.CardCount == 0)
            {
                var twoOfClubs = new Card(Suit.Clubs, Rank.Two);
                var forcedPlay = new List<Card> { twoOfClubs };
                _players[nextSeat].RequestPlayCard(forcedPlay, _currentTrick, (card) =>
                {
                    OnPlayerPlayedCard(nextSeat, card);
                });
                return;
            }

            var legalPlays = _players[nextSeat].Hand.GetLegalPlays(
                _currentTrick.LedSuit,
                isFirstTrick,
                _heartsBroken,
                _currentTrick.CardCount == 0 // isLeading
            );

            _players[nextSeat].RequestPlayCard(legalPlays, _currentTrick, (card) =>
            {
                OnPlayerPlayedCard(nextSeat, card);
            });
        }

        private void OnPlayerPlayedCard(int seat, Card card)
        {
            // Validate the play
            if (!_players[seat].Hand.Contains(card))
                throw new InvalidOperationException($"Player {seat} played {card} which is not in their hand.");

            // Remove from hand and play into trick
            _players[seat].Hand.RemoveCard(card);
            _currentTrick.PlayCard(seat, card);

            // Check if Hearts are broken
            if (!_heartsBroken && card.Suit == Suit.Hearts)
            {
                _heartsBroken = true;
                OnHeartsBroken?.Invoke();
            }

            OnCardPlayed?.Invoke(seat, card);

            if (_currentTrick.IsComplete)
            {
                CompleteTrick();
            }
            else
            {
                RequestNextPlay();
            }
        }

        private void CompleteTrick()
        {
            SetPhase(GamePhase.TrickComplete);

            int winnerSeat = _currentTrick.GetWinnerSeat();
            int penaltyPoints = _currentTrick.PenaltyPoints;

            _roundPenaltyPoints[winnerSeat] += penaltyPoints;

            // Check for Jack of Diamonds
            if (_settings.JackOfDiamondsEnabled)
            {
                if (_currentTrick.Plays.Any(p => p.Card.IsJackOfDiamonds))
                {
                    _hasJackOfDiamonds[winnerSeat] = true;
                }
            }

            _completedTricks.Add(_currentTrick);

            // Notify all players
            for (int i = 0; i < 4; i++)
            {
                _players[i].OnTrickComplete(_currentTrick, winnerSeat);
            }

            OnTrickCompleted?.Invoke(_currentTrick, winnerSeat);

            _tricksPlayed++;

            if (_tricksPlayed == 13)
            {
                // Round is complete
                CompleteRound();
            }
            else
            {
                // Winner leads next trick
                _currentLeadSeat = winnerSeat;
                StartNextTrick();
            }
        }

        #endregion

        #region Round Completion

        private void CompleteRound()
        {
            SetPhase(GamePhase.ScoringRound);
            OnRoundComplete?.Invoke(_roundPenaltyPoints, _hasJackOfDiamonds);
        }

        /// <summary>
        /// Gets the raw penalty points collected by each player this round (before Shoot the Moon adjustment).
        /// </summary>
        public int[] GetRoundPenaltyPoints() => (int[])_roundPenaltyPoints.Clone();

        /// <summary>
        /// Gets whether each player collected the Jack of Diamonds this round.
        /// </summary>
        public bool[] GetJackOfDiamondsStatus() => (bool[])_hasJackOfDiamonds.Clone();

        #endregion

        private void SetPhase(GamePhase newPhase)
        {
            Phase = newPhase;
            OnPhaseChanged?.Invoke(newPhase);
        }
    }
}
