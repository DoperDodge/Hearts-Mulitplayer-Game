using System;
using System.Collections.Generic;
using System.Linq;

namespace Hearts.Core
{
    /// <summary>
    /// Simple AI player that makes basic strategic decisions.
    /// Used for the debug prototype and as a foundation for smarter AI later.
    /// </summary>
    public class AIPlayer : IPlayer
    {
        public int SeatIndex { get; }
        public string DisplayName { get; }
        public bool IsAI => true;
        public bool IsConnected => true;
        public Hand Hand { get; } = new Hand();

        public AIPlayer(int seatIndex, string name = null)
        {
            SeatIndex = seatIndex;
            DisplayName = name ?? $"Bot {seatIndex}";
        }

        public void OnCardsDealt(List<Card> cards)
        {
            Hand.Clear();
            Hand.AddCards(cards);
        }

        public void RequestPassCards(PassDirection direction, Action<List<Card>> onCardsSelected)
        {
            // Strategy: pass highest penalty-risk cards
            var toPass = new List<Card>();

            // Try to pass Queen of Spades first
            var queenOfSpades = Hand.Cards.FirstOrDefault(c => c.IsQueenOfSpades);
            if (queenOfSpades.Suit == Suit.Spades && queenOfSpades.Rank == Rank.Queen)
                toPass.Add(queenOfSpades);

            // Pass high spades (Ace, King) that could force us to take the Queen
            foreach (var card in Hand.Cards.Where(c => c.Suit == Suit.Spades && c.Rank >= Rank.King).OrderByDescending(c => c.Rank))
            {
                if (toPass.Count >= 3) break;
                if (!toPass.Contains(card))
                    toPass.Add(card);
            }

            // Fill remaining with highest hearts
            foreach (var card in Hand.Cards.Where(c => c.Suit == Suit.Hearts).OrderByDescending(c => c.Rank))
            {
                if (toPass.Count >= 3) break;
                if (!toPass.Contains(card))
                    toPass.Add(card);
            }

            // Fill remaining with highest cards of any suit
            foreach (var card in Hand.Cards.OrderByDescending(c => c.Rank))
            {
                if (toPass.Count >= 3) break;
                if (!toPass.Contains(card))
                    toPass.Add(card);
            }

            onCardsSelected(toPass.Take(3).ToList());
        }

        public void OnCardsReceived(List<Card> cards)
        {
            Hand.AddCards(cards);
        }

        public void RequestPlayCard(List<Card> legalPlays, Trick currentTrick, Action<Card> onCardPlayed)
        {
            Card chosen = ChooseCard(legalPlays, currentTrick);
            onCardPlayed(chosen);
        }

        private Card ChooseCard(List<Card> legalPlays, Trick currentTrick)
        {
            if (legalPlays.Count == 1)
                return legalPlays[0];

            bool isLeading = currentTrick.CardCount == 0;

            if (isLeading)
                return ChooseLead(legalPlays);

            bool isLastToPlay = currentTrick.CardCount == 3;
            Suit ledSuit = currentTrick.LedSuit.Value;
            bool canFollowSuit = legalPlays.Any(c => c.Suit == ledSuit);

            if (canFollowSuit)
                return ChooseFollowSuit(legalPlays, currentTrick, ledSuit, isLastToPlay);

            // Void in led suit — dump penalty cards
            return ChooseDump(legalPlays);
        }

        private Card ChooseLead(List<Card> legalPlays)
        {
            // Lead with lowest non-penalty card
            var safe = legalPlays.Where(c => !c.IsPenaltyCard).OrderBy(c => c.Rank).ToList();
            if (safe.Count > 0)
                return safe[0];

            // Only penalty cards — play lowest
            return legalPlays.OrderBy(c => c.Rank).First();
        }

        private Card ChooseFollowSuit(List<Card> legalPlays, Trick currentTrick, Suit ledSuit, bool isLastToPlay)
        {
            var suitCards = legalPlays.Where(c => c.Suit == ledSuit).OrderBy(c => c.Rank).ToList();

            if (isLastToPlay && currentTrick.PenaltyPoints == 0)
            {
                // No penalty in trick, safe to play highest
                return suitCards.Last();
            }

            // Try to play under the current highest card of led suit
            Rank highestPlayed = currentTrick.Plays
                .Where(p => p.Card.Suit == ledSuit)
                .Max(p => p.Card.Rank);

            var underCards = suitCards.Where(c => c.Rank < highestPlayed).ToList();
            if (underCards.Count > 0)
                return underCards.Last(); // Play highest card that still ducks

            // Must play over — play lowest (minimize future risk)
            return suitCards.First();
        }

        private Card ChooseDump(List<Card> legalPlays)
        {
            // Dump Queen of Spades if possible
            var queen = legalPlays.FirstOrDefault(c => c.IsQueenOfSpades);
            if (queen.IsQueenOfSpades)
                return queen;

            // Dump highest hearts
            var hearts = legalPlays.Where(c => c.Suit == Suit.Hearts).OrderByDescending(c => c.Rank).ToList();
            if (hearts.Count > 0)
                return hearts[0];

            // Dump highest spades (avoid taking Queen later)
            var highSpades = legalPlays.Where(c => c.Suit == Suit.Spades).OrderByDescending(c => c.Rank).ToList();
            if (highSpades.Count > 0)
                return highSpades[0];

            // Dump highest card of any suit
            return legalPlays.OrderByDescending(c => c.Rank).First();
        }

        public void OnTrickComplete(Trick completedTrick, int winnerSeat) { }
        public void OnRoundComplete(int[] roundScores, int[] cumulativeScores) { }
        public void OnGameOver(int winnerSeat, int[] finalScores) { }
    }
}
