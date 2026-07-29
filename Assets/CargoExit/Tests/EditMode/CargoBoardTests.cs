using System.Linq;
using NUnit.Framework;

namespace BannoyasGames.CargoExit.Core.Tests
{
    public sealed class CargoBoardTests
    {
        [Test]
        public void RotatingHorizontalParcelProducesVerticalParcel()
        {
            var shape = new ParcelShape(new[]
            {
                new GridPosition(0, 0),
                new GridPosition(1, 0)
            });

            var rotated = shape.GetCells(QuarterTurn.Clockwise90);

            CollectionAssert.AreEquivalent(
                new[]
                {
                    new GridPosition(0, 0),
                    new GridPosition(0, 1)
                },
                rotated);
        }

        [Test]
        public void RejectsPlacementOutsideBoard()
        {
            var board = new CargoBoard(2, 2);
            var parcel = Parcel("parcel", 1, HorizontalDomino());

            var result = board.TryPlace(new ParcelPlacement(
                parcel,
                new GridPosition(1, 0)));

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Failure, Is.EqualTo(PlacementFailure.OutOfBounds));
        }

        [Test]
        public void RejectsOverlap()
        {
            var board = new CargoBoard(2, 2);
            var first = Parcel("first", 1, SingleCell());
            var second = Parcel("second", 2, SingleCell());

            board.TryPlace(new ParcelPlacement(first, new GridPosition(0, 0)));
            var result = board.TryPlace(new ParcelPlacement(second, new GridPosition(0, 0)));

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Failure, Is.EqualTo(PlacementFailure.Overlap));
        }

        [Test]
        public void CanMoveAnAlreadyPlacedParcel()
        {
            var board = new CargoBoard(2, 2);
            var parcel = Parcel("parcel", 1, SingleCell());

            board.TryPlace(new ParcelPlacement(parcel, new GridPosition(0, 0)));
            var result = board.TryPlace(new ParcelPlacement(parcel, new GridPosition(1, 1)));

            Assert.That(result.Succeeded, Is.True);
            Assert.That(
                board.Placements.Single().Origin,
                Is.EqualTo(new GridPosition(1, 1)));
        }

        private static ParcelDefinition Parcel(
            string id,
            int order,
            ParcelShape shape)
        {
            return new ParcelDefinition(id, order, shape);
        }

        private static ParcelShape SingleCell()
        {
            return new ParcelShape(new[] { new GridPosition(0, 0) });
        }

        private static ParcelShape HorizontalDomino()
        {
            return new ParcelShape(new[]
            {
                new GridPosition(0, 0),
                new GridPosition(1, 0)
            });
        }
    }
}

