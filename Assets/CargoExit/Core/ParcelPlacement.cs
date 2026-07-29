using System.Collections.Generic;
using System.Linq;

namespace BannoyasGames.CargoExit.Core
{
    public sealed class ParcelPlacement
    {
        public ParcelPlacement(
            ParcelDefinition parcel,
            GridPosition origin,
            QuarterTurn rotation = QuarterTurn.None)
        {
            Parcel = parcel;
            Origin = origin;
            Rotation = rotation;
        }

        public ParcelDefinition Parcel { get; }
        public GridPosition Origin { get; }
        public QuarterTurn Rotation { get; }

        public IReadOnlyList<GridPosition> OccupiedCells()
        {
            return Parcel.Shape
                .GetCells(Rotation)
                .Select(cell => cell + Origin)
                .ToArray();
        }
    }
}

