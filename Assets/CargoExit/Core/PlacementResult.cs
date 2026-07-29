namespace BannoyasGames.CargoExit.Core
{
    public enum PlacementFailure
    {
        None,
        OutOfBounds,
        Overlap
    }

    public readonly struct PlacementResult
    {
        private PlacementResult(bool succeeded, PlacementFailure failure)
        {
            Succeeded = succeeded;
            Failure = failure;
        }

        public bool Succeeded { get; }
        public PlacementFailure Failure { get; }

        public static PlacementResult Success()
        {
            return new PlacementResult(true, PlacementFailure.None);
        }

        public static PlacementResult Failed(PlacementFailure failure)
        {
            return new PlacementResult(false, failure);
        }
    }
}

