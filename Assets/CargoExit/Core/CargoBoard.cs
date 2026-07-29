using System;
using System.Collections.Generic;
using System.Linq;

namespace BannoyasGames.CargoExit.Core
{
    public sealed class CargoBoard
    {
        private readonly Dictionary<string, ParcelPlacement> placements = new();

        public CargoBoard(int width, int height)
        {
            if (width < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }
        public IReadOnlyCollection<ParcelPlacement> Placements => placements.Values;

        public PlacementResult TryPlace(ParcelPlacement candidate)
        {
            if (candidate == null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            var occupiedByOthers = placements.Values
                .Where(item => item.Parcel.Id != candidate.Parcel.Id)
                .SelectMany(item => item.OccupiedCells())
                .ToHashSet();

            var candidateCells = candidate.OccupiedCells();

            if (candidateCells.Any(cell =>
                    cell.X < 0 ||
                    cell.Y < 0 ||
                    cell.X >= Width ||
                    cell.Y >= Height))
            {
                return PlacementResult.Failed(PlacementFailure.OutOfBounds);
            }

            if (candidateCells.Any(occupiedByOthers.Contains))
            {
                return PlacementResult.Failed(PlacementFailure.Overlap);
            }

            placements[candidate.Parcel.Id] = candidate;
            return PlacementResult.Success();
        }

        public bool Remove(string parcelId)
        {
            return placements.Remove(parcelId);
        }
    }
}

