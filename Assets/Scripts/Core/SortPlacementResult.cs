namespace BannoyasGames.CargoExit.Core
{
    public readonly struct SortPlacementResult
    {
        public SortPlacementResult(
            SortPlacementStatus status,
            int sortedAtDestination,
            int requiredAtDestination,
            bool roundComplete)
        {
            Status = status;
            SortedAtDestination = sortedAtDestination;
            RequiredAtDestination = requiredAtDestination;
            RoundComplete = roundComplete;
        }

        public SortPlacementStatus Status { get; }

        public int SortedAtDestination { get; }

        public int RequiredAtDestination { get; }

        public bool RoundComplete { get; }

        public bool Accepted => Status == SortPlacementStatus.Accepted;
    }
}
