namespace Hearts.Core
{
    /// <summary>
    /// Direction of card passing. Rotates each round: Left, Right, Across, Hold.
    /// </summary>
    public enum PassDirection
    {
        Left = 0,
        Right = 1,
        Across = 2,
        Hold = 3
    }

    public static class PassDirectionExtensions
    {
        /// <summary>
        /// Gets the pass direction for a given round number (0-indexed).
        /// Cycles: Left, Right, Across, Hold, Left, Right, ...
        /// </summary>
        public static PassDirection ForRound(int roundNumber)
        {
            return (PassDirection)(roundNumber % 4);
        }

        /// <summary>
        /// Gets the target player seat index given the source player seat index and pass direction.
        /// Seat indices are 0–3 in clockwise order.
        /// </summary>
        public static int GetTargetSeat(this PassDirection direction, int sourceSeat)
        {
            return direction switch
            {
                PassDirection.Left => (sourceSeat + 1) % 4,
                PassDirection.Right => (sourceSeat + 3) % 4, // +3 is equivalent to -1 mod 4
                PassDirection.Across => (sourceSeat + 2) % 4,
                PassDirection.Hold => sourceSeat, // No passing
                _ => sourceSeat
            };
        }
    }
}
