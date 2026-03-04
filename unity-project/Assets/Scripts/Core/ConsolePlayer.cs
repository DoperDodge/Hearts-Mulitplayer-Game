using System;
using System.Collections.Generic;
using System.Linq;

namespace Hearts.Core
{
    /// <summary>
    /// Human player that takes input from the console.
    /// Used for the debug prototype.
    /// </summary>
    public class ConsolePlayer : IPlayer
    {
        public int SeatIndex { get; }
        public string DisplayName { get; }
        public bool IsAI => false;
        public bool IsConnected => true;
        public Hand Hand { get; } = new Hand();

        public ConsolePlayer(int seatIndex, string name = null)
        {
            SeatIndex = seatIndex;
            DisplayName = name ?? "You";
        }

        public void OnCardsDealt(List<Card> cards)
        {
            Hand.Clear();
            Hand.AddCards(cards);
        }

        public void RequestPassCards(PassDirection direction, Action<List<Card>> onCardsSelected)
        {
            Console.WriteLine();
            Console.WriteLine($"=== PASS 3 CARDS ({direction}) ===");
            PrintHand();

            var selected = new List<Card>();
            for (int i = 0; i < 3; i++)
            {
                Console.Write($"  Select card {i + 1}/3 to pass (enter number): ");
                Card card = ReadCardSelection(Hand.Cards.Where(c => !selected.Contains(c)).ToList());
                selected.Add(card);
                Console.WriteLine($"  -> Selected: {card}");
            }

            Console.WriteLine($"  Passing: {string.Join(" ", selected)}");
            onCardsSelected(selected);
        }

        public void OnCardsReceived(List<Card> cards)
        {
            Hand.AddCards(cards);
            Console.WriteLine($"  Received: {string.Join(" ", cards)}");
        }

        public void RequestPlayCard(List<Card> legalPlays, Trick currentTrick, Action<Card> onCardPlayed)
        {
            Console.WriteLine();
            PrintTrick(currentTrick);
            Console.WriteLine("  Your hand:");
            PrintHandWithLegal(legalPlays);

            Console.Write("  Play a card (enter number): ");
            Card card = ReadCardSelection(legalPlays);
            onCardPlayed(card);
        }

        public void OnTrickComplete(Trick completedTrick, int winnerSeat) { }
        public void OnRoundComplete(int[] roundScores, int[] cumulativeScores) { }
        public void OnGameOver(int winnerSeat, int[] finalScores) { }

        private void PrintHand()
        {
            Console.Write("  Your hand: ");
            for (int i = 0; i < Hand.Cards.Count; i++)
            {
                Console.Write($"[{i + 1}]{Hand.Cards[i]} ");
            }
            Console.WriteLine();
        }

        private void PrintHandWithLegal(List<Card> legalPlays)
        {
            Console.Write("  ");
            int num = 1;
            for (int i = 0; i < Hand.Cards.Count; i++)
            {
                bool isLegal = legalPlays.Contains(Hand.Cards[i]);
                if (isLegal)
                {
                    Console.Write($"[{num}]{Hand.Cards[i]} ");
                    num++;
                }
                else
                {
                    Console.Write($"  {Hand.Cards[i]} ");
                }
            }
            Console.WriteLine();
        }

        private void PrintTrick(Trick trick)
        {
            if (trick.CardCount == 0)
            {
                Console.WriteLine("  Trick: (empty - you lead)");
                return;
            }

            Console.Write("  Trick: ");
            foreach (var play in trick.Plays)
            {
                Console.Write($"P{play.PlayerSeat}:{play.Card} ");
            }
            Console.WriteLine();
        }

        private Card ReadCardSelection(List<Card> validCards)
        {
            while (true)
            {
                string input = Console.ReadLine()?.Trim();
                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= validCards.Count)
                {
                    return validCards[choice - 1];
                }
                Console.Write($"  Invalid. Enter 1-{validCards.Count}: ");
            }
        }
    }
}
