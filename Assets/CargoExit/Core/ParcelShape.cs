using System;
using System.Collections.Generic;
using System.Linq;

namespace BannoyasGames.CargoExit.Core
{
    public sealed class ParcelShape
    {
        private readonly GridPosition[] cells;

        public ParcelShape(IEnumerable<GridPosition> cells)
        {
            if (cells == null)
            {
                throw new ArgumentNullException(nameof(cells));
            }

            this.cells = Normalize(cells).ToArray();

            if (this.cells.Length == 0)
            {
                throw new ArgumentException("A parcel shape needs at least one cell.", nameof(cells));
            }

            if (this.cells.Distinct().Count() != this.cells.Length)
            {
                throw new ArgumentException("A parcel shape cannot contain duplicate cells.", nameof(cells));
            }
        }

        public IReadOnlyList<GridPosition> GetCells(QuarterTurn turn)
        {
            IEnumerable<GridPosition> rotated = cells;

            for (var i = 0; i < (int)turn; i++)
            {
                rotated = rotated.Select(RotateClockwise).ToArray();
            }

            return Normalize(rotated).ToArray();
        }

        private static GridPosition RotateClockwise(GridPosition cell)
        {
            return new GridPosition(cell.Y, -cell.X);
        }

        private static IEnumerable<GridPosition> Normalize(IEnumerable<GridPosition> source)
        {
            var materialized = source.ToArray();
            if (materialized.Length == 0)
            {
                return materialized;
            }

            var minimumX = materialized.Min(cell => cell.X);
            var minimumY = materialized.Min(cell => cell.Y);

            return materialized
                .Select(cell => new GridPosition(cell.X - minimumX, cell.Y - minimumY))
                .OrderBy(cell => cell.Y)
                .ThenBy(cell => cell.X);
        }
    }
}

