using System;
using System.Collections.Generic;

namespace Hearts.Core
{
    /// <summary>
    /// Simple player implementation for testing. Stores actions and allows manual/automatic responses.
    /// Can be used as a base for AI players as well.
    /// </summary>
    public class TestPlayer : IPlayer
    {
        public int SeatIndex { get; }
        public string DisplayName { get; }
        public bool IsAI { get; set; }
        public bool IsConnected { get; set; } = true;
        public Hand Hand { get; } = new Hand();

        // Callbacks stored for manual resolution in tests
        private Action<List<Card>> _pendingPassCallback;
        private Action<Card> _pendingPlayCallback;

        // Track what happened for test assertions
        public List<Trick> CompletedTricksReceived { get; } = new List<Trick>();
        public int[] LastRoundScores { get; private set; }
        public int[] LastCumulativeScores { get; private set; }
        public int GameOverWinnerSeat { get; private set; } = -1;
        public bool GameIsOver { get; private set; }

        /// <summary>
        /// When true, auto-plays the first legal card and auto-passes the first 3 cards.
        /// </summary>
        public bool AutoPlay { get; set; }

        public TestPlayer(int seatIndex, string name = null, bool autoPlay = false)
        {
            SeatIndex = seatIndex;
            DisplayName = name ?? $"Player {seatIndex}";
            AutoPlay = autoPlay;
        }

        public void OnCardsDealt(List<Card> cards)
        {
            Hand.Clear();
            Hand.AddCards(cards);
        }

        public void RequestPassCards(PassDirection direction, Action<List<Card>> onCardsSelected)
        {
            if (AutoPlay)
            {
                // Auto-pass first 3 cards
                var toPass = new List<Card>();
                for (int i = 0; i < 3 && i < Hand.Cards.Count; i++)
                {
                    toPass.Add(Hand.Cards[i]);
                }
                onCardsSelected(toPass);
            }
            else
            {
                _pendingPassCallback = onCardsSelected;
            }
        }

        public void OnCardsReceived(List<Card> cards)
        {
            Hand.AddCards(cards);
        }

        public void RequestPlayCard(List<Card> legalPlays, Trick currentTrick, Action<Card> onCardPlayed)
        {
            if (AutoPlay)
            {
                // Auto-play first legal card
                onCardPlayed(legalPlays[0]);
            }
            else
            {
                _pendingPlayCallback = onCardPlayed;
            }
        }

        public void OnTrickComplete(Trick completedTrick, int winnerSeat)
        {
            CompletedTricksReceived.Add(completedTrick);
        }

        public void OnRoundComplete(int[] roundScores, int[] cumulativeScores)
        {
            LastRoundScores = roundScores;
            LastCumulativeScores = cumulativeScores;
        }

        public void OnGameOver(int winnerSeat, int[] finalScores)
        {
            GameOverWinnerSeat = winnerSeat;
            GameIsOver = true;
        }

        // Manual test control methods

        /// <summary>
        /// Manually resolve a pending pass selection (for non-auto tests).
        /// </summary>
        public void ResolvePass(List<Card> cards)
        {
            var cb = _pendingPassCallback;
            _pendingPassCallback = null;
            cb?.Invoke(cards);
        }

        /// <summary>
        /// Manually resolve a pending card play (for non-auto tests).
        /// </summary>
        public void ResolvePlay(Card card)
        {
            var cb = _pendingPlayCallback;
            _pendingPlayCallback = null;
            cb?.Invoke(card);
        }

        /// <summary>
        /// Whether there's a pending pass request waiting.
        /// </summary>
        public bool HasPendingPass => _pendingPassCallback != null;

        /// <summary>
        /// Whether there's a pending play request waiting.
        /// </summary>
        public bool HasPendingPlay => _pendingPlayCallback != null;
    }
}
