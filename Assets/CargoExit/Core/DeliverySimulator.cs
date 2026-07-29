using System.Collections.Generic;
using System.Linq;

namespace BannoyasGames.CargoExit.Core
{
    public static class DeliverySimulator
    {
        public static DeliveryResult Evaluate(CargoBoard board)
        {
            var occupancy = BuildOccupancy(board.Placements);
            var deliverySequence = board.Placements
                .OrderBy(item => item.Parcel.DeliveryOrder)
                .ThenBy(item => item.Parcel.Id)
                .ToArray();

            foreach (var placement in deliverySequence)
            {
                var blockers = FindBlockers(placement, occupancy);
                if (blockers.Count > 0)
                {
                    return DeliveryResult.Blocked(placement.Parcel.Id, blockers);
                }

                foreach (var cell in placement.OccupiedCells())
                {
                    occupancy.Remove(cell);
                }
            }

            return DeliveryResult.Success();
        }

        private static Dictionary<GridPosition, string> BuildOccupancy(
            IEnumerable<ParcelPlacement> placements)
        {
            return placements
                .SelectMany(placement => placement
                    .OccupiedCells()
                    .Select(cell => new KeyValuePair<GridPosition, string>(
                        cell,
                        placement.Parcel.Id)))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private static HashSet<string> FindBlockers(
            ParcelPlacement placement,
            IReadOnlyDictionary<GridPosition, string> occupancy)
        {
            var blockers = new HashSet<string>();

            foreach (var cell in placement.OccupiedCells())
            {
                for (var y = cell.Y - 1; y >= 0; y--)
                {
                    if (occupancy.TryGetValue(
                            new GridPosition(cell.X, y),
                            out var occupyingParcelId) &&
                        occupyingParcelId != placement.Parcel.Id)
                    {
                        blockers.Add(occupyingParcelId);
                    }
                }
            }

            return blockers;
        }
    }
}

