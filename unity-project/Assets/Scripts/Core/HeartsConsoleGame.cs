using System;
using System.Linq;

namespace Hearts.Core
{
    /// <summary>
    /// Debug console prototype. Runs a full game of Hearts in the terminal.
    /// Player 0 = Human, Players 1-3 = AI.
    /// </summary>
    public class HeartsConsoleGame
    {
        private GameManager _game;
        private IPlayer[] _players;
        private int _trickNumber;

        public void Run()
        {
            PrintTitle();
            SetupGame();
            SubscribeEvents();
            _game.StartGame();
        }

        private void PrintTitle()
        {
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║     HEARTS - Debug Console Build     ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.WriteLine();
        }

        private void SetupGame()
        {
            var settings = GameSettings.Default;
            _game = new GameManager(settings);

            _players = new IPlayer[]
            {
                new ConsolePlayer(0, "You"),
                new AIPlayer(1, "Alice"),
                new AIPlayer(2, "Bob"),
                new AIPlayer(3, "Carol")
            };

            for (int i = 0; i < 4; i++)
            {
                _game.SetPlayer(i, _players[i]);
            }
        }

        private void SubscribeEvents()
        {
            _game.OnRoundStarted += (roundNum, passDir) =>
            {
                Console.WriteLine();
                Console.WriteLine($"{'=',-40}");
                Console.WriteLine($"  ROUND {roundNum + 1}  |  Pass: {passDir}");
                Console.WriteLine($"{'=',-40}");
                _trickNumber = 0;
            };

            _game.CurrentRound?.OnCardPlayed += OnCardPlayed;
            _game.CurrentRound?.OnTrickCompleted += OnTrickCompleted;
            _game.CurrentRound?.OnHeartsBroken += OnHeartsBroken;

            // Re-subscribe each round since CurrentRound changes
            _game.OnRoundStarted += (_, __) =>
            {
                _game.CurrentRound.OnCardPlayed += OnCardPlayed;
                _game.CurrentRound.OnTrickCompleted += OnTrickCompleted;
                _game.CurrentRound.OnHeartsBroken += OnHeartsBroken;
            };

            _game.OnRoundScored += OnRoundScored;
            _game.OnGameOver += OnGameOver;
        }

        private void OnCardPlayed(int seat, Card card)
        {
            if (seat != 0) // Don't duplicate human player's output
            {
                string name = _players[seat].DisplayName;
                Console.WriteLine($"  {name} plays {card}");
            }
        }

        private void OnTrickCompleted(Trick trick, int winnerSeat)
        {
            _trickNumber++;
            string winner = _players[winnerSeat].DisplayName;
            int points = trick.PenaltyPoints;

            Console.WriteLine($"  --- Trick {_trickNumber}: {winner} wins" +
                (points > 0 ? $" (+{points} pts)" : "") + " ---");
        }

        private void OnHeartsBroken()
        {
            Console.WriteLine();
            Console.WriteLine("  *** HEARTS BROKEN ***");
            Console.WriteLine();
        }

        private void OnRoundScored(int[] roundScores, int[] cumulativeScores)
        {
            Console.WriteLine();
            Console.WriteLine("  ┌────────────────────────────────────┐");
            Console.WriteLine("  │          ROUND SCORES              │");
            Console.WriteLine("  ├──────────┬──────────┬──────────────┤");
            Console.WriteLine("  │ Player   │ Round    │ Total        │");
            Console.WriteLine("  ├──────────┼──────────┼──────────────┤");

            // Check for Shoot the Moon
            bool moonShot = roundScores.Any(s => s == 0) &&
                            roundScores.Any(s => s == 26) &&
                            roundScores.Count(s => s == 26) == 3;

            for (int i = 0; i < 4; i++)
            {
                string name = _players[i].DisplayName.PadRight(8);
                string round = roundScores[i].ToString().PadLeft(5);
                string total = cumulativeScores[i].ToString().PadLeft(8);
                Console.WriteLine($"  │ {name} │ {round}    │ {total}      │");
            }

            Console.WriteLine("  └──────────┴──────────┴──────────────┘");

            if (moonShot)
            {
                Console.WriteLine("  *** SOMEONE SHOT THE MOON! ***");
            }

            Console.WriteLine();
            Console.WriteLine("  Press Enter to continue...");
            Console.ReadLine();
        }

        private void OnGameOver(int winnerSeat, int[] finalScores)
        {
            Console.WriteLine();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║            GAME OVER                 ║");
            Console.WriteLine("╠══════════════════════════════════════╣");

            for (int i = 0; i < 4; i++)
            {
                string marker = (i == winnerSeat) ? " <<< WINNER" : "";
                Console.WriteLine($"║  {_players[i].DisplayName,-10}  {finalScores[i],5} pts{marker,-14}║");
            }

            Console.WriteLine("╚══════════════════════════════════════╝");
        }

        /// <summary>
        /// Entry point for running the debug prototype.
        /// </summary>
        public static void Main(string[] args)
        {
            var game = new HeartsConsoleGame();
            game.Run();
        }
    }
}
