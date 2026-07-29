using NUnit.Framework;

namespace BannoyasGames.CargoExit.Core.Tests
{
    public sealed class DeliverySimulatorTests
    {
        [Test]
        public void ReportsParcelBlockingTheNextDelivery()
        {
            var board = new CargoBoard(2, 3);
            var firstDelivery = Parcel("first", 1);
            var secondDelivery = Parcel("second", 2);

            board.TryPlace(new ParcelPlacement(firstDelivery, new GridPosition(0, 1)));
            board.TryPlace(new ParcelPlacement(secondDelivery, new GridPosition(0, 0)));

            var result = DeliverySimulator.Evaluate(board);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.FailingParcelId, Is.EqualTo("first"));
            CollectionAssert.Contains(result.BlockingParcelIds, "second");
        }

        [Test]
        public void DeliversEveryParcelWhenExitOrderIsClear()
        {
            var board = new CargoBoard(2, 3);
            var firstDelivery = Parcel("first", 1);
            var secondDelivery = Parcel("second", 2);

            board.TryPlace(new ParcelPlacement(firstDelivery, new GridPosition(0, 0)));
            board.TryPlace(new ParcelPlacement(secondDelivery, new GridPosition(0, 1)));

            var result = DeliverySimulator.Evaluate(board);

            Assert.That(result.Succeeded, Is.True);
        }

        private static ParcelDefinition Parcel(string id, int order)
        {
            return new ParcelDefinition(
                id,
                order,
                new ParcelShape(new[] { new GridPosition(0, 0) }));
        }
    }
}

