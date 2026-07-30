using NUnit.Framework;

namespace BannoyasGames.CargoExit.Core.Tests
{
    public sealed class CargoSortSessionTests
    {
        [Test]
        public void TryPlace_AcceptsParcelAtItsDestination()
        {
            var session = CreateSession();

            var result = session.TryPlace("P1", CargoDestination.A);

            Assert.That(result.Status, Is.EqualTo(SortPlacementStatus.Accepted));
            Assert.That(result.SortedAtDestination, Is.EqualTo(1));
            Assert.That(session.SortedParcels, Is.EqualTo(1));
        }

        [Test]
        public void TryPlace_RejectsWrongDestinationWithoutProgress()
        {
            var session = CreateSession();

            var result = session.TryPlace("P1", CargoDestination.B);

            Assert.That(
                result.Status,
                Is.EqualTo(SortPlacementStatus.WrongDestination));
            Assert.That(session.SortedParcels, Is.Zero);
        }

        [Test]
        public void TryPlace_RejectsSameParcelTwice()
        {
            var session = CreateSession();
            session.TryPlace("P1", CargoDestination.A);

            var result = session.TryPlace("P1", CargoDestination.A);

            Assert.That(
                result.Status,
                Is.EqualTo(SortPlacementStatus.AlreadySorted));
            Assert.That(session.SortedParcels, Is.EqualTo(1));
        }

        [Test]
        public void TryPlace_CompletesOnlyAfterEveryPlannedParcel()
        {
            var session = CreateSession();

            var first = session.TryPlace("P1", CargoDestination.A);
            var second = session.TryPlace("P2", CargoDestination.B);

            Assert.That(first.RoundComplete, Is.False);
            Assert.That(second.RoundComplete, Is.True);
            Assert.That(session.IsComplete, Is.True);
        }

        private static CargoSortSession CreateSession()
        {
            return new CargoSortSession(new[]
            {
                new CargoParcelPlan("P1", CargoDestination.A),
                new CargoParcelPlan("P2", CargoDestination.B)
            });
        }
    }
}
